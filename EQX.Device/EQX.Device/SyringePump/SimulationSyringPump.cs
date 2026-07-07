using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Device.SyringePump
{
    public class SimulationSyringePump : SyringePumpBase
    {
        public SimulationSyringePump(string name, int id) : base(name, id)
        {
            Name = name;
            Id = id;
        }

        public override bool IsConnected => true;

        public override bool Connect()
        {
            return true;
        }
        public override bool Disconnect()
        {
            return true;
        }
        public override void Dispense(double volume, int port)
        {

        }
        public override void Fill(double volume)
        {

        }
        public override void Initialize()
        {

        }

        public override void SetAcceleration(int accCode)
        {

        }

        public override void SetDeccelation(int decCode)
        {

        }

        public override void SetSpeed(int speed)
        {

        }

        public override void Stop()
        {

        }

        public override void Reset()
        {

        }
    }
}
