# Authentication setup (Windows desktop)

The start screen offers **two independent methods**. Either one can unlock the desktop menu after successful sign-in:

| Option | Method | Administration required | Connectivity |
| --- | --- | --- | --- |
| A | Windows / Active Directory | Allowlisted Windows account SID or security group SID | Uses the signed-in Windows token; domain policy controls group membership |
| B | Microsoft Entra ID | Single-tenant desktop app registration and allowed user object IDs | Browser and Microsoft identity platform access required |

**Both methods deny access by default.** There is no shared user name, shared password, offline bypass or debug-environment-variable override. Configuring A does not enable B, or vice versa.

## Configure Option A (Windows / Active Directory)

1. Create a dedicated Windows/Active Directory **security group** for authorized operators. Prefer group SIDs over individual-user SIDs for routine administration.
2. Have an administrator retrieve the *SID* (not the display name) of that group. For example, use `Get-ADGroup -Identity "YourGroupName" -Properties SID` from a domain administration workstation, or inspect the member's Windows token with `whoami /groups`.
3. Enter one or more SIDs in `Auth.Windows.AllowedGroupSids` in the deployed application's `.exe.config`, separated by semicolons. For an exceptional individual user, use `Auth.Windows.AllowedUserSids` (find the user's token SID with `whoami /user`). Leave unused settings empty.
4. Have the operator **sign out and back into Windows** after changing group membership so the Windows access token is refreshed. Run the application with the intended standard user account and click **A**.

The application uses `WindowsIdentity.GetCurrent()` and the `WindowsPrincipal.IsInRole(SecurityIdentifier)` token check. It does not ask for or store a domain password. Invalid SIDs, missing allowlists, and nonmember accounts are denied.

## Configure Option B (Microsoft Entra ID)

1. In the Microsoft Entra admin center, register an application with **Accounts in this organizational directory only** (single tenant).
2. Configure the **Mobile and desktop applications** platform redirect URI `http://localhost`. Configure the app as a public client; **do not create or distribute a client secret** for this desktop program.
3. Add/approve delegated Microsoft Graph `User.Read` permission. Use organizational Conditional Access and multifactor authentication policy as appropriate.
4. Enter the **Directory (tenant) ID** GUID in `Auth.Entra.TenantId` and **Application (client) ID** GUID in `Auth.Entra.ClientId`.
5. Enter the authorized **home-tenant user object ID** GUID(s) in `Auth.Entra.AllowedUserObjectIds`, separated by semicolons. This initial allowlist is for home-tenant members; **guest/B2B accounts are not supported** until a tenant-aware membership verification is implemented.
6. Start the app, choose **B**, complete the interactive Microsoft sign-in in the system browser, and return to the app. A sign-in expires after five minutes if it is not completed.

The Microsoft Authentication Library uses a tenant-specific authority, browser redirect + PKCE, and checks the tenant ID and account object ID returned by the library against the administrator's allowlist. Authentication failure or a disallowed identity never unlocks the menu. No token, Microsoft account password, or client secret is written to the app's logs.

## Configuration file

Edit the `.exe.config` **beside the executable actually being launched**, not only the source `App.config`. A packaged build renames the executable and matching config to the same base name.

~~~xml
<appSettings>
  <add key="Auth.Windows.AllowedUserSids" value="" />
  <add key="Auth.Windows.AllowedGroupSids" value="" />
  <add key="Auth.Entra.TenantId" value="" />
  <add key="Auth.Entra.ClientId" value="" />
  <add key="Auth.Entra.AllowedUserObjectIds" value="" />
</appSettings>
~~~

Do not add credentials or access tokens to this config. **The allowlist is deployment configuration, not a cryptographically protected server-side access-control policy.** Distribute the application through managed endpoints with administrator-controlled file permissions and software integrity controls. A person able to edit the config or modify/run a replacement binary can bypass desktop-only controls. The application must continue to use **independently authenticated, least-privilege database accounts**; server-side authorization is required to protect data and SQL operations. A managed API/service is necessary for centrally enforced, immediately revocable authorization.

## Acceptance tests (Windows)

| Scenario | Expected result |
| --- | --- |
| All auth settings blank | Both choices deny; all operation menus stay disabled |
| A with approved SID | Menu unlocks after Windows token membership check |
| A with unapproved SID or malformed SID | Access denied |
| B with approved tenant/client/home user object ID | System-browser sign-in, then menu unlocks |
| B with wrong tenant, wrong user ID, or invalid configuration | Access denied |
| B user cancels or sign-in exceeds five minutes | Access denied; user can retry |
| Release build with debugger attached or `SPI_DATA_COMP_DEBUG=1` | Cannot bypass sign-in; developer diagnostics remain disabled |
| Run SQL or Excel tool after approved sign-in | Existing functionality continues working with its independent data permissions |

The GitHub Actions Windows job validates the build, and repository contract tests validate the fail-closed wiring. **Live Windows/AD and Entra sign-in scenarios need an actual organization/tenant and administrator-supplied allowlists.** No tenant details or access credentials are included in the repository.

References: [Microsoft desktop registration](https://learn.microsoft.com/en-us/entra/identity-platform/scenario-desktop-app-configuration), [MSAL.NET system browser](https://learn.microsoft.com/en-us/entra/msal/dotnet/acquiring-tokens/using-web-browsers), [WindowsPrincipal.IsInRole](https://learn.microsoft.com/en-us/dotnet/api/system.security.principal.windowsprincipal.isinrole).
