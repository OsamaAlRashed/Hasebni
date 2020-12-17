using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hasebni.Main.Data.Repository;
using Hasebni.Main.Idata.Interfaces;
using Hasebni.Model.Security;
using Hasebni.Security.Data.Repositories;
using Hasebni.Security.Idata.Interfaces;
using Hasebni.SharedKernal.ExtensionMethod;
using Hasebni.SqlServer.DataBase;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;

namespace Hasebni.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        //private readonly string  
        
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var credential_path = "C:/Users/Osama Al-Rashed/source/repos/Hasebni/Hasebni.API/hasebni-f2a6d-firebase-adminsdk-setsh-4cc9a2de0f.json";
            System.Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", credential_path);
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
            });
            services.AddOpenAPI();

            services.AddSignalR();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });

            services.AddControllers();

            services.AddDbContext<HasebniDbContext>(options =>
            {
               options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")); //NewServerConnection-ServerConnection-LocalConnection
            });

            services.AddScoped<IAPIRepository, APIRepository>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IPurchaseRepository, PurchaseRepository>();
            services.AddScoped <INotificationRepository, NotificationRepository>();

            services.AddIdentity<HUser, HRole>()
            .AddEntityFrameworkStores<HasebniDbContext>()
            .AddDefaultTokenProviders();
            
            services.Configure<IdentityOptions>(identity =>
            {
                identity.Password.RequiredLength = 4;
                identity.Password.RequireNonAlphanumeric = false;
                identity.Password.RequireLowercase = false;
                identity.Password.RequireUppercase = false;
                identity.Password.RequireDigit = false; 
                identity.SignIn.RequireConfirmedEmail = true;
            });

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
                opt.TokenLifespan = TimeSpan.FromHours(2));

            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true, 
                    
                    ValidIssuer = Configuration["JwtOptions:Issuer"],
                    ValidAudience = Configuration["JwtOptions:Audience"],
                    //RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(Configuration["JwtOptions:Key"]))
                };
            });

            services.AddRazorPages();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.ConfigureOpenAPI();
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();


            app.UseCors("CorsPolicy");
            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapHub<TestSignalR>("/testhub");
           // https://localhost:5001/testhub
                // endpoints.MapControllerRoute(
                //   name: "default",
                // pattern: "{controller=WeatherForecast}/{action=Get}");

            });
        }
    }
}
