namespace EQX.Core.Common
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService(IUserStore userStore)
        {
            _userStore = userStore;
        }

        public bool ValidatePermission(EPermission permission, string password)
        {
            if (ValidatePassword(permission, password))
            {
                _userStore.Permission = permission;
                return true;
            }

            _userStore.Permission = EPermission.Operator;
            return false;
        }

        /// <summary>
        /// Validates the provided password against the specified permission level.
        /// </summary>
        /// <remarks>
        /// This is a <c>virtual</c> method providing a default implementation with hardcoded credentials (e.g., "operator123").
        /// Derived classes should override this method to implement secure authentication logic (e.g., database verification or password hashing).
        /// </remarks>
        /// <param name="permission">The permission level to authenticate against.</param>
        /// <param name="password">The plain-text password to validate.</param>
        /// <returns>
        /// <c>true</c> if the password matches the permission's requirements; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool ValidatePassword(EPermission permission, string password)
        {
            return permission switch
            {
                EPermission.Operator => true,
                EPermission.Admin => password == "1234",
                EPermission.SuperUser => password == "3141",
                _ => false,
            };
        }

        #region Privates
        private readonly IUserStore _userStore;
        #endregion
    }
}
