namespace EQX.Core.InOut
{
    public interface IDInput
    {
        int Id { get; init; }
        string Name { get; init; }
        bool Value { get; }

        int SimulationOffset { get; }

        void RaiseValueUpdated();
        event EventHandler? ValueUpdated;
        event EventHandler? ValueChanged;
    }
}