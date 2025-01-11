using Microsoft.AspNetCore.Authentication.JwtBearer;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = builder.Configuration["Auth0:Authority"]!;
    options.Audience = builder.Configuration["Auth0:Audience"]!;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Auth0:Authority"]!
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("allow-cors",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:7249")
                                .AllowAnyHeader();
                      });
});

builder.Services.AddReverseProxy()
                .LoadFromConfig(builder.Configuration.GetSection("ApiGateway"));

WebApplication app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("allow-cors");

app.UseAuthentication();

app.UseAuthorization();

app.MapReverseProxy();

app.Run();