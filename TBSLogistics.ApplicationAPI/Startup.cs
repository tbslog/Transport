using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Text;
using TBSLogistics.Data.TBSLogisticsDbContext;
using TBSLogistics.Data.TMS;
using TBSLogistics.Service.Repository.AddressManage;
using TBSLogistics.Service.Repository.Authenticate;
using TBSLogistics.Service.Repository.BillOfLadingManage;
using TBSLogistics.Service.Repository.Common;
using TBSLogistics.Service.Repository.CustommerManage;
using TBSLogistics.Service.Repository.DriverManage;
using TBSLogistics.Service.Repository.PricelistManage;
using TBSLogistics.Service.Repository.SupplierManage;
using TBSLogistics.Service.Repository.VehicleManage;

namespace TBSLogistics.ApplicationAPI
{
    public class Startup
    {
        readonly string apiCorsPolicy = "ApiCorsPolicy";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: apiCorsPolicy,
                builder =>
                {
                    //builder.WithOrigins("file:///C:/Users/ad/Desktop/test.html")
                    builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    //.AllowCredentials();
                    //.WithMethods("OPTIONS", "GET");
                });
            });

            services.AddDbContext<TBSTuyenDungContext>(options =>
            options.UseSqlServer(Configuration["SQLCnn"]));
            services.AddDbContext<TMSContext>(options =>
            options.UseSqlServer(Configuration["DBVanTai"]));

            services.AddControllersWithViews();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
            {
                option.RequireHttpsMetadata = false;
                option.SaveToken = true;
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICommon, CommonService>();
            services.AddTransient<IAuthenticate, AuthenticateService>();
            services.AddTransient<IAddress, AddressService>();
            services.AddTransient<ICustomer, CustomerService>();
            services.AddTransient<IPriceList, PriceListService>();
            services.AddTransient<ISupplier, SupplierService>();
            services.AddTransient<IDriver, DriverService>();
            services.AddTransient<IVehicle, VehicleService>();
            services.AddTransient<IBillOfLading, BillOfLadingService>();

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "TBSLogistics.ApplicationAPI", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TBSLogistics.ApplicationAPI v1"));
            }

            app.UseHttpsRedirection();
            app.UseCors(apiCorsPolicy);
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}