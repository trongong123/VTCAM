namespace EQX.Core.InOut
{
    public interface ICylinderFactory
    {
        ICylinder Create(IDInput? inForward, IDInput? inBackward, IDOutput? outForward, IDOutput? outBackward);
    }
}
