#!/usr/bin/env bash
# Quick script to fix banner order
# Usage: ./Scripts/fix_banners.sh

set -euo pipefail

DB_HOST="${DB_HOST:-localhost}"
DB_PORT="${DB_PORT:-5432}"
DB_NAME="${DB_NAME:-johnhenry_db}"
DB_USER="${DB_USER:-johnhenry_user}"
DB_PASS="${DB_PASS:-johnhenry2024}"

export PGPASSWORD="$DB_PASS"

echo "🔧 Fixing banner order..."
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -f "$(dirname "$0")/fix_banner_order.sql"

echo "✅ Done! Check output above for verification."

unset PGPASSWORD
