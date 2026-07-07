using EQX.Core.Common;

namespace EQX.Core.Vision.Algorithms
{
    public delegate void ToolRunCallback(string errorMessage, IObjectCollection outputs);

    public interface IVisionTool : IIdentifier, IAsyncRunnable
    {
        string ErrorMessage { get; }
        ToolRunCallback ToolRunFinished { get; set; }

        IObjectCollection Parameters { get; }

        /// <summary>
        /// Including input image
        /// </summary>
        IObjectCollection Inputs { get; }
        IObjectCollection Outputs { get; }

    }
}