namespace EQX.Device.Regulator
{
    public class SimulationRegulator : RegulatorBase
    {
        private readonly double _maxPressure;
        private double _pressure;
        public SimulationRegulator(int id, string name) : base(id, name)
        {

        }

        public override bool Connect()
        {
            IsConnected = true;
            return true;
        }

        public override bool Disconnect()
        {
            IsConnected = false;
            return true;
        }

        public override bool SetPressure(double value)
        {
            _pressure = Math.Clamp(value, 0, _maxPressure);
            return true;
        }

        public override bool IncreasePressure()
        {
            return SetPressure(_pressure + 0.1);
        }

        public override bool DecreasePressure()
        {
            return SetPressure(_pressure - 0.1);
        }

        public override double GetPressure()
        {
            return _pressure;
        }

    }
}
