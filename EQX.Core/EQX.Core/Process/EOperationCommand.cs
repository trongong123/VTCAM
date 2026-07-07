namespace EQX.Core.Sequence
{
    /// <summary>
    /// Operator (user) selected command
    /// </summary>
    public enum EOperationCommand
    {
        /// <summary>
        /// No active command
        /// </summary>
        None,
        /// <summary>
        /// User request Origin command
        /// </summary>
        Origin,
        /// <summary>
        /// User request Ready command
        /// </summary>
        Ready,
        /// <summary>
        /// User request Start command
        /// </summary>
        Start,
        /// <summary>
        /// User request Stop command
        /// </summary>
        Stop,
        /// <summary>
        /// User request Reset command
        /// <br/>This command should be set with specific sequence (SemiAuto sequence)
        /// </summary>
        Reset,
        /// <summary>
        /// User request SemiAuto command
        /// <br/>This command should be set with specific sequence (SemiAuto sequence)
        /// </summary>
        SemiAuto,
    }
}
