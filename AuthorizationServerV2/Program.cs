using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthorizationServer.Services;
using Microsoft.OpenApi.Models;
using AuthorizationServer.Data;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.Cookies;

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

            // Example of how to implement a Policy (be sure claim exists!!!!!!)
            //builder.Services.AddAuthorization(option =>
            //{
            //    option.AddPolicy("SiteAdmin", policy => policy.RequireClaim("Site Administrator"));
            //});

            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = true;
                options.SignIn.RequireConfirmedEmail = true;
                options.User.RequireUniqueEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 3;
            })
            //.AddDefaultUI() //--removed since we scaffolded the Pages for register/login, etc. under Areas
            .AddSignInManager()
            .AddRoles<ApplicationRole>()
            .AddDefaultTokenProviders()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddClaimsPrincipalFactory<ApplicationUserClaimsPrincipalFactory>();

            builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>>();

            builder.Services.AddDistributedMemoryCache();

            //??
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.Cookie.Name = "YourAppCookieName";
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.LoginPath = "/Identity/Account/Login";
                // ReturnUrlParameter requires 
                //using Microsoft.AspNetCore.Authentication.Cookies;
                options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
                options.SlidingExpiration = true;
            });

            builder.Services.ConfigureExternalCookie(options => {
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = (Microsoft.AspNetCore.Http.SameSiteMode.Unspecified);
            });
            //??

            //-----------------------------------
            // Customize Authentication to allow BOTH Cookie and JWT Bearer Tokens
            // --https://weblog.west-wind.com/posts/2022/Mar/29/Combining-Bearer-Token-and-Cookie-Auth-in-ASPNET
            //-----------------------------------
            builder.Services.AddAuthentication(options =>
            {
                // custom scheme defined in .AddPolicyScheme() below
                options.DefaultScheme = "JWT_OR_COOKIE";
                options.DefaultChallengeScheme = "JWT_OR_COOKIE";
                options.DefaultSignInScheme = "JWT_OR_COOKIE";
                options.DefaultSignOutScheme = "JWT_OR_COOKIE";
            })
            .AddCookie("Cookies", options =>
            {
                options.LoginPath = "/login";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            })
            .AddCookie(IdentityConstants.ExternalScheme)
            .AddCookie(IdentityConstants.ApplicationScheme)
            .AddCookie(IdentityConstants.TwoFactorUserIdScheme)
            .AddJwtBearer("Bearer", options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuerSigningKey"]),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:IssuerSigningKey"])),
                    ValidateIssuer = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuer"]),
                    ValidAudience = builder.Configuration["JWTSettings:ValidAudience"],
                    ValidIssuer = builder.Configuration["JWTSettings:ValidIssuer"],
                    ValidateAudience = bool.Parse(builder.Configuration["JWTSettings:ValidateAudience"]),
                    RequireExpirationTime = bool.Parse(builder.Configuration["JWTSettings:RequireExpirationTime"]),
                    ValidateLifetime = bool.Parse(builder.Configuration["JWTSettings:ValidateLifetime"])
                };
            })
            // this is the key piece!
            .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
            {
                // runs on each request
                options.ForwardDefaultSelector = context =>
                {
                    // filter by auth type
                    string authorization = context.Request.Headers[HeaderNames.Authorization];
                    if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
                        return "Bearer";

                    // otherwise always check for cookie auth
                    return "Cookies";
                };
            });
            //-----------------------------------

            //builder.Services.AddSession(options =>
            //{
            //    options.IdleTimeout = TimeSpan.FromMinutes(2);
            //    options.Cookie.HttpOnly = true;
            //    options.Cookie.SameSite = SameSiteMode.None;
            //    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            //});

            //builder.Services.AddAuthentication(o =>
            //{
            //    o.DefaultScheme = IdentityConstants.ApplicationScheme;
            //    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            //})
            //.AddIdentityCookies(o => { });

            //// Configure JWT authentication
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            //.AddJwtBearer(options =>
            //{
            //    options.SaveToken = true;
            //    options.RequireHttpsMetadata = false;
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateIssuerSigningKey = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuerSigningKey"]),
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:IssuerSigningKey"])),
            //        ValidateIssuer = bool.Parse(builder.Configuration["JWTSettings:ValidateIssuer"]),
            //        ValidAudience = builder.Configuration["JWTSettings:ValidAudience"],
            //        ValidIssuer = builder.Configuration["JWTSettings:ValidIssuer"],
            //        ValidateAudience = bool.Parse(builder.Configuration["JWTSettings:ValidateAudience"]),
            //        RequireExpirationTime = bool.Parse(builder.Configuration["JWTSettings:RequireExpirationTime"]),
            //        ValidateLifetime = bool.Parse(builder.Configuration["JWTSettings:ValidateLifetime"])
            //    };
            //});

            //--------------------------------
            // Register the Swagger generator, defining 1 or more Swagger documents
            builder.Services.AddSwaggerGen(swagger =>
            {
                swagger.SwaggerDoc("v1", new OpenApiInfo { Title = "JWT Token Auth API", Version = "v1" });
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer'[space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\""
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

            var app = builder.Build();

            //Initialise and seed the database
            await app.InitialiseDatabaseAsync();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            app.UseSwagger(options =>
            {
                //options.SerializeAsV2 = true; //uncomment if we cannot use OpenApi V3
            });

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                //options.RoutePrefix = string.Empty;
            });

            //app.UseAuthentication();
            //app.UseAuthorization();
            //app.UseSession();

            app.MapDefaultControllerRoute();
            app.MapControllers();
            app.MapFallbackToFile("index.html");

            app.UseExceptionHandler(options => { });

            //app.MapEndpoints();

            //app.UseCors(policy => policy
            //        .WithOrigins("http://localhost:5000", "https://localhost:5001")
            //        .AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());




            app.MapRazorPages();

            app.Run();
        }
    }
}