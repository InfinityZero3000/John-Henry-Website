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

// Load environment variables from .env file
DotNetEnv.Env.Load();

// Override configuration with environment variables
var configuration = builder.Configuration;

// Database Configuration
configuration["ConnectionStrings:DefaultConnection"] = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("DB_PORT")};Database={Environment.GetEnvironmentVariable("DB_NAME")};Username={Environment.GetEnvironmentVariable("DB_USER")};Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};SSL Mode=Prefer;Trust Server Certificate=true";
configuration["ConnectionStrings:Redis"] = Environment.GetEnvironmentVariable("REDIS_CONNECTION");

// JWT Configuration
configuration["JWT:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET");
configuration["JWT:Issuer"] = Environment.GetEnvironmentVariable("JWT_ISSUER");
configuration["JWT:Audience"] = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
configuration["JWT:ExpiryHours"] = Environment.GetEnvironmentVariable("JWT_EXPIRY_HOURS");

// Google OAuth
configuration["Authentication:Google:ClientId"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
configuration["Authentication:Google:ClientSecret"] = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");

// Email Settings
configuration["EmailSettings:SmtpServer"] = Environment.GetEnvironmentVariable("EMAIL_HOST");
configuration["EmailSettings:SmtpPort"] = Environment.GetEnvironmentVariable("EMAIL_PORT");
configuration["EmailSettings:UseSsl"] = Environment.GetEnvironmentVariable("EMAIL_USE_SSL");
configuration["EmailSettings:Username"] = Environment.GetEnvironmentVariable("EMAIL_USER");
configuration["EmailSettings:Password"] = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
configuration["EmailSettings:FromEmail"] = Environment.GetEnvironmentVariable("EMAIL_FROM");
configuration["EmailSettings:FromName"] = Environment.GetEnvironmentVariable("EMAIL_FROM_NAME");
configuration["EmailSettings:BaseUrl"] = Environment.GetEnvironmentVariable("BASE_URL");

// Site Settings
configuration["SiteSettings:BaseUrl"] = Environment.GetEnvironmentVariable("BASE_URL");
configuration["SiteSettings:SiteName"] = Environment.GetEnvironmentVariable("SITE_NAME");
configuration["SiteSettings:CacheDurationMinutes"] = Environment.GetEnvironmentVariable("CACHE_DURATION_MINUTES");
configuration["SiteSettings:EnableImageOptimization"] = Environment.GetEnvironmentVariable("ENABLE_IMAGE_OPTIMIZATION");
configuration["SiteSettings:MaxImageWidth"] = Environment.GetEnvironmentVariable("MAX_IMAGE_WIDTH");
configuration["SiteSettings:ImageQuality"] = Environment.GetEnvironmentVariable("IMAGE_QUALITY");

// File Upload
configuration["FileUpload:MaxFileSize"] = Environment.GetEnvironmentVariable("MAX_FILE_SIZE");
configuration["FileUpload:UploadPath"] = Environment.GetEnvironmentVariable("UPLOAD_PATH");

// Payment Gateways
configuration["PaymentGateways:VNPay:TmnCode"] = Environment.GetEnvironmentVariable("VNPAY_TMN_CODE");
configuration["PaymentGateways:VNPay:HashSecret"] = Environment.GetEnvironmentVariable("VNPAY_HASH_SECRET");
configuration["PaymentGateways:VNPay:PaymentUrl"] = Environment.GetEnvironmentVariable("VNPAY_PAYMENT_URL");
configuration["PaymentGateways:VNPay:ApiUrl"] = Environment.GetEnvironmentVariable("VNPAY_API_URL");
configuration["PaymentGateways:VNPay:IsEnabled"] = Environment.GetEnvironmentVariable("VNPAY_ENABLED");
configuration["PaymentGateways:VNPay:IsSandbox"] = Environment.GetEnvironmentVariable("VNPAY_SANDBOX");

configuration["PaymentGateways:MoMo:PartnerCode"] = Environment.GetEnvironmentVariable("MOMO_PARTNER_CODE");
configuration["PaymentGateways:MoMo:AccessKey"] = Environment.GetEnvironmentVariable("MOMO_ACCESS_KEY");
configuration["PaymentGateways:MoMo:SecretKey"] = Environment.GetEnvironmentVariable("MOMO_SECRET_KEY");
configuration["PaymentGateways:MoMo:ApiUrl"] = Environment.GetEnvironmentVariable("MOMO_API_URL");
configuration["PaymentGateways:MoMo:PublicKey"] = Environment.GetEnvironmentVariable("MOMO_PUBLIC_KEY");
configuration["PaymentGateways:MoMo:PrivateKey"] = Environment.GetEnvironmentVariable("MOMO_PRIVATE_KEY");
configuration["PaymentGateways:MoMo:IsEnabled"] = Environment.GetEnvironmentVariable("MOMO_ENABLED");
configuration["PaymentGateways:MoMo:IsSandbox"] = Environment.GetEnvironmentVariable("MOMO_SANDBOX");

configuration["PaymentGateways:Stripe:PublishableKey"] = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY");
configuration["PaymentGateways:Stripe:SecretKey"] = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
configuration["PaymentGateways:Stripe:WebhookSecret"] = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
configuration["PaymentGateways:Stripe:IsEnabled"] = Environment.GetEnvironmentVariable("STRIPE_ENABLED");
configuration["PaymentGateways:Stripe:IsSandbox"] = Environment.GetEnvironmentVariable("STRIPE_SANDBOX");

configuration["PaymentGateways:CashOnDelivery:IsEnabled"] = Environment.GetEnvironmentVariable("COD_ENABLED");
configuration["PaymentGateways:CashOnDelivery:MaxOrderAmount"] = Environment.GetEnvironmentVariable("COD_MAX_AMOUNT");
configuration["PaymentGateways:CashOnDelivery:ServiceFee"] = Environment.GetEnvironmentVariable("COD_SERVICE_FEE");

configuration["PaymentGateways:BankTransfer:IsEnabled"] = Environment.GetEnvironmentVariable("BANK_TRANSFER_ENABLED");
configuration["PaymentGateways:BankTransfer:BankAccounts:0:AccountNumber"] = Environment.GetEnvironmentVariable("BANK_VIETCOMBANK_ACCOUNT");
configuration["PaymentGateways:BankTransfer:BankAccounts:0:AccountHolder"] = Environment.GetEnvironmentVariable("BANK_VIETCOMBANK_HOLDER");
configuration["PaymentGateways:BankTransfer:BankAccounts:0:Branch"] = Environment.GetEnvironmentVariable("BANK_VIETCOMBANK_BRANCH");
configuration["PaymentGateways:BankTransfer:BankAccounts:1:AccountNumber"] = Environment.GetEnvironmentVariable("BANK_TECHCOMBANK_ACCOUNT");
configuration["PaymentGateways:BankTransfer:BankAccounts:1:AccountHolder"] = Environment.GetEnvironmentVariable("BANK_TECHCOMBANK_HOLDER");
configuration["PaymentGateways:BankTransfer:BankAccounts:1:Branch"] = Environment.GetEnvironmentVariable("BANK_TECHCOMBANK_BRANCH");

// Security Settings
configuration["Security:PasswordPolicy:MinLength"] = Environment.GetEnvironmentVariable("PASSWORD_MIN_LENGTH");
configuration["Security:PasswordPolicy:RequireDigit"] = Environment.GetEnvironmentVariable("PASSWORD_REQUIRE_DIGIT");
configuration["Security:PasswordPolicy:RequireLowercase"] = Environment.GetEnvironmentVariable("PASSWORD_REQUIRE_LOWERCASE");
configuration["Security:PasswordPolicy:RequireUppercase"] = Environment.GetEnvironmentVariable("PASSWORD_REQUIRE_UPPERCASE");
configuration["Security:PasswordPolicy:RequireSpecialChar"] = Environment.GetEnvironmentVariable("PASSWORD_REQUIRE_SPECIAL_CHAR");
configuration["Security:PasswordPolicy:MaxAgeDays"] = Environment.GetEnvironmentVariable("PASSWORD_MAX_AGE_DAYS");
configuration["Security:MaxLoginAttempts"] = Environment.GetEnvironmentVariable("MAX_LOGIN_ATTEMPTS");
configuration["Security:LockoutDurationMinutes"] = Environment.GetEnvironmentVariable("LOCKOUT_DURATION_MINUTES");
configuration["Security:SessionTimeoutMinutes"] = Environment.GetEnvironmentVariable("SESSION_TIMEOUT_MINUTES");
configuration["Security:RequireTwoFactorForAdmin"] = Environment.GetEnvironmentVariable("REQUIRE_2FA_FOR_ADMIN");
configuration["Security:RequireEmailConfirmation"] = Environment.GetEnvironmentVariable("REQUIRE_EMAIL_CONFIRMATION");

// Application Insights
configuration["ApplicationInsights:ConnectionString"] = Environment.GetEnvironmentVariable("APPLICATION_INSIGHTS_CONNECTION_STRING");

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
