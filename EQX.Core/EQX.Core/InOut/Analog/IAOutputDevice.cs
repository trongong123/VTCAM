using EQX.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EQX.Core.InOut.Analog
{
    public interface IAOutputDevice : IHandleConnection , IIdentifier
    {
        List<IAOutput> AnalogOutputs { get; }

        bool Initialize();

        double GetVolt(int channel);
        void SetVolt(int channel, double voltage);
    }
}
