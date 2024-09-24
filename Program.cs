using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;



var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });



builder.Services.AddAuthorization();

// Configure services
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Configure DbContext with MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 21))));







builder.Services.AddScoped<IECustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IECustomerService, CustomerService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
await ConfigureAuthorizationPolicies(builder.Services.BuildServiceProvider());


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "JWT Bearer Test API",
        Version = "v1"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\nExample: \"Bearer 12345abcdef\""
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();






// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseMiddleware<CustomUnauthorizedResponseMiddleware>();
app.UseMiddleware<AthorizeRoleMiddleware>();

app.UseRouting();
app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();


app.Run();

async Task ConfigureAuthorizationPolicies(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var roleService = scope.ServiceProvider.GetRequiredService<IRoleService>();
        var permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        var roles = await roleService.GetAllRolesAsync(); // Get all roles
        var rolePermissionMap = new Dictionary<string, List<string>>();

        // Load permissions for each role
        foreach (var role in roles)
        {
            var roleWithPermissions = await permissionService.GetRoleWithPermissionsAsync(role.Id);
            rolePermissionMap[role.Name] = roleWithPermissions.Permissions.Select(p => p.Name).ToList();
        }

        // Register dynamic role policy
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("DynamicRolePolicy", policy =>
            {
                // Allow any user with one of the roles
                policy.RequireAssertion(context =>
                {
                    var userRoles = context.User.Claims
                        .Where(c => c.Type == ClaimTypes.Role)
                        .Select(c => c.Value)
                        .ToList();

                    return userRoles.Any(role => roles.Any(r => r.Name == role));
                });
            });

            foreach (var role in roles)
            {
                options.AddPolicy(role.Name, policy => policy.RequireRole(role.Name));

                if (rolePermissionMap.TryGetValue(role.Name, out var permissions))
                {
                    foreach (var permission in permissions)
                    {
                        options.AddPolicy($"{permission}", policy =>
                        {
                            policy.RequireAssertion(context =>
                                context.User.IsInRole(role.Name) &&
                                context.User.HasClaim("Permission", permission));
                        });
                    }
                }
            }
        });
    }
}
