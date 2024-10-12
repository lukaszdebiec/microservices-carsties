using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options => 
{
    // The authority where the tokens are issued
    options.Authority = builder.Configuration["IdentityServiceUrl"]; // Ensure this is set correctly
    options.RequireHttpsMetadata = false; // Disable HTTPS requirement for development

    // Token validation parameters
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false, // Disable audience validation
        NameClaimType = "username", // Set the claim type for the username
        ValidIssuer = "http://localhost:5000", // Ensure this matches the 'iss' claim in your token
        ValidateIssuer = true, // Still validate the issuer
        ValidateLifetime = true, // Validate token expiration
        ValidateIssuerSigningKey = false // Disable signature validation
        
    };

    options.TokenValidationParameters.SignatureValidator = (token, _) => new JsonWebToken(token);
});

var app = builder.Build();

app.MapReverseProxy();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
