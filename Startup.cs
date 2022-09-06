using CCAP.Api.DataAcess;
using CCAP.Api.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using CCAP.Api.Filters;
using CCAP.Api.Utils;
using Microsoft.OpenApi.Models;
using System;

namespace CCAP.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => {
                options.Filters.Add(new GeneralExceptionHandlerFilter());
            });

            services.AddCors(options => {
                options.AddPolicy(StaticProvider.FrontendCorsPolicy, builder => {
                    builder.WithOrigins(
                            _configuration.GetSection(StaticProvider.AllowedOrigins).Get<string[]>()
                            )
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo
                {

                    Version = "v1",
                    Title = "Credit Card Application Processing",
                    Description = "Processing the credit card applications for the bank",
                    Contact = new OpenApiContact
                    {
                        Name = "Prachi Poddar",
                        Email = "prachi99poddar@gmail.com"                        
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT Licence",
                        Url = new Uri("http://domain.com/license"),
                    }
                });
            });

            services.AddDbContext<CCAPContext>(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("SqlServer"));
            });

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICreditCardApplicationService, CreditCardApplicationService>();
            services.AddScoped<ICreditCardService, CreditCardService>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.Default.GetBytes(_configuration.GetSection("AuthConfig:ServerSecret").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            services.AddAuthorization(options => {
                options.AddPolicy(StaticProvider.UserPolicy, policy => policy.RequireClaim("Roles", "user"));
                options.AddPolicy(StaticProvider.ApproverPolicy, policy => policy.RequireClaim("Roles", "approver"));
                options.AddPolicy(StaticProvider.IssuerPolicy, policy => policy.RequireClaim("Roles", "issuer"));
                options.AddPolicy(StaticProvider.AdminPolicy, policy => policy.RequireClaim("Roles", "admin"));
                options.AddPolicy(StaticProvider.StaffPolicy, policy => policy.RequireClaim("Roles", new string[] { "approver", "issuer" }));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseCors(StaticProvider.FrontendCorsPolicy);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Swagger Demo App");
            });

            
            app.UseAuthentication();
            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
