using System;
using System.Configuration;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace GSA.Samples.Northwind.OData.Common.Filters
{
    public class KeyAndSecretBasicAuthenticationAttribute : BasicAuthenticationAttribute
    {
        protected virtual string Key => ConfigurationManager.AppSettings["OData.Api.Basic.Authentication.Key"];
        protected virtual string Secret => ConfigurationManager.AppSettings["OData.Api.Basic.Authentication.Secret"];

        protected override async Task<IPrincipal> AuthenticateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            // Ignoring case in comparison because we are comparing key/secret instead of username/password
            if (!string.Equals(userName, Key, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(password, Secret, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested(); // Unfortunately, UserManager doesn't support CancellationTokens.

            // Create a ClaimsIdentity with all the claims for this user.
            cancellationToken.ThrowIfCancellationRequested();

            return await Task.FromResult<IPrincipal>(new GenericPrincipal(new GenericIdentity(userName), new string[0]));
        }
    }
}