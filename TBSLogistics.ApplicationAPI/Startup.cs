using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TBSLogistics.Data.TMS;
using TBSLogistics.Model.Model.MailSettings;
using TBSLogistics.Service.Panigation;
using TBSLogistics.Service.Services.AccountManager;
using TBSLogistics.Service.Services.AddressManage;
using TBSLogistics.Service.Services.Bill;
using TBSLogistics.Service.Services.BillOfLadingManage;
using TBSLogistics.Service.Services.Common;
using TBSLogistics.Service.Services.ContractManage;
using TBSLogistics.Service.Services.CustommerManage;
using TBSLogistics.Service.Services.DriverManage;
using TBSLogistics.Service.Services.MobileManager;
using TBSLogistics.Service.Services.NotificationManage;
using TBSLogistics.Service.Services.PricelistManage;
using TBSLogistics.Service.Services.PriceTableManage;
using TBSLogistics.Service.Services.ProductServiceManage;
using TBSLogistics.Service.Services.Report;
using TBSLogistics.Service.Services.RoadManage;
using TBSLogistics.Service.Services.RomoocManage;
using TBSLogistics.Service.Services.SFeeByTcommandManage;
using TBSLogistics.Service.Services.SubFeePriceManage;
using TBSLogistics.Service.Services.UserManage;
using TBSLogistics.Service.Services.VehicleManage;

namespace TBSLogistics.ApplicationAPI
{
    public class Startup
    {
        private readonly string apiCorsPolicy = "ApiCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(option =>
            {
                option.AddPolicy(name: apiCorsPolicy, policy =>
                 {
                     policy.WithOrigins("http://localhost:3000", "http://192.168.0.254:9999", "https://tms.tbslogistics.com.vn").AllowAnyMethod().AllowAnyHeader();
                    // policy.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
                 });
            });



            services.AddDbContext<TMSContext>(options => options.UseSqlServer(Configuration["TMS_Cloud"]));
            services.AddHttpContextAccessor();
            services.AddSingleton<IPaginationService>(o =>
            {
                var accessor = o.GetRequiredService<IHttpContextAccessor>();
                var request = accessor.HttpContext.Request;
                var uri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent());
                return new PaginationService(uri);
            });

            services.AddOptions();                                        
            var mailsettings = Configuration.GetSection("MailSettings");  
            services.Configure<MailSettings>(mailsettings);                


            services.AddControllersWithViews();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = Configuration["Jwt:Audience"],
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

            services.AddSingleton(sp => sp.GetRequiredService<ILoggerFactory>().CreateLogger("DefaultLogger"));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ICommon, CommonService>();
            services.AddTransient<IAddress, AddressService>();
            services.AddTransient<ICustomer, CustomerService>();
            services.AddTransient<IPriceTable, PriceTableService>();
            services.AddTransient<IDriver, DriverService>();
            services.AddTransient<IVehicle, VehicleService>();
            services.AddTransient<IRoad, RoadService>();
            services.AddTransient<IContract, ContractService>();
            services.AddTransient<IProduct, ProductService>();
            services.AddTransient<IRomooc, RomoocService>();
            services.AddTransient<IBillOfLading, BillOfLadingService>();
            services.AddTransient<ISubFeePrice, SubFeePriceService>();
            services.AddTransient<INotification, NotificationService>();
            services.AddTransient<IUser, UserService>();
            services.AddTransient<ISFeeByTcommand, SFeeByTcommandService>();
            services.AddTransient<IBill, BillService>();
            services.AddTransient<IReport, ReportService>();
            services.AddTransient<IMobile, MobileServices>();
            services.AddTransient<IAccount, AccountService>();

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
            app.UseRouting();
            app.UseCors(apiCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}