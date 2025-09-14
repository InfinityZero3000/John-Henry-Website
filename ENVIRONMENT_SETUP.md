# Environment Configuration Guide

This document explains how to configure the John Henry Fashion Web Platform using environment variables for secure deployment.

## Security Architecture

All sensitive configuration data is stored in environment variables instead of configuration files. This approach ensures:

- **Security**: No sensitive data in source control
- **Flexibility**: Easy configuration for different environments
- **Scalability**: Simple deployment across multiple environments
- **Best Practices**: Follows 12-factor app methodology

## Required Environment Variables

### Database Configuration
```bash
# PostgreSQL Database Settings
DB_HOST=localhost                    # Database host
DB_PORT=5432                        # Database port
DB_NAME=johnhenry_db                # Database name
DB_USER=johnhenry_user              # Database username
DB_PASSWORD=YourSecurePassword      # Database password

# Redis Cache Settings
REDIS_CONNECTION=localhost:6379      # Redis connection string
```

### Application Settings
```bash
# Application Environment
ASPNETCORE_ENVIRONMENT=Development   # Environment (Development/Staging/Production)
ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000
BASE_URL=https://localhost:5001     # Application base URL
SITE_NAME=John Henry Fashion        # Site display name
```

### Authentication & Security
```bash
# JWT Configuration
JWT_SECRET=YourSuperSecretJWTKey123!@#    # JWT signing key (min 32 chars)
JWT_ISSUER=JohnHenryFashion              # JWT issuer
JWT_AUDIENCE=JohnHenryUsers              # JWT audience
JWT_EXPIRY_HOURS=24                      # Token expiry time

# Google OAuth
GOOGLE_CLIENT_ID=your-google-client-id.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=your-google-client-secret

# Password Policy
PASSWORD_MIN_LENGTH=8                    # Minimum password length
PASSWORD_REQUIRE_DIGIT=true             # Require digits in password
PASSWORD_REQUIRE_LOWERCASE=true         # Require lowercase letters
PASSWORD_REQUIRE_UPPERCASE=true         # Require uppercase letters
PASSWORD_REQUIRE_SPECIAL_CHAR=true      # Require special characters
PASSWORD_MAX_AGE_DAYS=90                # Password expiry days
MAX_LOGIN_ATTEMPTS=5                    # Max failed login attempts
LOCKOUT_DURATION_MINUTES=15             # Account lockout duration
SESSION_TIMEOUT_MINUTES=30              # Session timeout
REQUIRE_2FA_FOR_ADMIN=true              # Require 2FA for admin users
REQUIRE_EMAIL_CONFIRMATION=true         # Require email confirmation
```

### Email Configuration
```bash
# SMTP Settings
EMAIL_HOST=smtp.gmail.com               # SMTP server
EMAIL_PORT=587                          # SMTP port
EMAIL_USE_SSL=true                      # Use SSL/TLS
EMAIL_USER=your-email@gmail.com         # SMTP username
EMAIL_PASSWORD=your-app-password        # SMTP password (use app password for Gmail)
EMAIL_FROM=noreply@johnhenry.com        # From email address
EMAIL_FROM_NAME=John Henry Fashion      # From display name
```

### Payment Gateway Configuration

#### VNPay
```bash
VNPAY_TMN_CODE=your-vnpay-terminal-code
VNPAY_HASH_SECRET=your-vnpay-hash-secret
VNPAY_PAYMENT_URL=https://sandbox.vnpayment.vn/paymentv2/vpcpay.html
VNPAY_API_URL=https://sandbox.vnpayment.vn/merchant_webapi/api/transaction
VNPAY_VERSION=2.1.0
VNPAY_ENABLED=true
VNPAY_SANDBOX=true
```

#### MoMo
```bash
MOMO_PARTNER_CODE=your-momo-partner-code
MOMO_ACCESS_KEY=your-momo-access-key
MOMO_SECRET_KEY=your-momo-secret-key
MOMO_API_URL=https://test-payment.momo.vn/v2/gateway/api
MOMO_PUBLIC_KEY=your-momo-public-key
MOMO_PRIVATE_KEY=your-momo-private-key
MOMO_ENABLED=true
MOMO_SANDBOX=true
```

#### Stripe
```bash
STRIPE_PUBLISHABLE_KEY=pk_test_your-stripe-publishable-key
STRIPE_SECRET_KEY=sk_test_your-stripe-secret-key
STRIPE_WEBHOOK_SECRET=whsec_your-stripe-webhook-secret
STRIPE_API_URL=https://api.stripe.com
STRIPE_CURRENCY=vnd
STRIPE_ENABLED=true
STRIPE_SANDBOX=true
```

#### Bank Transfer
```bash
BANK_TRANSFER_ENABLED=true
BANK_VIETCOMBANK_ACCOUNT=1234567890
BANK_VIETCOMBANK_HOLDER=JOHN HENRY FASHION
BANK_VIETCOMBANK_BRANCH=Hà Nội
BANK_TECHCOMBANK_ACCOUNT=0987654321
BANK_TECHCOMBANK_HOLDER=JOHN HENRY FASHION
BANK_TECHCOMBANK_BRANCH=TP.HCM
```

#### Cash on Delivery
```bash
COD_ENABLED=true
COD_MAX_AMOUNT=10000000
COD_SERVICE_FEE=0
```

### File Upload & Media
```bash
# File Upload Settings
MAX_FILE_SIZE=5242880                   # Max file size in bytes (5MB)
ALLOWED_EXTENSIONS=jpg,jpeg,png,gif,webp # Allowed file extensions
UPLOAD_PATH=wwwroot/uploads             # Upload directory
MAX_IMAGE_WIDTH=1200                    # Max image width for optimization
IMAGE_QUALITY=85                        # Image compression quality (1-100)
ENABLE_IMAGE_OPTIMIZATION=true          # Enable automatic image optimization
```

### Monitoring & Analytics
```bash
# Application Insights (optional)
APPLICATION_INSIGHTS_CONNECTION_STRING=your-app-insights-connection-string

# Cache Settings
CACHE_DURATION_MINUTES=30               # Default cache duration
```

## Environment Setup Instructions

### 1. Development Environment

1. **Copy the .env template**:
   ```bash
   cp .env.example .env
   ```

2. **Configure required variables**:
   - Database credentials for your local PostgreSQL
   - Google OAuth credentials from Google Console
   - Email settings (use Gmail App Password for development)
   - Generate a secure JWT secret key

3. **Optional configurations**:
   - Payment gateway credentials (for testing payment features)
   - Application Insights connection string (for monitoring)

### 2. Production Environment

#### Docker Deployment
```bash
# Create environment file
echo "DB_HOST=production-db-host" >> .env
echo "DB_PASSWORD=super-secure-production-password" >> .env
# ... add all other production variables

# Run with Docker Compose
docker-compose --env-file .env up -d
```

#### Azure App Service
1. Go to Azure Portal → App Service → Configuration
2. Add each environment variable as Application Settings
3. Restart the application

#### AWS/Other Cloud Providers
Configure environment variables through your cloud provider's configuration interface.

### 3. CI/CD Pipeline

For automated deployments, store sensitive variables in your CI/CD system's secret management:

- **GitHub Actions**: Use repository secrets
- **Azure DevOps**: Use variable groups with secret variables
- **Jenkins**: Use credential management
- **GitLab CI**: Use protected variables

## Security Best Practices

### Environment Variable Security
1. **Never commit .env files** to version control
2. **Use strong, unique passwords** for all services
3. **Rotate secrets regularly**, especially in production
4. **Use separate configurations** for each environment
5. **Limit access** to production environment variables

### JWT Security
- **JWT_SECRET**: Must be at least 32 characters long
- **Use cryptographically secure** random string generators
- **Rotate JWT secrets** periodically in production

### Database Security
- **Use strong passwords** with mixed characters
- **Enable SSL/TLS** for database connections
- **Use connection pooling** for better performance
- **Regular backup** and encryption at rest

### Email Security
- **Use App Passwords** instead of account passwords
- **Enable 2FA** on email service accounts
- **Use dedicated email accounts** for application sending

## Configuration Validation

The application includes built-in validation for critical configuration:

- Required environment variables are checked at startup
- Invalid configuration values will prevent application start
- Missing payment gateway credentials will disable specific payment methods
- Email configuration is validated before sending emails

## Troubleshooting

### Common Issues

#### 1. Application won't start
- Check that all required environment variables are set
- Verify database connection string format
- Ensure JWT secret is at least 32 characters

#### 2. Database connection fails
- Verify database host and port are accessible
- Check username and password are correct
- Ensure database exists and user has proper permissions

#### 3. Email sending fails
- Verify SMTP settings are correct
- Check if using Gmail, ensure App Password is used
- Confirm firewall allows SMTP traffic

#### 4. Payment gateway errors
- Verify API credentials are correct
- Check if using sandbox/test mode for development
- Ensure webhook URLs are properly configured

### Getting Help

If you encounter issues with configuration:

1. Check the application logs for specific error messages
2. Verify all environment variables are properly set
3. Test database and external service connectivity
4. Review the [GitHub Issues](https://github.com/InfinityZero3000/John-Henry-Website/issues) for similar problems

## Example .env File

```bash
# ========================================
# EXAMPLE .env FILE - DO NOT USE IN PRODUCTION
# ========================================

# Database Configuration
DB_HOST=localhost
DB_PORT=5432
DB_NAME=johnhenry_db
DB_USER=johnhenry_user
DB_PASSWORD=YourSecurePassword123!

# Application Configuration
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000
BASE_URL=https://localhost:5001
SITE_NAME=John Henry Fashion

# JWT Authentication
JWT_SECRET=ThisIsAVerySecureJWTSecretKeyThatShouldBeAtLeast32Characters!
JWT_ISSUER=JohnHenryFashion
JWT_AUDIENCE=JohnHenryUsers
JWT_EXPIRY_HOURS=24

# Google OAuth (Get from Google Cloud Console)
GOOGLE_CLIENT_ID=your-client-id.apps.googleusercontent.com
GOOGLE_CLIENT_SECRET=your-client-secret

# Email Configuration
EMAIL_HOST=smtp.gmail.com
EMAIL_PORT=587
EMAIL_USE_SSL=true
EMAIL_USER=your-email@gmail.com
EMAIL_PASSWORD=your-app-password
EMAIL_FROM=noreply@johnhenry.com
EMAIL_FROM_NAME=John Henry Fashion

# Cache Configuration
REDIS_CONNECTION=localhost:6379
CACHE_DURATION_MINUTES=30

# File Upload Configuration
MAX_FILE_SIZE=5242880
ALLOWED_EXTENSIONS=jpg,jpeg,png,gif,webp
UPLOAD_PATH=wwwroot/uploads
MAX_IMAGE_WIDTH=1200
IMAGE_QUALITY=85
ENABLE_IMAGE_OPTIMIZATION=true

# Security Configuration
PASSWORD_MIN_LENGTH=8
PASSWORD_REQUIRE_DIGIT=true
PASSWORD_REQUIRE_LOWERCASE=true
PASSWORD_REQUIRE_UPPERCASE=true
PASSWORD_REQUIRE_SPECIAL_CHAR=true
PASSWORD_MAX_AGE_DAYS=90
MAX_LOGIN_ATTEMPTS=5
LOCKOUT_DURATION_MINUTES=15
SESSION_TIMEOUT_MINUTES=30
REQUIRE_2FA_FOR_ADMIN=true
REQUIRE_EMAIL_CONFIRMATION=true

# Payment Gateways (Optional - for testing payments)
VNPAY_TMN_CODE=your-vnpay-code
VNPAY_HASH_SECRET=your-vnpay-secret
VNPAY_ENABLED=false
VNPAY_SANDBOX=true

MOMO_PARTNER_CODE=your-momo-code
MOMO_ACCESS_KEY=your-momo-access-key
MOMO_SECRET_KEY=your-momo-secret
MOMO_ENABLED=false
MOMO_SANDBOX=true

STRIPE_PUBLISHABLE_KEY=pk_test_your-stripe-key
STRIPE_SECRET_KEY=sk_test_your-stripe-secret
STRIPE_WEBHOOK_SECRET=whsec_your-webhook-secret
STRIPE_ENABLED=false
STRIPE_SANDBOX=true

# Bank Transfer & COD
BANK_TRANSFER_ENABLED=true
BANK_VIETCOMBANK_ACCOUNT=1234567890
BANK_VIETCOMBANK_HOLDER=JOHN HENRY FASHION
BANK_VIETCOMBANK_BRANCH=Hà Nội

COD_ENABLED=true
COD_MAX_AMOUNT=10000000
COD_SERVICE_FEE=0

# Monitoring (Optional)
APPLICATION_INSIGHTS_CONNECTION_STRING=
```

---

**Remember**: Never use the example values in production. Always generate secure, unique credentials for each environment.