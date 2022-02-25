using System;
using AutoMapper;
using System.Linq;
using System.Text;
using RestaurantPOS.Models;
using RestaurantPOS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.AspNetCore.StaticFiles;
using RestaurantPOS.Repository.Mappings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using RestaurantPOS.SignalR;
namespace RestaurantPOS
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
            
            services.AddControllers();
            services.AddDbContext<RestaurantPosDBContext>(opt => opt.UseSqlServer(Configuration["ConnectionString:RestaurantDB"]));
            services.AddDbContext<RestaurantLivePosDBContext>(opt => opt.UseSqlServer(Configuration["ConnectionString:RestaurantDBLive"]));
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                List<SymmetricSecurityKey> symmetricSecurityKeys = new List<SymmetricSecurityKey>
                {
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:key"]))
                };
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKeys = symmetricSecurityKeys,
                    ValidIssuer = Configuration["JWT:Issuer"],
                    ValidAudience = Configuration["JWT:Audience"],
                    RequireExpirationTime = true,
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
            services.AddControllers().AddNewtonsoftJson(options =>
                 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );
            //Automapper
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AllowNullCollections = true;
                cfg.AddProfile(new AdminMappingProfile());
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            #region CORS
            services.AddCors();
            //services.AddCors(c =>
            //{
            //    c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            //});
            //services.AddCors(options =>
            //{
            //    //options.AddPolicy("AllowOrigin", p => p.WithOrigins(Configuration["CORSUrl:Url"])
            //    //        .AllowAnyMethod()
            //    //        .AllowAnyHeader()
            //    //        .AllowCredentials());
            //    //options.AddPolicy("AllowOrigin", p => p.WithOrigins("http://192.168.0.117:91", "http://localhost:5200", "http://localhost:4200", "http://localhost:4400", "http://192.168.0.123:3400", "http://192.168.0.123:4300", "http://localhost:4300","http://localost:3400", "http://192.168.0.119:4200", "http://192.168.0.105:3400","http://192.168.0.137:3400", "https://localhost:44391", "http://localhost:55358", "http://192.168.0.105:82", "http://thepalmgrill.builtinsoft.com")
            //    //                         .AllowAnyMethod()
            //    //                         .AllowAnyHeader()
            //    //                         .AllowCredentials());
            //});
            #endregion
            services.AddMemoryCache();
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true; // consent required
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true;
                options.IdleTimeout = TimeSpan.FromSeconds(60);
            });
            services.AddMvc(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("RestaurantPOS", new OpenApiInfo
                {
                    Version = "RestaurantPOS",
                    Title = "RestaurantPOS Admin Api Architecture",
                    Description = "RestaurantPOS Admin API",
                    TermsOfService = new Uri("https://restaurant.builtinsoft.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "RestaurantPOS",
                        Email = string.Empty,
                        Url = new Uri("http://restaurant.builtinsoft.com/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Use under LICX",
                        Url = new Uri("https://restaurant.builtinsoft.com/license"),
                    }
                });
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"bearer {token}\"",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.EnableAnnotations();
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First()); //This line
                c.SchemaFilter<SwaggerIgnoreFilter>();
            });
            //services.AddSwaggerGenNewtonsoftSupport();
            DependencyInjectionScopesHelper.Update(services);
            services.AddSignalR();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();
            app.UseSession();
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/RestaurantPOS/swagger.json", "RestaurantPOS");
                c.RoutePrefix = string.Empty;
                c.DisplayRequestDuration();
                c.DefaultModelRendering(ModelRendering.Model);
                c.DefaultModelsExpandDepth(1);
                c.DisplayOperationId();
                c.DisplayRequestDuration();
                c.EnableFilter();
                c.ShowExtensions();
                c.EnableValidator();
                c.DocExpansion(DocExpansion.None);
            });
            app.UseRouting();
            //app.UseCors("AllowOrigin");
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
            app.UseAuthorization();
           
            app.UseStaticFiles();
            var options = new StaticFileOptions
            {
                ContentTypeProvider = new FileExtensionContentTypeProvider()
            };
            app.UseStaticFiles(options);
            app.UseDefaultFiles();
            var cookiePolicyOptions = new CookiePolicyOptions
            {
                Secure = CookieSecurePolicy.SameAsRequest,
                MinimumSameSitePolicy = SameSiteMode.None
            };
            app.UseCookiePolicy(cookiePolicyOptions);
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<OrderNotificationHub>("/ordernotificationhub");
            });
            //app.UseMvc();
        }
    }
}
