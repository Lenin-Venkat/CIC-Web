using CICLatest.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Azure.Storage.Queues;
using Azure.Storage.Blobs;
using Azure.Core.Extensions;
using CICLatest.Contracts;
using CICLatest.Helper;

namespace CICLatest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("sqlConnection")));
            //services.AddIdentity<UserModel, IdentityRole>().AddEntityFrameworkStores<ApplicationContext>();
            services.AddControllersWithViews();
            services.AddIdentity<UserModel, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false; 
                opt.User.RequireUniqueEmail = true;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
                opt.Lockout.MaxFailedAccessAttempts = 3;
            }).AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
            services.AddMemoryCache();
            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["StorageConnectionString:blob"], preferMsi: true);
                builder.AddQueueServiceClient(Configuration["StorageConnectionString:queue"], preferMsi: true);
            });
            services.AddSession();
            services.AddHttpContextAccessor();
            var AzureConfig = Configuration
                .GetSection("AzureStorageConfiguration")
                .Get<AzureStorageConfiguration>();
            services.AddSingleton(AzureConfig);
            var emailConfig = Configuration
               .GetSection("EmailConfiguration")
               .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
            services.AddScoped<IAppSettingsReader, AppSettingsReader>();
            services.AddScoped<IBlobStorageService, BlobStorageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
    internal static class StartupExtensions
    {
        public static IAzureClientBuilder<BlobServiceClient, BlobClientOptions> AddBlobServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddBlobServiceClient(serviceUri);
            }
            else
            {
                return builder.AddBlobServiceClient(serviceUriOrConnectionString);
            }
        }
        public static IAzureClientBuilder<QueueServiceClient, QueueClientOptions> AddQueueServiceClient(this AzureClientFactoryBuilder builder, string serviceUriOrConnectionString, bool preferMsi)
        {
            if (preferMsi && Uri.TryCreate(serviceUriOrConnectionString, UriKind.Absolute, out Uri serviceUri))
            {
                return builder.AddQueueServiceClient(serviceUri);
            }
            else
            {
                return builder.AddQueueServiceClient(serviceUriOrConnectionString);
            }
        }
    }
}
