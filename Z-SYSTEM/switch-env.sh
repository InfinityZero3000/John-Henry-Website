#!/bin/bash

# ========================================
# ENVIRONMENT SWITCHER
# ========================================
# Usage: 
#   ./switch-env.sh dev       - Switch to development (localhost:5001)
#   ./switch-env.sh local     - Switch to local server (localhost:8080)
#   ./switch-env.sh prod      - Switch to production (johnhenry-infinityzero.com)

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

ENV=$1

if [ -z "$ENV" ]; then
    echo -e "${RED}❌ Error: Environment not specified${NC}"
    echo ""
    echo "Usage:"
    echo -e "  ${YELLOW}./switch-env.sh dev${NC}     - Development (https://localhost:5001)"
    echo -e "  ${YELLOW}./switch-env.sh local${NC}   - Local Server (http://localhost:8080)"
    echo -e "  ${YELLOW}./switch-env.sh prod${NC}    - Production (https://johnhenry-infinityzero.com)"
    echo ""
    exit 1
fi

# Backup current .env
if [ -f .env ]; then
    cp .env .env.backup.$(date +%Y%m%d_%H%M%S)
    echo -e "${GREEN}✅ Backed up current .env${NC}"
fi

case $ENV in
    dev|development)
        if [ -f .env.backup.* ]; then
            # Find the most recent backup that looks like development
            LATEST_BACKUP=$(ls -t .env.backup.* | head -1)
            if grep -q "localhost:5001" "$LATEST_BACKUP" 2>/dev/null; then
                cp "$LATEST_BACKUP" .env
                echo -e "${GREEN}✅ Switched to DEVELOPMENT environment${NC}"
                echo -e "${BLUE}📍 Base URL: https://localhost:5001${NC}"
                echo -e "${BLUE}🔧 Payment Gateways: SANDBOX mode${NC}"
            else
                echo -e "${YELLOW}⚠️  No development backup found, keeping current .env${NC}"
            fi
        else
            echo -e "${YELLOW}⚠️  Current .env is already development${NC}"
        fi
        ;;
    
    local|localhost)
        if [ ! -f .env.local ]; then
            echo -e "${RED}❌ Error: .env.local not found${NC}"
            exit 1
        fi
        cp .env.local .env
        echo -e "${GREEN}✅ Switched to LOCAL SERVER environment${NC}"
        echo -e "${BLUE}📍 Base URL: http://localhost:8080${NC}"
        echo -e "${BLUE}🔧 Payment Gateways: SANDBOX mode${NC}"
        echo ""
        echo -e "${YELLOW}💡 To run on localhost:8080:${NC}"
        echo -e "   ${YELLOW}dotnet run --urls=http://localhost:8080${NC}"
        ;;
    
    prod|production)
        if [ ! -f .env.production ]; then
            echo -e "${RED}❌ Error: .env.production not found${NC}"
            exit 1
        fi
        cp .env.production .env
        echo -e "${GREEN}✅ Switched to PRODUCTION environment${NC}"
        echo -e "${BLUE}📍 Base URL: https://johnhenry-infinityzero.com${NC}"
        echo -e "${RED}⚠️  Payment Gateways: PRODUCTION mode (need real credentials!)${NC}"
        echo ""
        echo -e "${YELLOW}🔐 IMPORTANT: Update production credentials in .env:${NC}"
        echo "   - VNPay: VNPAY_TMN_CODE, VNPAY_HASH_SECRET"
        echo "   - MoMo: MOMO_PARTNER_CODE, MOMO_ACCESS_KEY, MOMO_SECRET_KEY"
        echo "   - Stripe: pk_live_xxx, sk_live_xxx"
        ;;
    
    *)
        echo -e "${RED}❌ Invalid environment: $ENV${NC}"
        echo ""
        echo "Valid options: dev, local, prod"
        exit 1
        ;;
esac

echo ""
echo -e "${BLUE}🔍 Verifying configuration...${NC}"
echo ""

# Show current config
echo -e "${YELLOW}Current Configuration:${NC}"
grep "^BASE_URL=" .env | sed 's/BASE_URL=/  Base URL: /'
grep "^ASPNETCORE_ENVIRONMENT=" .env | sed 's/ASPNETCORE_ENVIRONMENT=/  Environment: /'
grep "^VNPAY_SANDBOX=" .env | sed 's/VNPAY_SANDBOX=/  VNPay Sandbox: /'
grep "^MOMO_SANDBOX=" .env | sed 's/MOMO_SANDBOX=/  MoMo Sandbox: /'
grep "^STRIPE_SANDBOX=" .env | sed 's/STRIPE_SANDBOX=/  Stripe Sandbox: /'

echo ""
echo -e "${GREEN}✅ Environment switched successfully!${NC}"
echo ""
echo -e "${YELLOW}Next steps:${NC}"
echo "1. Review .env file to ensure settings are correct"
echo "2. Restart application: dotnet run"
echo "3. Test payment gateways"
