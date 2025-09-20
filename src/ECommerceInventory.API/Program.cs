using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ECommerceInventory.Application.Services;
using ECommerceInventory.Domain.Repositories;
using ECommerceInventory.Infrastructure.Data;
using ECommerceInventory.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
var postgresConnection = builder.Configuration.GetConnectionString("PostgreSQL");
var providerSetting = builder.Configuration.GetValue<string>("DatabaseProvider");

var usePostgres = false;

if (!string.IsNullOrWhiteSpace(providerSetting))
{
    var normalizedProvider = providerSetting.Trim();
    usePostgres = normalizedProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase)
        || normalizedProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase)
        || normalizedProvider.Equals("Npgsql", StringComparison.OrdinalIgnoreCase);
}
else if (!string.IsNullOrWhiteSpace(postgresConnection))
{
    usePostgres = true;
}
else if (!string.IsNullOrWhiteSpace(defaultConnection))
{
    usePostgres = defaultConnection.Contains("Host=", StringComparison.OrdinalIgnoreCase)
        || defaultConnection.Contains("Username=", StringComparison.OrdinalIgnoreCase)
        || defaultConnection.Contains("User ID=", StringComparison.OrdinalIgnoreCase);
}

// Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (usePostgres)
    {
        var connectionString = postgresConnection ?? defaultConnection
            ?? throw new InvalidOperationException("A PostgreSQL connection string is required when DatabaseProvider is configured for PostgreSQL.");
        options.UseNpgsql(connectionString);
    }
    else
    {
        var connectionString = defaultConnection
            ?? throw new InvalidOperationException("A SQL Server connection string named 'DefaultConnection' is required when DatabaseProvider is configured for SQL Server.");
        options.UseSqlServer(connectionString);
    }
});

// Repository Pattern and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();

// JWT Authentication
var jwtSecret = builder.Configuration["JWT:Secret"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
var jwtIssuer = builder.Configuration["JWT:Issuer"] ?? "ECommerceInventoryAPI";
var jwtAudience = builder.Configuration["JWT:Audience"] ?? "ECommerceInventoryApp";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Swagger/OpenAPI configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "E-Commerce Inventory API",
        Version = "v1",
        Description = "A RESTful API for managing e-commerce inventory with JWT authentication",
        Contact = new OpenApiContact
        {
            Name = "E-Commerce Inventory API",
            Email = "contact@ecommerce-inventory.com"
        }
    });

    // Add JWT authentication support to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

// CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce Inventory API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
