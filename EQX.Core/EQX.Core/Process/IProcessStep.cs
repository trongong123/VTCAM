namespace EQX.Core.Sequence
{
    public interface IProcessStep
    {
        event EventHandler? StepChangedHandler;
        int OriginStep { get; set; }
        int RunStep { get; set; }
        int ToRunStep { get; set; }

        int PreProcessStep { get; set; }
    }
}
