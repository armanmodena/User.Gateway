using Flurl.Http.Configuration;
using Microsoft.Extensions.DependencyInjection;
using User.Gateway.Services;
using User.Gateway.Services.Interfaces;

namespace User.Gateway.Extensions
{
    public static class ServiceManager
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            services.AddSingleton<IFlurlClientFactory, PerBaseUrlFlurlClientFactory>();
            services.AddTransient<IFLService, FLService>();
            services.AddTransient<IAuthService, Services.V2.AuthService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IUserTokenService, UserTokenService>();
            services.AddTransient<IFileUploadService, FileUploadService>();
            services.AddTransient<IExportFileService, ExportFileService>();

            return services;
        }
    }
}
