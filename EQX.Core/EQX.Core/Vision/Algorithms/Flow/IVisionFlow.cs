using EQX.Core.Common;
using System.Collections.ObjectModel;

namespace EQX.Core.Vision.Algorithms
{
    public delegate void VisionFlowRunFinishedHandler(object sender, IObjectCollection result);
    public interface IVisionFlow : IIdentifier, IAsyncRunnable
    {
        event VisionFlowRunFinishedHandler VisionFlowRunFinished;
        ObservableCollection<IVisionTool> VisionTools { get; }
        ObservableCollection<VisionToolConnection> VisionToolConnections { get; }
        IObjectCollection Outputs { get; }
        IObjectCollection Inputs { get; }

        string Script { get; set; }
        string ErrorMessage { get; set; }

        IVisionTool this[string toolName] { get; }

        IVisionFlow Clone();

        bool WarmUpCompiler();

    }
}