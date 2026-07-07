namespace EQX.Core.InOut
{
    /// <summary>
    /// <para/>Non Reverse :
    /// <para/>    Forward = Up = Right = Grip = Algin = Lock = Flip = Clamp
    /// <para/>    Backward = Down = Left = Ungrip = Unalign = Unlock = Unflip = Unclamp
    /// 
    /// <para/>Reverse :
    /// <para/>    Forward = Down = Left = Ungrip = Unalign = Unlock = UnFlip = Unclamp
    /// <para/>    Backward = Up = Right = Grip = Algin = Lock = Flip = Clamp
    /// </summary>
    public enum ECylinderType
    {
        ForwardBackward,
        ForwardBackwardReverse,
        UpDown,
        UpDownReverse,
        OpenClose,
        OpenCloseReverse,
        RightLeft,
        RightLeftReverse,
        GripUngrip,
        GripUngripReverse,
        AlignUnalign,
        AlignUnalignReverse,
        LockUnlock,
        LockUnlockReverse,
        FlipUnflip, //Flip = 180°, Unflip = 0°
        FlipUnflipReverse,
        ClampUnclamp,
        ClampUnclampReverse
    }
}
