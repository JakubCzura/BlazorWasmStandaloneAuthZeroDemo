using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using Shared.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

namespace ApplicationClient.AuthConfig;

/// <summary>
/// Class to resolve problem with mapping JWT roles and permission to claims for WASM apps. <br/>
/// In the current implementation of the `AccountClaimsPrincipalFactory<TAccount>` class, the roles are serialized as an array in a single claim,
/// so this class is used to deserialize the roles and add them as separate claims to the user.
/// </summary>
/// <param name="accessor">Accessor for the token provider.</param>
public class AuthorizedUserFactory(IAccessTokenProviderAccessor accessor)
    : AccountClaimsPrincipalFactory<RemoteUserAccount>(accessor)
{
    public override async ValueTask<ClaimsPrincipal> CreateUserAsync(RemoteUserAccount account,
                                                                     RemoteAuthenticationUserOptions options)
    {
        ClaimsPrincipal userAccount = await base.CreateUserAsync(account, options);
        ClaimsIdentity userIdentity = (ClaimsIdentity)userAccount.Identity!;

        if (userIdentity.IsAuthenticated)
        {        
            AccessTokenResult getAccessTokenResult = await TokenProvider.RequestAccessToken();
            if (getAccessTokenResult.TryGetToken(out AccessToken? accessToken))
            {
                //Auth0 permissions are not included in userIdentity claims, so I need to add them manually.
                //I fetch the permissions from the token and add them as separate claims to the user with the type "permissions".
                //It makes me available to use them in Blazor components for authorization purposes, for example creating policies based on permissions.
                //For exaple: [Authorize(Policy = "read-all")] can require the "read:user", "read:posts", "read:comments" permissions.
                JwtSecurityToken jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken.Value);
                foreach (Claim claim in jwtToken.Claims.Where(x => x.Type == ClaimConstants.Permissions))
                {
                    userIdentity.AddClaim(new(claim.Type, claim.Value));
                }
            }

            //We need to check if the roles are serialized as an array, that's the point of putting them in a separate claims
            JsonElement? userRoles = account.AdditionalProperties[userIdentity.RoleClaimType] as JsonElement?;
            if (userRoles?.ValueKind == JsonValueKind.Array)
            {
                //Try to delete existing role claim that contains the serialized array of roles
                userIdentity.TryRemoveClaim(userIdentity.Claims.FirstOrDefault(c => c.Type == userIdentity.RoleClaimType));

                //Add each role to user as separate claim
                foreach (JsonElement element in userRoles.Value.EnumerateArray())
                {
                    userIdentity.AddClaim(new(userIdentity.RoleClaimType, element.GetString()!));
                }
            }
        }

        return userAccount;
    }
}