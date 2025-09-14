using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using JohnHenryFashionWeb.Data;
using JohnHenryFashionWeb.Models;
using JohnHenryFashionWeb.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/john-henry-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add Entity Framework and PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity with enhanced security settings
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Password settings - Enhanced security
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 3;

    // Lockout settings - Enhanced protection
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;

    // User settings - Security focused
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;

    // Sign-in settings - Enhanced security
    options.SignIn.RequireConfirmedEmail = true;
    options.SignIn.RequireConfirmedAccount = true;
    options.SignIn.RequireConfirmedPhoneNumber = false;

    // Token settings
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
    options.Tokens.ChangeEmailTokenProvider = TokenOptions.DefaultEmailProvider;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Custom");

// Configure Cookie settings for enhanced security
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ReturnUrlParameter = "returnUrl";
    
    // Enhanced cookie security
    options.Cookie.Name = "JohnHenryAuth";
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.StatusCode = 401;
            return Task.CompletedTask;
        }
        context.Response.Redirect(context.RedirectUri);
        return Task.CompletedTask;
    };
});

// Add JWT Authentication
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
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"]!))
    };
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    googleOptions.SaveTokens = true;
});

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AdminPolicies.RequireAdminRole,
        policy => policy.RequireRole(UserRoles.Admin));
    
    options.AddPolicy(AdminPolicies.RequireSellerRole,
        policy => policy.RequireRole(UserRoles.Seller));
    
    options.AddPolicy(AdminPolicies.RequireAdminOrSellerRole,
        policy => policy.RequireRole(UserRoles.Admin, UserRoles.Seller));
});

// AutoMapper will be configured later

// Register custom services
builder.Services.AddScoped<JohnHenryFashionWeb.Services.ICacheService, JohnHenryFashionWeb.Services.CacheService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IImageOptimizationService, JohnHenryFashionWeb.Services.ImageOptimizationService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.ISeoService, JohnHenryFashionWeb.Services.SeoService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.SeoService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IPerformanceMonitorService, JohnHenryFashionWeb.Services.PerformanceMonitorService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IOptimizedDataService, JohnHenryFashionWeb.Services.OptimizedDataService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IEmailService, JohnHenryFashionWeb.Services.EmailService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.INotificationService, JohnHenryFashionWeb.Services.NotificationService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.ISecurityService, JohnHenryFashionWeb.Services.SecurityService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IAnalyticsService, JohnHenryFashionWeb.Services.AnalyticsService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IReportingService, JohnHenryFashionWeb.Services.ReportingService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IAuthService, JohnHenryFashionWeb.Services.AuthService>();
builder.Services.AddScoped<JohnHenryFashionWeb.Services.IPaymentService, JohnHenryFashionWeb.Services.PaymentService>();

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Add Memory Caching and Distributed Caching
builder.Services.AddMemoryCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "JohnHenryFashion";
});

// Configure Email Settings
builder.Services.Configure<JohnHenryFashionWeb.Services.EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

// Add Response Caching
builder.Services.AddResponseCaching();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "image/svg+xml",
        "application/json",
        "text/json"
    });
});

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// Add Controllers with Views and API support
builder.Services.AddControllersWithViews(options =>
{
    // Add response caching filter globally for performance
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.ResponseCacheAttribute
    {
        VaryByHeader = "User-Agent",
        Duration = 300 // 5 minutes default cache
    });
})
    .AddNewtonsoftJson();

// Add HttpClient for Payment Service
builder.Services.AddHttpClient();

// Add API Explorer for Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    
    // Add security headers for production
    app.Use((context, next) =>
    {
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
        return next();
    });
}

// Enable response compression
app.UseResponseCompression();

// Add custom middleware
app.UseMiddleware<JohnHenryFashionWeb.Middleware.PerformanceMiddleware>();

// Enable response caching
app.UseResponseCaching();

app.UseHttpsRedirection();

// Configure static files with caching
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        // Cache static files for 1 year
        const int durationInSeconds = 60 * 60 * 24 * 365;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// API Routes
app.MapControllerRoute(
    name: "api",
    pattern: "api/{controller}/{action=Index}/{id?}");

// Ensure database is created and roles seeded
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    
    try
    {
        context.Database.EnsureCreated();
        Log.Information("Database ensured created successfully");
        
        // Seed roles
        await SeedRoles(roleManager);
        Log.Information("Roles seeded successfully");
        
        // Seed admin user
        await SeedAdminUser(userManager);
        Log.Information("Admin user seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while ensuring database created or seeding data");
    }
}

// Helper methods for seeding
static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    var roles = new[] { UserRoles.Admin, UserRoles.Seller, UserRoles.Customer };
    
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
{
    var adminEmail = "admin@johnhenry.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "Admin",
            LastName = "John Henry",
            EmailConfirmed = true,
            IsActive = true,
            IsApproved = true,
            ApprovedAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }
    }

    // Create default seller user
    var sellerEmail = "seller@johnhenry.com";
    var sellerUser = await userManager.FindByEmailAsync(sellerEmail);
    
    if (sellerUser == null)
    {
        sellerUser = new ApplicationUser
        {
            UserName = sellerEmail,
            Email = sellerEmail,
            FirstName = "Seller",
            LastName = "Demo",
            EmailConfirmed = true,
            IsActive = true,
            IsApproved = true,
            ApprovedAt = DateTime.UtcNow
        };
        
        var result = await userManager.CreateAsync(sellerUser, "Seller123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(sellerUser, UserRoles.Seller);
        }
    }
}

try
{
    Log.Information("Starting John Henry Fashion Web Application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
