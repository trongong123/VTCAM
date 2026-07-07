namespace EQX.Core.Common
{
    public interface IUserStore
    {
        event Action? UserChanged;
        EPermission Permission { get; internal set; }
    }

    public class UserStore : IUserStore
    {
        public event Action? UserChanged;

        public EPermission Permission
        {
            get => _permission;
            set
            {
                _permission = value;
                UserChanged?.Invoke();
            }
        }

        #region Privates
        private EPermission _permission = EPermission.Operator;
        #endregion
    }
}
