using EQX.Core.InOut;

namespace EQX.InOut
{
    public static class CylinderHelpers
    {
        public static ICylinder SetIdentity(this ICylinder cylinder, int id, string name)
        {
            // [SUGGESTION] This cast breaks the abstraction of the ICylinder interface.
            // It creates a hard dependency on the 'CylinderBase' implementation.
            // A better approach is to include 'Id' and 'Name' properties in the ICylinder interface itself.
            if (cylinder is CylinderBase concreteCylinder)
            {
                concreteCylinder.Id = id;
                concreteCylinder.Name = name;
            }

            return cylinder;
        }

        // THE CODE AFTER THIS IS WRITTEN BY GEMINI (AI)
        // ========================================================================
        // PRIVATE CORE LOGIC (THE "ENGINE")
        // ========================================================================

        /// <summary>
        /// Xử lý logic điều khiển (Action) chung cho tất cả các cặp
        /// </summary>
        /// <param name="cylinder">Đối tượng cylinder</param>
        /// <param name="normalType">Kiểu thuận (VD: OpenClose)</param>
        /// <param name="reverseType">Kiểu nghịch (VD: OpenCloseReverse)</param>
        /// <param name="isPrimaryAction">
        /// True nếu là hành động tương đương Forward ở chế độ thuận (VD: Open, Up, Grip...).
        /// False nếu là hành động tương đương Backward ở chế độ thuận.
        /// </param>
        private static void ExecuteAction(ICylinder cylinder, ECylinderType normalType, ECylinderType reverseType, bool isPrimaryAction)
        {
            if (cylinder.CylinderType == normalType)
            {
                if (isPrimaryAction) cylinder.Forward();
                else cylinder.Backward();
            }
            else if (cylinder.CylinderType == reverseType)
            {
                // Logic đảo ngược: Primary Action sẽ kích hoạt Backward
                if (isPrimaryAction) cylinder.Backward();
                else cylinder.Forward();
            }
            else
            {
                throw new InvalidOperationException($"Cylinder '{cylinder.Name}' (Id: {cylinder.Id}) has type '{cylinder.CylinderType}', but expected '{normalType}' or '{reverseType}'.");
            }
        }

        /// <summary>
        /// Xử lý logic kiểm tra trạng thái (Status Check) chung
        /// </summary>
        private static bool CheckStatus(ICylinder cylinder, ECylinderType normalType, ECylinderType reverseType, bool isPrimaryState)
        {
            if (cylinder.CylinderType == normalType)
            {
                return isPrimaryState ? cylinder.IsForward : cylinder.IsBackward;
            }
            else if (cylinder.CylinderType == reverseType)
            {
                // Logic đảo ngược: Primary State sẽ check IsBackward
                return isPrimaryState ? cylinder.IsBackward : cylinder.IsForward;
            }
            else
            {
                throw new InvalidOperationException($"Cylinder '{cylinder.Name}' (Id: {cylinder.Id}) has type '{cylinder.CylinderType}', but expected '{normalType}' or '{reverseType}'.");
            }
        }

        /// <summary>
        /// Kiểm tra trạng thái lệnh ra output (van/coil), tương ứng <see cref="CylinderBase.IsOutForward"/> / <see cref="CylinderBase.IsOutBackward"/>.
        /// </summary>
        private static bool CheckOutputStatus(ICylinder cylinder, ECylinderType normalType, ECylinderType reverseType, bool isPrimaryState)
        {
            if (cylinder is not CylinderBase cb)
            {
                throw new InvalidOperationException($"Output status checks require a {nameof(CylinderBase)} implementation. Cylinder '{cylinder.Name}' (Id: {cylinder.Id}) is '{cylinder.GetType().Name}'.");
            }

            if (cylinder.CylinderType == normalType)
            {
                return isPrimaryState ? cb.IsOutForward : cb.IsOutBackward;
            }
            else if (cylinder.CylinderType == reverseType)
            {
                return isPrimaryState ? cb.IsOutBackward : cb.IsOutForward;
            }
            else
            {
                throw new InvalidOperationException($"Cylinder '{cylinder.Name}' (Id: {cylinder.Id}) has type '{cylinder.CylinderType}', but expected '{normalType}' or '{reverseType}'.");
            }
        }

        // ========================================================================
        // PUBLIC EXTENSIONS
        // ========================================================================

        // --- 1. Open / Close ---
        public static void Open(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, true);

        public static bool IsOpen(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, true);

        public static void Close(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, false);

        public static bool IsClose(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, false);

        public static bool IsOpenOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, true);

        public static bool IsCloseOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.OpenClose, ECylinderType.OpenCloseReverse, false);


        // --- 2. Up / Down ---
        public static void Up(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, true);

        public static bool IsUp(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, true);

        public static void Down(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, false);

        public static bool IsDown(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, false);

        public static bool IsUpOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, true);

        public static bool IsDownOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.UpDown, ECylinderType.UpDownReverse, false);


        // --- 3. Right / Left ---
        public static void Right(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, true);

        public static bool IsRight(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, true);

        public static void Left(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, false);

        public static bool IsLeft(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, false);

        public static bool IsRightOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, true);

        public static bool IsLeftOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.RightLeft, ECylinderType.RightLeftReverse, false);


        // --- 4. Grip / Ungrip ---
        public static void Grip(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, true);

        public static bool IsGrip(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, true);

        public static void Ungrip(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, false);

        public static bool IsUngrip(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, false);

        public static bool IsGripOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, true);

        public static bool IsUngripOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.GripUngrip, ECylinderType.GripUngripReverse, false);


        // --- 5. Align / Unalign ---
        public static void Align(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, true);

        public static bool IsAlign(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, true);

        public static void Unalign(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, false);

        public static bool IsUnalign(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, false);

        public static bool IsAlignOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, true);

        public static bool IsUnalignOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.AlignUnalign, ECylinderType.AlignUnalignReverse, false);


        // --- 6. Lock / Unlock ---
        public static void Lock(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, true);

        public static bool IsLock(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, true);

        public static void Unlock(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, false);

        public static bool IsUnlock(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, false);

        public static bool IsLockOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, true);

        public static bool IsUnlockOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.LockUnlock, ECylinderType.LockUnlockReverse, false);


        // --- 7. Flip / Unflip ---
        public static void Flip(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, true);

        public static bool IsFlip(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, true);

        public static void Unflip(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, false);

        public static bool IsUnflip(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, false);

        public static bool IsFlipOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, true);

        public static bool IsUnflipOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.FlipUnflip, ECylinderType.FlipUnflipReverse, false);


        // --- 8. Clamp / Unclamp ---
        public static void Clamp(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, true);

        public static bool IsClamp(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, true);

        public static void Unclamp(this ICylinder cylinder) =>
            ExecuteAction(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, false);

        public static bool IsUnclamp(this ICylinder cylinder) =>
            CheckStatus(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, false);

        public static bool IsClampOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, true);

        public static bool IsUnclampOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.ClampUnclamp, ECylinderType.ClampUnclampReverse, false);


        // --- 9. Forward / Backward (raw) ---
        public static bool IsForwardOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.ForwardBackward, ECylinderType.ForwardBackwardReverse, true);

        public static bool IsBackwardOutput(this ICylinder cylinder) =>
            CheckOutputStatus(cylinder, ECylinderType.ForwardBackward, ECylinderType.ForwardBackwardReverse, false);
    }
}
