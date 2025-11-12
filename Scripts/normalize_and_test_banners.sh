#!/usr/bin/env bash
# normalize_and_test_banners.sh
# - Normalizes SortOrder for collection banners
# - Optionally simulates single-banner reorders via admin API
# - Prints verification SELECTs so you can confirm results
#
# Usage:
#   ./Scripts/normalize_and_test_banners.sh \
#       --db-host localhost --db-port 5432 \
#       --db-name johnhenry_db --db-user johnhenry_user \
#       --db-pass <PASSWORD> --base-url http://localhost:5101 \
#       [--simulate-reorder <BANNER_ID> <TARGET_SORT>]
#
# Notes:
# - Requires psql in PATH and curl
# - Runs normalization for collection_hero groups (partitioned by TargetPage)
# - When --simulate-reorder is provided it will call the admin reorder API for that banner id

set -euo pipefail

# defaults
DB_HOST="localhost"
DB_PORT=5432
DB_NAME="johnhenry_db"
DB_USER="johnhenry_user"
DB_PASS=""
BASE_URL="http://localhost:5101"
SIMULATE_REORDER=0
REORDER_BANNER_ID=""
REORDER_TARGET_SORT=0

print_usage() {
  sed -n '1,120p' "$0" | sed -n '1,60p'
}

# parse args
while [[ $# -gt 0 ]]; do
  case "$1" in
    --db-host) DB_HOST="$2"; shift 2;;
    --db-port) DB_PORT="$2"; shift 2;;
    --db-name) DB_NAME="$2"; shift 2;;
    --db-user) DB_USER="$2"; shift 2;;
    --db-pass) DB_PASS="$2"; shift 2;;
    --base-url) BASE_URL="$2"; shift 2;;
    --simulate-reorder) SIMULATE_REORDER=1; REORDER_BANNER_ID="$2"; REORDER_TARGET_SORT="$3"; shift 3;;
    -h|--help) print_usage; exit 0;;
    *) echo "Unknown arg: $1"; print_usage; exit 1;;
  esac
done

if [[ -z "$DB_PASS" ]]; then
  echo "ERROR: --db-pass is required"
  exit 1
fi

export PGPASSWORD="$DB_PASS"

echo "[INFO] Normalizing SortOrder for collection_hero (grouped by TargetPage)"
psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -v ON_ERROR_STOP=1 <<'SQL'
-- Re-number collection_hero banners inside each TargetPage to sequential 1..N
WITH Ranked AS (
  SELECT "Id", ROW_NUMBER() OVER (PARTITION BY "TargetPage" ORDER BY "SortOrder", "CreatedAt") AS new_sort
  FROM "MarketingBanners"
  WHERE "Position" = 'collection_hero'
)
UPDATE "MarketingBanners" m
SET "SortOrder" = r.new_sort, "UpdatedAt" = NOW()
FROM Ranked r
WHERE m."Id" = r."Id";

-- Also normalize home_main and home_side globally
WITH RankedHomeMain AS (
  SELECT "Id", ROW_NUMBER() OVER (ORDER BY "SortOrder", "CreatedAt") AS new_sort
  FROM "MarketingBanners"
  WHERE "Position" = 'home_main'
)
UPDATE "MarketingBanners" m
SET "SortOrder" = r.new_sort, "UpdatedAt" = NOW()
FROM RankedHomeMain r
WHERE m."Id" = r."Id";

WITH RankedHomeSide AS (
  SELECT "Id", ROW_NUMBER() OVER (ORDER BY "SortOrder", "CreatedAt") AS new_sort
  FROM "MarketingBanners"
  WHERE "Position" = 'home_side'
)
UPDATE "MarketingBanners" m
SET "SortOrder" = r.new_sort, "UpdatedAt" = NOW()
FROM RankedHomeSide r
WHERE m."Id" = r."Id";

-- Verify
SELECT "Position", "TargetPage", "Title", "SortOrder" FROM "MarketingBanners" WHERE "Position" IN ('collection_hero','home_main','home_side') ORDER BY "Position","TargetPage","SortOrder";
SQL

echo

if [[ "$SIMULATE_REORDER" -eq 1 ]]; then
  if [[ -z "$REORDER_BANNER_ID" || -z "$REORDER_TARGET_SORT" ]]; then
    echo "--simulate-reorder requires <BANNER_ID> <TARGET_SORT>"
    exit 1
  fi

  echo "[INFO] Simulating reorder: banner=$REORDER_BANNER_ID -> targetSort=$REORDER_TARGET_SORT"

  # Determine banner's Position and TargetPage to include in body
  read -r POSITION TARGETPAGE <<< $(psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -t -A -c "SELECT \"Position\", COALESCE(\"TargetPage\", '') FROM \"MarketingBanners\" WHERE \"Id\" = '$REORDER_BANNER_ID';")

  if [[ -z "$POSITION" ]]; then
    echo "ERROR: banner id not found: $REORDER_BANNER_ID"
    exit 1
  fi

  if [[ "$TARGETPAGE" == "" ]]; then TARGETPAGE=null; else TARGETPAGE=\"$TARGETPAGE\"; fi

  # Build payload (match server model PascalCase)
  read -r response_status response_body <<< $(curl -s -o /tmp/reorder_result.json -w "%{http_code}" -X POST "$BASE_URL/admin/api/banners/$REORDER_BANNER_ID/reorder" \
    -H "Content-Type: application/json" \
    -d "{\"Position\": \"$POSITION\", \"TargetPage\": $TARGETPAGE, \"NewSortOrder\": $REORDER_TARGET_SORT, \"OldSortOrder\": 0 }" )

  echo "[HTTP] Status: $response_status"
  cat /tmp/reorder_result.json || true
  echo
  echo "[DB] Verify group after simulated reorder"
  psql -h "$DB_HOST" -p "$DB_PORT" -U "$DB_USER" -d "$DB_NAME" -c "SELECT \"Id\", \"Title\", \"Position\", \"TargetPage\", \"SortOrder\" FROM \"MarketingBanners\" WHERE \"Position\" = '$POSITION' AND (\"TargetPage\" = '' OR \"TargetPage\" = COALESCE(NULLIF('$TARGETPAGE','null'),'') ) ORDER BY \"SortOrder\";"
fi

echo
echo "[DONE]"

# Unset password var to avoid leakage
unset PGPASSWORD
