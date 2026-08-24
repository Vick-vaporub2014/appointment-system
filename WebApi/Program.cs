using Application.Interfaces;
using Application.InterfacesRepo;
using Application.InterfacesServices;
using Application.Services;
using Application.UnitOfWork;
using Domain.Enums;  
using Infrastructure.DbContext;
using Infrastructure.Identity;
using Infrastructure.Repositries;
using Infrastructure.TokenGenerateService;
using Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//This means always load the appsettings.json file and then override it with the environment-specific file if it exists.
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();
//Read environment variables
var jwtKey = builder.Configuration["JWT_KEY"];
if(string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT_KEY debe estar configurado en el archivo de configuración.");
}
var jwtIssuer = builder.Configuration["JWT_ISSUER"];
var jwtAudience = builder.Configuration["JWT_AUDIENCE"];
var connectionString = builder.Configuration["DB_CONNECTION"];
if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("DB_CONNECTION debe estar configurado en el archivo de configuración.");
}

if (builder.Environment.IsProduction() || builder.Environment.IsEnvironment("Docker"))
{
    //if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(connectionString))
    //{
    //    throw new InvalidOperationException("JWT_KEY y DB_CONNECTION deben estar configurados en producción/Docker.");
    //}
}

if (string.IsNullOrEmpty(connectionString))
{
    // fallback a DefaultConnection solo en Development
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}


//Connection to SQL Server Database
//Dbcontext configuration with retry logic for transient faults
//This will automatically retry failed database operations up to 5 times with a delay of up to 10 seconds between retries, which can help improve the resilience
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,              // número de reintentos
            maxRetryDelay: TimeSpan.FromSeconds(10), // tiempo máximo entre reintentos
            errorNumbersToAdd: null        // usa los errores transitorios por defecto
        );
    }));
//Register the identity services and configure it to use the ApplicationDbContext and ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 12;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddScoped<ErrorHandlingMiddlewareService>();

//Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        NameClaimType = ClaimTypes.NameIdentifier, // Set the NameClaimType to NameIdentifier to ensure User.FindFirstValue(ClaimTypes.NameIdentifier) works correctly
        RoleClaimType = ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };
});

//Dynamic CORS CONFIGURATION
// Load allowed origins from configuration (appsettings.json)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DynamicCors", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
            policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
        else
            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
// Apply pending migrations and create the database if it does not exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();// This will apply any pending migrations and create the database if it does not exist

//Create default roles if they do not exist
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    string[] roles = { Roles.Patient, Roles.Admin, Roles.Doctor };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        context.Response.StatusCode = 500; // Internal Server Error
        context.Response.ContentType = "application/json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        
        if (error != null)
        {
           logger.LogError(error.Error, "An unhandled exception occurred.");
            var response = new ServiceResponse<string>
            {
                Success = false,
                Message = error.Error.Message,
                Data = null,
                ErrorType= "ServerError"

            };
            await context.Response.WriteAsJsonAsync(response);
        }
    });
});
app.UseRouting();
//Use  CORS policy
app.UseCors("DynamicCors");

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        //app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "API AppointmentSystem está corriendo ");
app.Run();


