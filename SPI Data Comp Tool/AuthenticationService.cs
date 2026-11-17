using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Identity.Client;

namespace SPI_Data_Comp_Tool
{
    /// <summary>
    /// Desktop sign-in choices. A successful sign-in only unlocks this application's UI;
    /// every database/API still needs to enforce its own independent authorization.
    /// </summary>
    internal static class AuthenticationService
    {
        private static readonly string[] EntraScopes = { "User.Read" };

        internal static SignInResult SignInWithWindows()
        {
            try
            {
                var allowedUsers = ReadWindowsSids("Auth.Windows.AllowedUserSids");
                var allowedGroups = ReadWindowsSids("Auth.Windows.AllowedGroupSids");
                if (allowedUsers.Count == 0 && allowedGroups.Count == 0)
                    return SignInResult.Denied("Windows sign-in is not configured. Contact your administrator.");

                using (var identity = WindowsIdentity.GetCurrent())
                {
                    if (identity == null || !identity.IsAuthenticated || identity.User == null)
                        return SignInResult.Denied("Windows could not verify the current account.");

                    var principal = new WindowsPrincipal(identity);
                    if (allowedUsers.Any(sid => sid.Equals(identity.User)) ||
                        allowedGroups.Any(principal.IsInRole))
                        return SignInResult.Granted("Windows / Active Directory");
                }

                return SignInResult.Denied("This Windows account is not authorized to use the application.");
            }
            catch (ArgumentException)
            {
                return SignInResult.Denied("The Windows identity configuration is invalid. Contact your administrator.");
            }
            catch (System.Security.SecurityException)
            {
                return SignInResult.Denied("Windows could not verify the current account.");
            }
        }

        internal static async Task<SignInResult> SignInWithEntraAsync(IWin32Window owner, CancellationToken cancellationToken)
        {
            Guid tenantId;
            Guid clientId;
            if (!Guid.TryParse(Setting("Auth.Entra.TenantId"), out tenantId) || tenantId == Guid.Empty ||
                !Guid.TryParse(Setting("Auth.Entra.ClientId"), out clientId) || clientId == Guid.Empty)
                return SignInResult.Denied("Microsoft Entra ID is not configured. Contact your administrator.");

            HashSet<Guid> allowedUsers;
            if (!TryReadGuids("Auth.Entra.AllowedUserObjectIds", out allowedUsers) || allowedUsers.Count == 0)
                return SignInResult.Denied("The Entra ID user allowlist is missing or invalid. Contact your administrator.");

            // System browser + PKCE; no client secret or credentials are stored by this app.
            // The Entra app registration must allow the http://localhost desktop redirect URI.
            var client = PublicClientApplicationBuilder.Create(clientId.ToString("D"))
                .WithAuthority(AzureCloudInstance.AzurePublic, tenantId.ToString("D"))
                .WithRedirectUri("http://localhost")
                .Build();

            var result = await client.AcquireTokenInteractive(EntraScopes)
                .WithParentActivityOrWindow(owner)
                .WithUseEmbeddedWebView(false)
                .ExecuteAsync(cancellationToken);

            Guid actualTenantId;
            Guid actualObjectId;
            if (result == null || !Guid.TryParse(result.TenantId, out actualTenantId) ||
                actualTenantId != tenantId || result.Account == null || result.Account.HomeAccountId == null ||
                !Guid.TryParse(result.Account.HomeAccountId.ObjectId, out actualObjectId) ||
                !allowedUsers.Contains(actualObjectId))
                return SignInResult.Denied("This Entra ID account is not authorized to use the application.");

            return SignInResult.Granted("Microsoft Entra ID");
        }

        private static List<SecurityIdentifier> ReadWindowsSids(string setting)
        {
            return Split(Setting(setting)).Select(value => new SecurityIdentifier(value)).ToList();
        }

        private static bool TryReadGuids(string setting, out HashSet<Guid> values)
        {
            values = new HashSet<Guid>();
            foreach (var raw in Split(Setting(setting)))
            {
                Guid parsed;
                if (!Guid.TryParse(raw, out parsed) || parsed == Guid.Empty)
                    return false;
                values.Add(parsed);
            }
            return true;
        }

        private static IEnumerable<string> Split(string list)
        {
            return (list ?? string.Empty)
                .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(value => value.Trim())
                .Where(value => value.Length > 0);
        }

        private static string Setting(string name)
        {
            return ConfigurationManager.AppSettings[name] ?? string.Empty;
        }
    }

    internal sealed class SignInResult
    {
        private SignInResult(bool authorized, string message)
        {
            Authorized = authorized;
            Message = message;
        }

        internal bool Authorized { get; private set; }
        internal string Message { get; private set; }

        internal static SignInResult Granted(string method)
        {
            return new SignInResult(true, "Signed in with " + method + ".");
        }

        internal static SignInResult Denied(string message)
        {
            return new SignInResult(false, message);
        }
    }
}
