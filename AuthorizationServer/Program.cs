using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthorizationServer.Services;
using Microsoft.OpenApi.Models;
using AuthorizationServer.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AuthorizationServer.Infrastructure;

namespace AuthorizationServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            // Configure the context to use SQL Database store.
            var connectionString = builder.Configuration
                .GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(connectionString)
            );

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            //.AddSignInManager()
            .AddApiEndpoints() //<<---- .NET 8
            .AddRoles<ApplicationRole>()
            //.AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            ;

            //builder.Services.AddIdentityCore<ApplicationUser>(options =>
            //{
            //    options.SignIn.RequireConfirmedAccount = true;
            //    options.Stores.MaxLengthForKeys = 128;
            //    options.SignIn.RequireConfirmedEmail = true;
            //    options.User.RequireUniqueEmail = true;
            //    options.Lockout.MaxFailedAccessAttempts = 3;
            //})
            ////.AddDefaultUI() //--removed since we scaffolded the Pages for register/login, etc. under Areas
            //.AddRoles<ApplicationRole>()
            //.AddApiEndpoints() //<<---- .NET 8
            //.AddDefaultTokenProviders()
            //.AddEntityFrameworkStores<ApplicationDbContext>()
            ////.AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>()
            //;
            //builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();

            builder.Services.AddAuthentication()
                .AddBearerToken(IdentityConstants.BearerScheme);

            builder.Services.AddAuthorizationBuilder()
              .AddPolicy("api", policy =>
              {
                  policy.RequireAuthenticatedUser();
                  policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
              });

            // Policies that DON'T required Authentication Scheme = "IdentityConstants.BearerScheme" won't work!
            //builder.Services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("manager", policy => policy.RequireRole("manager"));
            //    options.AddPolicy("operator", policy => policy.RequireRole("operator"));
            //});

            // Hardcode policies
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy("SuperAdminOnly", policy =>
                {
                    policy.AddAuthenticationSchemes(IdentityConstants.BearerScheme);
                    policy.RequireRole("SuperAdmin");
                });

            //builder.Services.AddDistributedMemoryCache();
            //services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(2);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SameSite = SameSiteMode.None;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //});


            //-------------------------------------------------------------------------------
            // Register the Swagger generator, defining one or more Swagger documents
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Token Auth API", Version = "v1" });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter ONLY the Login token in the text input below.\r\n\r\nExample: \"12345abcdef\""
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            builder.Services.AddTransient<IEmailSenderUI, EmailSenderUI>();

            builder.Services.ConfigureApplicationCookie(o =>
            {
                o.ExpireTimeSpan = TimeSpan.FromDays(5);
                o.SlidingExpiration = true;
            });

            builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
                o.TokenLifespan = TimeSpan.FromHours(3));

            //Seed Database records for MVC
            builder.Services.AddScoped<ApplicationDbContextInitializer>();

            builder.Services.AddHealthChecks()
                .AddDbContextCheck<ApplicationDbContext>();

            builder.Services.AddExceptionHandler<CustomExceptionHandler>();

            // Customize default API behaviour ???
            builder.Services.Configure<ApiBehaviorOptions>(options =>
                options.SuppressModelStateInvalidFilter = true);

            builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            }));

            var app = builder.Build();

            //Initialise and seed the database
            try
            {
                await Task.Delay(5); //only used so I can comment out below when needed ... after initial deployment
                //await app.InitialiseDatabaseAsync();
            }
            catch (Exception)
            {
                //logger.LogError(ex, "An error occurred during database initialization.");
                throw;
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                //app.UseWebAssemblyDebugging(); // for Blazor apps?
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHealthChecks("/health");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            //app.UseRouting();
            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //options.RoutePrefix = "swagger"; //default
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapDefaultControllerRoute();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.UseExceptionHandler(options => { });

            app.MapRazorPages();

            // Test using policy authorization (only works with MapGroup!!!)
            app.MapGet("v1/api/foo", () =>
            {
                return new[] { "One", "Two", "Three" };
            })
            .RequireAuthorization("api"); //policy defined above

            app.MapGroup("api")
              .MapIdentityApi<ApplicationUser>();

            // Apply the AdminOnly policy to this endpoint using 'decorator pattern' ... works in Controllers the same way!
            app.MapGet("SuperAdminOnly", [Authorize(Policy = "SuperAdminOnly")] () => "Hello Super Admin!").RequireAuthorization();

            app.Run();
        }
    }
}