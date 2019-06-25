using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;

namespace OCREngine.WebApi.Authentication
{
    public class ConfigureAzureOptions : IConfigureNamedOptions<OpenIdConnectOptions>
    {
        private readonly AzureAdOptions _azureOptions;

        public ConfigureAzureOptions(IOptions<AzureAdOptions> azureOptions)
        {
            _azureOptions = azureOptions.Value;
        }

        public void Configure(string name, OpenIdConnectOptions options)
        {
            options.ClientId = _azureOptions.ClientId;
            options.Authority = $"{_azureOptions.Instance}{_azureOptions.TenantId}";
            options.UseTokenLifetime = true;
            options.CallbackPath = _azureOptions.CallbackPath;
            options.RequireHttpsMetadata = false;
        }

        public void Configure(OpenIdConnectOptions options)
        {
            Configure(Options.DefaultName, options);
        }
    }
}