using DotNet8Authentication.Data;
using DotNet8Authentication.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
//builder.Services.AddControllers();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
         policy => policy.RequireRole("Administrator"));
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

//builder.Services.AddIdentityCore<ApplicationUser>()
//    .AddRoles<ApplicationRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//builder.Services.AddDefaultIdentity<ApplicationUser>(
//    options => options.SignIn.RequireConfirmedAccount = false
//    )
//    .AddRoles<ApplicationRole>()
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization();

builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
//    .AddRoles<ApplicationRole>
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.MapIdentityApi<ApplicationUser>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapRazorPages();

//--https://www.youtube.com/watch?v=Y6DCP-yH-9Q
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
    var roles = new[] { "Administrator", "Operations", "Accounts", "Users" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new ApplicationRole(role)); 
    }
};

using (var scope = app.Services.CreateScope())
{
    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    string email = "administrator@localhost";
    string password = "Administrator1!";

    if (await userManager.FindByEmailAsync(email) is null)
    {
        var user = new ApplicationUser();
        user.Email = email;
        user.UserName = email;
        await userManager.CreateAsync(user, password);

        // Add User to Role
        await userManager.AddToRoleAsync(user, "Administrator");
    };
};

app.Run();
