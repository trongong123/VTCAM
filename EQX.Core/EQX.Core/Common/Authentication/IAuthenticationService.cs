namespace EQX.Core.Common
{
    public interface IAuthenticationService
    {
        /// <summary>
        /// Validate the provided credential for the requested permission.
        /// Implementations encapsulate password rules (e.g., dynamic SuperUser code).
        /// </summary>
        /// <returns>true when credential is valid; otherwise false.</returns>
        bool ValidatePermission(EPermission permission, string password);
    }
}
