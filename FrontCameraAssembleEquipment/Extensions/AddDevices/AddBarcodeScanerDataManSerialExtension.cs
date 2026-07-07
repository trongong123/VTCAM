using EQX.Core.Communication;
using EQX.Core.Device.BarCodeScanner;
using EQX.Device.CognexDataMan150X;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Ports;

namespace FrontCameraAssembleEquipment.Extensions
{
    public static class AddBarcodeScanerDataManSerialExtension
    {
        public static IHostBuilder AddBarcodeReader(this IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddKeyedScoped<SerialCommunicator>("DataManSerialCommunication", (services, obj) =>
                {
                    return new SerialCommunicator(1, "DataMan", "COM2", 115200, Parity.None, 8, StopBits.One);
                });

#if SIMULATION
                services.AddSingleton<IBarCodeScanner, SimulationDataManBarCodeScanner>((ser) =>
                {
                    return new SimulationDataManBarCodeScanner(1, "DataMan");
                });
#else
                services.AddSingleton<IBarCodeScanner, DataMan150XBarCodeScanner>((ser) =>
                {
                    SerialCommunicator serialCommunicator = ser.GetRequiredKeyedService<SerialCommunicator>("DataManSerialCommunication");
                    return new DataMan150XBarCodeScanner(1, "DataMan", serialCommunicator);
                });
#endif
            });

            return hostBuilder;
        }
    }
}
