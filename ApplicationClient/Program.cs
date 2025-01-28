using ApplicationClient;
using ApplicationClient.AuthConfig;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Shared.Auth;
using Shared.Communication;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new AuthorizationMessageHandler(sp.GetRequiredService<IAccessTokenProvider>(),
                                                                 sp.GetRequiredService<NavigationManager>())
                .ConfigureHandler([builder.Configuration["ApiGateway:Address"]!]));

builder.Services.AddHttpClient(HttpClientConstants.ApiGateway, client => client.BaseAddress = new Uri(builder.Configuration["ApiGateway:Address"]!))
                .AddHttpMessageHandler<AuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(HttpClientConstants.ApiGateway));

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);

    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);

    options.ProviderOptions.ResponseType = "code";

    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.DefaultScopes.Add("offline_access");
}).AddAccountClaimsPrincipalFactory<AuthorizedUserFactory>();

builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy(PolicyConstants.GetUser, policy => policy.RequireClaim(ClaimConstants.Permissions, PermissionConstants.ReadUser));
    config.AddPolicy(PolicyConstants.GetWeather, policy => policy.RequireClaim(ClaimConstants.Permissions, PermissionConstants.ReadWeather));
});

await builder.Build().RunAsync();