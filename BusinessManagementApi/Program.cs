using BusinessManagementApi.Helpers;
using BusinessManagementApi.Models;
using BusinessManagementApi.Repositories;
using BusinessManagementApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")  
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});


/// <summary>
/// JWT CONFIGURATION
/// </summary>

// Read JWT values from appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Register helper classes
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<AuthService>();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is missing"))
        )
    };
});

/// <summary>
/// MONGODB CONFIGURATION
/// </summary>

// Read MongoDB config from appsettings.json
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

// Register MongoDB client
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDbSettings section is missing.");

    return new MongoClient(mongoSettings.ConnectionString ?? throw new InvalidOperationException("MongoDB connection string is missing"));
});

// Register MongoDB database
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>()
        ?? throw new InvalidOperationException("MongoDbSettings section is missing.");

    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName ?? throw new InvalidOperationException("MongoDB database name is missing"));
});

// Register MongoDB context
builder.Services.AddSingleton<MongoDbContext>();

/// <summary>
/// REPOSITORIES & SERVICES
/// </summary>

// Directly register repository classes (no interfaces)
builder.Services.AddScoped<HistoryHelper>();
builder.Services.AddScoped<Repository<History>>(sp =>
    new Repository<History>(sp.GetRequiredService<MongoDbContext>(), "Histories"));

builder.Services.AddScoped(sp => new Repository<User>(sp.GetRequiredService<MongoDbContext>(), "Users"));
builder.Services.AddScoped(sp => new Repository<Business>(sp.GetRequiredService<MongoDbContext>(), "Businesses"));
builder.Services.AddScoped(sp => new Repository<Invoice>(sp.GetRequiredService<MongoDbContext>(), "Invoices"));

builder.Services.AddScoped<InvoiceRepository>(); // InvoiceRepository with custom methods
builder.Services.AddScoped<InvoiceService>();

builder.Services.AddSingleton<BusinessRepository>(); 
builder.Services.AddScoped<BusinessService>();

/// <summary>
/// CONTROLLERS & SWAGGER CONFIGURATION
/// </summary>

// Add controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Business Management API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Enter JWT token in format: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

/// <summary>
/// BUILD & RUN THE APP
/// </summary>

var app = builder.Build();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");


// Middlewares
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

app.Run();