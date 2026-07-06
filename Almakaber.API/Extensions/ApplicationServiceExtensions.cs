using Almakaber.BLL.Helpers;
using Almakaber.BLL.Helpers.Email;
using Almakaber.BLL.Services.Implementations;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.DAL.Repositories.Implementations;
using Almakaber.DAL.Repositories.Interfaces;

namespace Almakaber.API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IGraveService, GraveService>();
            services.AddScoped<IDeceasedService, DeceasedService>();
            services.AddScoped<ISupplicationService, SupplicationService>();
            services.AddScoped<IDeceasedSupplicationService, DeceasedSupplicationService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}