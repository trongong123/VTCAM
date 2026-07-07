using CommunityToolkit.Mvvm.ComponentModel;
using EQX.Core.Common;

namespace EQX.Core.InOut
{
    public interface ICylinder : IIdentifier
    {
        ECylinderType CylinderType { get; set; }
        Dictionary<string, Func<bool>>? ForwardInterlocks { get; set; }
        Dictionary<string, Func<bool>>? BackwardInterlocks { get; set; }
        bool IsForward { get; }
        bool IsBackward { get; }
        void Forward();
        void Backward();
        void UpdateIOStatus();
    }
}
