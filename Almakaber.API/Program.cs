
using Almakaber.API.Extensions;
using Almakaber.API.Middlewares;
using Almakaber.BLL.Helpers;
using Almakaber.BLL.Mapping;
using Almakaber.BLL.Services.Implementations;
using Almakaber.BLL.Services.Interfaces;
using Almakaber.BLL.Validators;
using Almakaber.DAL.Context;
using Almakaber.DAL.Entities;
using Almakaber.DAL.Repositories.Implementations;
using Almakaber.DAL.Repositories.Interfaces;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

namespace Almakaber.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((context, configuration) =>
                     configuration
                    .MinimumLevel.Information() 
                    .WriteTo.Console() 
                    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) 
);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<AlmakaberDbContext>(options =>
                options.UseSqlServer(connectionString));


            builder.Services.AddHangfire(configuration => configuration
                            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                            .UseSimpleAssemblyNameTypeSerializer()
                            .UseRecommendedSerializerSettings()
                            .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfireServer();

            builder.Services.AddAutoMapper(x => x.AddProfile(new MappingProfile()));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AlmakaberDbContext>()
                .AddDefaultTokenProviders();


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                o.RequireHttpsMetadata = true; 
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                };
            });



            builder.Services.AddApplicationServices();

            builder.Services.AddControllers();

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssemblyContaining<CreateGraveDtoValidator>();
           
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Almakaber API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "أدخل كلمة 'Bearer' متبوعة بمسافة ثم التوكن الخاص بك.\r\n\r\nمثال: 'Bearer 12345abcdef'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()   
                           .AllowAnyMethod()    
                           .AllowAnyHeader();  

                });
            });

            var app = builder.Build();

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/hangfire") && context.Request.Query.ContainsKey("access_token"))
                {
                    context.Request.Headers.Add("Authorization", $"Bearer {context.Request.Query["access_token"]}");
                }
                await next.Invoke();
            });

            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new Helpers.HangfireAuthorizationFilter() }
            });


            RecurringJob.AddOrUpdate<INotificationService>(
                "Friday-Reminder",
                service => service.SendFridayRemindersAsync(),
                "0 9 * * 5"); 

            RecurringJob.AddOrUpdate<INotificationService>(
                "Annual-Deceased-Reminder",
                service => service.SendAnnualRemindersAsync(),
                "0 10 * * *");

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    await IdentitySeed.SeedAdminUserAsync(userManager, roleManager);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "حدث خطأ أثناء عمل Seed لحساب الأدمن.");
                }
            }

            app.UseMiddleware<GlobalExceptionMiddleware>();
            

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseCors("AllowAll");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AlmakaberDbContext>();
                db.Database.Migrate();
            }

            app.Run();
        }
    }
}
