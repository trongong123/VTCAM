using EQX.Core.Device.UVLed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Device.UVLed
{
    public class UVLedBase : IUVLed
    {
        public int Id { get; init; }

        public string Name { get; set; }

        public virtual bool IsConnected { get; protected set; }

        public UVLedBase(string name, int id)
        {
            Name = name;
            Id = id;
        }
        public virtual bool AlarmCheck()
        {
            return false;
        }

        public virtual bool Connect()
        {
            return true;
        }

        public virtual bool Disconnect()
        {
            return true;
        }

        public virtual void LedOnTime(double second)
        {

        }

        public virtual void SetAll(bool isOn)
        {

        }

        public virtual void SetValue(double value)
        {

        }
    }
}
