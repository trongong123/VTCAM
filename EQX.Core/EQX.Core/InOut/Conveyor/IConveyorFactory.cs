namespace EQX.Core.InOut.Conveyor
{
    public interface IConveyorFactory
    {
        IConveyor Create(IDInput? inError, IDOutput? outRun, IDOutput? outReverseRun, IDOutput? outStop);
    }
}
