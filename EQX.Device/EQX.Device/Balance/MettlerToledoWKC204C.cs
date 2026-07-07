using EQX.Core.Communication;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EQX.Device.Balance
{
    /// <summary>
    /// Contains data from a weight measurement event.
    /// </summary>
    public class WeightEventArgs : EventArgs
    {
        public double Weight { get; }
        public string Unit { get; }
        public bool IsStable { get; }
        public string RawData { get; }

        public WeightEventArgs(double weight, string unit, bool isStable, string rawData)
        {
            Weight = weight;
            Unit = unit;
            IsStable = isStable;
            RawData = rawData;
        }
    }

    /// <summary>
    /// Represents a driver for the Mettler Toledo WKC204C balance via serial communication.
    /// This driver provides both synchronous (blocking) and asynchronous (Task-based) methods
    /// for interacting with the device, based on the Mettler-Toledo Standard Interface Command Set.
    /// </summary>
    public class MettlerToledoWKC204C : IDisposable
    {
        private readonly SerialCommunicator _serialCommunicator;
        private readonly SemaphoreSlim _requestLock = new SemaphoreSlim(1, 1);
        private readonly object _connectLock = new object();
        private bool _disposed = false;
        private Task? _readTask;
        private CancellationTokenSource? _cancellationTokenSource;
        private TaskCompletionSource<WeightEventArgs?>? _weightRequestTcs;

        /// <summary>
        /// Fires when a valid weight measurement is received and parsed.
        /// </summary>
        public event EventHandler<WeightEventArgs>? WeightReceived;
        /// <summary>
        /// Fires whenever any raw data is received from the device.
        /// </summary>
        public event EventHandler<string>? RawDataReceived;

        public WeightEventArgs? WeightData { get; private set; }

        public bool IsConnected => _serialCommunicator.IsConnected;

        public MettlerToledoWKC204C(SerialCommunicator serialCommunicator)
        {
            _serialCommunicator = serialCommunicator ?? throw new ArgumentNullException(nameof(serialCommunicator));
        }

        public bool Connect()
        {
            if (IsConnected) return false;

            lock (_connectLock)
            {
                try
                {
                    if (!_serialCommunicator.Connect()) return false;

                    _cancellationTokenSource = new CancellationTokenSource();
                    _readTask = Task.Run(async () => ReadDataContinuously(_cancellationTokenSource.Token));
                    return true;
                }
                catch
                {
                    _serialCommunicator.Disconnect();
                    return false;
                }
            }
        }

        public bool Disconnect()
        {
            lock (_connectLock)
            {
                _weightRequestTcs?.TrySetCanceled();
                _cancellationTokenSource?.Cancel();
                var result = _serialCommunicator.Disconnect();

                try
                {
                    _readTask?.Wait(1000);
                }
                catch (AggregateException) { /* Ignore task cancellation exceptions */ }

                _readTask = null;
                _cancellationTokenSource?.Dispose();
                _weightRequestTcs = null;
                _cancellationTokenSource = null;

                return result;
            }
        }

        private bool SendCommand(string command)
        {
            if (!IsConnected) return false;
            lock (_connectLock) // Use the same lock to prevent sending while disconnecting
            {
                try
                {
                    _serialCommunicator.WriteLine(command);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Sends a "Zero" command without waiting for a response.
        /// </summary>
        public bool SendZeroCommand()
        {
            WeightData = null;
            return SendCommand("Z");
        }

        /// <summary>
        /// Sends a "Tare" command without waiting for a response.
        /// </summary>
        public bool SendTareCommand() => SendCommand("T");

        /// <summary>
        /// Sends a "Reset" command without waiting for a response.
        /// </summary>
        public bool SendResetCommand() => SendCommand("@");

        /// <summary>
        /// Sends a command to request a stable weight value, without waiting for the response.
        /// </summary>
        public void SendRequestStableWeightCommand()
        {
            WeightData = null;
            SendCommand("S");
        }

        /// <summary>
        /// Sends a command to request an immediate weight value, without waiting for the response.
        /// </summary>
        public void SendRequestImmediateWeightCommand()
        {
            WeightData = null;
            SendCommand("SI");
        }

        /// <summary>
        /// Synchronously performs a zero operation and waits for confirmation.
        /// </summary>
        /// <returns>True if the operation was acknowledged as successful, otherwise false.</returns>
        public bool Zero(int timeoutMilliseconds = 5000)
        {
            try
            {
                return ZeroAsync(timeoutMilliseconds).Result;
            }
            catch { return false; }
        }

        /// <summary>
        /// Synchronously performs a tare operation and waits for confirmation.
        /// </summary>
        /// <returns>True if the operation was acknowledged as successful, otherwise false.</returns>
        public bool Tare(int timeoutMilliseconds = 5000)
        {
            try
            {
                return TareAsync(timeoutMilliseconds).Result;
            }
            catch { return false; }
        }

        /// <summary>
        /// Synchronously performs a reset operation and waits for confirmation.
        /// </summary>
        /// <returns>True if the operation was acknowledged as successful, otherwise false.</returns>
        public bool Reset(int timeoutMilliseconds = 5000)
        {
            try
            {
                return ResetAsync(timeoutMilliseconds).Result;
            }
            catch { return false; }
        }

        /// <summary>
        /// Synchronously requests and returns a stable weight reading.
        /// </summary>
        /// <returns>Weight data if successful, otherwise null.</returns>
        public WeightEventArgs? GetStableWeight(int timeoutMilliseconds = 5000)
        {
            try
            {
                return RequestStableWeightAsync(timeoutMilliseconds).Result;
            }
            catch { return null; }
        }

        /// <summary>
        /// Synchronously requests and returns an immediate weight reading.
        /// </summary>
        /// <returns>Weight data if successful, otherwise null.</returns>
        public WeightEventArgs? GetImmediateWeight(int timeoutMilliseconds = 5000)
        {
            try
            {
                return RequestImmediateWeightAsync(timeoutMilliseconds).Result;
            }
            catch { return null; }
        }

        /// <summary>
        /// Asynchronously performs a zero operation and waits for confirmation.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is true if successful, otherwise false.</returns>
        public async Task<bool> ZeroAsync(int timeoutMilliseconds = 5000)
        {
            var result = await ExecuteWeightRequestAsync("Z", timeoutMilliseconds);
            return result != null; // A non-null result indicates success acknowledgement
        }

        /// <summary>
        /// Asynchronously requests a stable weight reading from the balance.
        /// </summary>
        /// <param name="timeoutMilliseconds">The time to wait for a response.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the weight data, or null if the operation fails or times out.</returns>
        public Task<WeightEventArgs?> RequestStableWeightAsync(int timeoutMilliseconds = 5000) =>
            ExecuteWeightRequestAsync("S", timeoutMilliseconds);

        /// <summary>
        /// Asynchronously requests an immediate weight reading from the balance.
        /// </summary>
        public Task<WeightEventArgs?> RequestImmediateWeightAsync(int timeoutMilliseconds = 5000) =>
            ExecuteWeightRequestAsync("SI", timeoutMilliseconds);

        /// <summary>
        /// Asynchronously performs a tare operation and waits for confirmation.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is true if successful, otherwise false.</returns>
        public async Task<bool> TareAsync(int timeoutMilliseconds = 5000)
        {
            var result = await ExecuteWeightRequestAsync("T", timeoutMilliseconds);
            return result != null; // A non-null result indicates success acknowledgement
        }

        /// <summary>
        /// Asynchronously performs a reset operation and waits for confirmation.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result is true if successful, otherwise false.</returns>
        public async Task<bool> ResetAsync(int timeoutMilliseconds = 5000)
        {
            var result = await ExecuteWeightRequestAsync("@", timeoutMilliseconds);
            return result != null; // A non-null result indicates success acknowledgement
        }
        private async Task<WeightEventArgs?> ExecuteWeightRequestAsync(string command, int timeoutMilliseconds)
        {
            if (!IsConnected) throw new InvalidOperationException("Balance is not connected.");

            await _requestLock.WaitAsync();
            try
            {
                _weightRequestTcs = new TaskCompletionSource<WeightEventArgs?>();
                if (!SendCommand(command)) throw new InvalidOperationException("Failed to send command to the balance.");

                return await _weightRequestTcs.Task.WaitAsync(TimeSpan.FromMilliseconds(timeoutMilliseconds));
            }
            finally { _requestLock.Release(); }
        }

        private async Task ReadDataContinuously(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && IsConnected)
            {
                try
                {
                    string data = _serialCommunicator.Read();
                    if (!string.IsNullOrEmpty(data))
                    {
                        RawDataReceived?.Invoke(this, data);
                        ParseResponse(data);
                    }
                    await Task.Delay(50);
                }
                catch (Exception)
                {
                    if (!cancellationToken.IsCancellationRequested) { /* Handle disconnection */ }
                }
            }
        }

        private void ParseResponse(string data)
        {
            // Regex for weight responses (e.g., "S S 123.45 g")
            const string weightPattern = @"^\s*[A-Z]+\s+([SD?])\s+([-\d\.]+)\s*(\w+)";
            // Regex for simple acknowledgement responses (e.g., "Z A", "T A", "@ A")
            const string ackPattern = @"^\s*([ZT@])\s+A";

            var lines = data.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine)) continue;

                RawDataReceived?.Invoke(this, trimmedLine);

                // Try to match a weight response
                var weightMatch = Regex.Match(trimmedLine, weightPattern);
                if (weightMatch.Success)
                {
                    try
                    {
                        bool isStable = weightMatch.Groups[1].Value == "S";
                        double weight = double.Parse(weightMatch.Groups[2].Value, CultureInfo.InvariantCulture);
                        string unit = weightMatch.Groups[3].Value;

                        var eventArgs = new WeightEventArgs(weight, unit, isStable, trimmedLine);
                        WeightData = eventArgs;

                        WeightReceived?.Invoke(this, eventArgs);
                        _weightRequestTcs?.TrySetResult(eventArgs);
                        continue; // Move to the next line
                    }
                    catch (FormatException) { /* Failed to parse, ignore and try other patterns */ }
                }

                // Try to match a simple acknowledgement response
                var ackMatch = Regex.Match(trimmedLine, ackPattern);
                if (ackMatch.Success)
                {
                    // Create a dummy event args to signal success for commands like Zero, Tare, Reset
                    var eventArgs = new WeightEventArgs(0, "g", true, trimmedLine);
                    WeightData = eventArgs; // Update last known data

                    WeightReceived?.Invoke(this, eventArgs);
                    _weightRequestTcs?.TrySetResult(eventArgs);
                    continue; // Move to the next line
                }

                // Handle other known error/status responses (e.g., "S L", "S I", "S +", "S -")
                // For now, we'll treat any un-parsable but valid-looking response as a failure for a pending request.
                var parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    // It's a response, but not one we successfully parsed as data or ack.
                    // Consider it a failure for the pending request.
                    _weightRequestTcs?.TrySetResult(null);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Disconnect();
                    _requestLock.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
