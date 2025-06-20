#!/bin/bash

# SQL Server Management Service API Testing Script
# Bash script with curl for testing all API endpoints

set -e

# Configuration
BASE_URL="${BASE_URL:-https://localhost:7020/api}"
SERVER_INSTANCE="${SERVER_INSTANCE:-localhost}"
DATABASE_NAME="${DATABASE_NAME:-TestDB}"
SKIP_SSL="${SKIP_SSL:-false}"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
CYAN='\033[0;36m'
NC='\033[0m' # No Color

# Test variables
TEST_LOGIN_NAME="testuser_$(date +%s)"
TEST_PASSWORD="SecurePass123!"
TEST_WINDOWS_LOGIN="DOMAIN\\testuser_$(date +%s)"
TEST_SERVER_ROLE="CustomRole_$(date +%s)"
TEST_DATABASE_ROLE="CustomDBRole_$(date +%s)"
TEST_USER_NAME="dbuser_$(date +%s)"

# curl options
CURL_OPTS="-s -S"
if [ "$SKIP_SSL" = "true" ]; then
    CURL_OPTS="$CURL_OPTS -k"
fi

echo -e "${GREEN}Starting SQL Server Management Service API Tests${NC}"
echo -e "${YELLOW}Base URL: $BASE_URL${NC}"
echo -e "${YELLOW}Server Instance: $SERVER_INSTANCE${NC}"
echo -e "${YELLOW}Database: $DATABASE_NAME${NC}"

# Helper function for API calls
api_call() {
    local method="$1"
    local url="$2"
    local data="$3"
    local description="$4"
    
    echo -e "\n${CYAN}--- $description ---${NC}"
    echo -e "${BLUE}$method $url${NC}"
    
    if [ -n "$data" ]; then
        echo -e "${BLUE}Request Body: $data${NC}"
    fi
    
    local cmd="curl $CURL_OPTS -X $method"
    
    if [ -n "$data" ]; then
        cmd="$cmd -H 'Content-Type: application/json' -d '$data'"
    fi
    
    cmd="$cmd '$url'"
    
    if response=$(eval $cmd 2>&1); then
        echo -e "${GREEN}✅ SUCCESS${NC}"
        if [ -n "$response" ] && [ "$response" != "null" ] && [ "$response" != "" ]; then
            echo -e "${NC}Response: $response${NC}"
        fi
        echo "$response"
    else
        echo -e "${RED}❌ FAILED: $response${NC}"
        return 1
    fi
}

# Test 1: Health Check
echo -e "\n${CYAN}🔍 Testing Health Check...${NC}"
api_call "GET" "${BASE_URL}/../health" "" "Health Check" || true

# Test 2: Get All Logins
echo -e "\n${CYAN}🔍 Testing Login Management...${NC}"
logins=$(api_call "GET" "$BASE_URL/SqlLogin/server/$SERVER_INSTANCE" "" "Get All Logins" || echo "")

# Test 3: Create SQL Login
create_login_request="{
  \"loginName\": \"$TEST_LOGIN_NAME\",
  \"password\": \"$TEST_PASSWORD\",
  \"loginType\": \"SQL\",
  \"serverInstance\": \"$SERVER_INSTANCE\",
  \"defaultDatabase\": \"master\"
}"

created_login=$(api_call "POST" "$BASE_URL/SqlLogin" "$create_login_request" "Create SQL Login" || echo "")

if [ -n "$created_login" ] && [ "$created_login" != "null" ]; then
    # Test 4: Get Specific Login
    api_call "GET" "$BASE_URL/SqlLogin/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE" "" "Get Specific Login" || true
    
    # Test 5: Update Login (disable)
    update_request='{"isEnabled": false}'
    api_call "PUT" "$BASE_URL/SqlLogin/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE" "$update_request" "Disable Login" || true
    
    # Test 6: Update Login (re-enable and change password)
    update_request='{"isEnabled": true, "newPassword": "NewSecurePass123!"}'
    api_call "PUT" "$BASE_URL/SqlLogin/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE" "$update_request" "Enable Login and Change Password" || true
    
    # Test 7: Get Login Server Roles
    api_call "GET" "$BASE_URL/SqlLogin/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE/roles" "" "Get Login Server Roles" || true
fi

# Test 8: Create Windows Login
create_windows_login_request="{
  \"loginName\": \"$TEST_WINDOWS_LOGIN\",
  \"loginType\": \"Windows\",
  \"serverInstance\": \"$SERVER_INSTANCE\",
  \"defaultDatabase\": \"master\"
}"

created_windows_login=$(api_call "POST" "$BASE_URL/SqlLogin" "$create_windows_login_request" "Create Windows Login" || echo "")

# Test 9: Server Role Management
echo -e "\n${CYAN}🔍 Testing Server Role Management...${NC}"

# Get all server roles
server_roles=$(api_call "GET" "$BASE_URL/ServerRole/server/$SERVER_INSTANCE" "" "Get All Server Roles" || echo "")

# Create custom server role
create_server_role_request="{
  \"roleName\": \"$TEST_SERVER_ROLE\",
  \"serverInstance\": \"$SERVER_INSTANCE\"
}"

created_server_role=$(api_call "POST" "$BASE_URL/ServerRole" "$create_server_role_request" "Create Server Role" || echo "")

if [ -n "$created_server_role" ] && [ "$created_server_role" != "null" ]; then
    # Get specific server role
    api_call "GET" "$BASE_URL/ServerRole/$TEST_SERVER_ROLE/server/$SERVER_INSTANCE" "" "Get Specific Server Role" || true
    
    # Get role members
    api_call "GET" "$BASE_URL/ServerRole/$TEST_SERVER_ROLE/server/$SERVER_INSTANCE/members" "" "Get Role Members" || true
    
    # Add member to role (if we have a login)
    if [ -n "$created_login" ] && [ "$created_login" != "null" ]; then
        add_member_request="{
          \"roleName\": \"$TEST_SERVER_ROLE\",
          \"memberName\": \"$TEST_LOGIN_NAME\",
          \"serverInstance\": \"$SERVER_INSTANCE\"
        }"
        api_call "POST" "$BASE_URL/ServerRole/members" "$add_member_request" "Add Member to Server Role" || true
        
        # Remove member from role
        api_call "DELETE" "$BASE_URL/ServerRole/members" "$add_member_request" "Remove Member from Server Role" || true
    fi
fi

# Test 10: Database User Management
echo -e "\n${CYAN}🔍 Testing Database User Management...${NC}"

# Get all database users
database_users=$(api_call "GET" "$BASE_URL/DatabaseUser/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Get All Database Users" || echo "")

# Create database user (with login)
if [ -n "$created_login" ] && [ "$created_login" != "null" ]; then
    create_user_request="{
      \"userName\": \"$TEST_USER_NAME\",
      \"loginName\": \"$TEST_LOGIN_NAME\",
      \"databaseName\": \"$DATABASE_NAME\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"defaultSchema\": \"dbo\"
    }"
    
    created_user=$(api_call "POST" "$BASE_URL/DatabaseUser" "$create_user_request" "Create Database User with Login" || echo "")
    
    if [ -n "$created_user" ] && [ "$created_user" != "null" ]; then
        # Get specific database user
        api_call "GET" "$BASE_URL/DatabaseUser/$TEST_USER_NAME/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Get Specific Database User" || true
        
        # Get users by login
        api_call "GET" "$BASE_URL/DatabaseUser/login/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE" "" "Get Users by Login" || true
        
        # Update database user
        update_user_request='{"defaultSchema": "sales"}'
        api_call "PUT" "$BASE_URL/DatabaseUser/$TEST_USER_NAME/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "$update_user_request" "Update Database User" || true
        
        # Get user roles
        api_call "GET" "$BASE_URL/DatabaseUser/$TEST_USER_NAME/database/$DATABASE_NAME/server/$SERVER_INSTANCE/roles" "" "Get User Roles" || true
        
        # Add user to built-in role
        add_user_to_role_request="{
          \"memberName\": \"$TEST_USER_NAME\",
          \"roleName\": \"db_datareader\",
          \"databaseName\": \"$DATABASE_NAME\",
          \"serverInstance\": \"$SERVER_INSTANCE\"
        }"
        api_call "POST" "$BASE_URL/DatabaseUser/roles" "$add_user_to_role_request" "Add User to Database Role" || true
        
        # Remove user from role
        api_call "DELETE" "$BASE_URL/DatabaseUser/roles" "$add_user_to_role_request" "Remove User from Database Role" || true
    fi
fi

# Test 11: Database Role Management
echo -e "\n${CYAN}🔍 Testing Database Role Management...${NC}"

# Get all database roles
database_roles=$(api_call "GET" "$BASE_URL/DatabaseRole/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Get All Database Roles" || echo "")

# Create custom database role
create_db_role_request="{
  \"roleName\": \"$TEST_DATABASE_ROLE\",
  \"databaseName\": \"$DATABASE_NAME\",
  \"serverInstance\": \"$SERVER_INSTANCE\"
}"

created_db_role=$(api_call "POST" "$BASE_URL/DatabaseRole" "$create_db_role_request" "Create Database Role" || echo "")

if [ -n "$created_db_role" ] && [ "$created_db_role" != "null" ]; then
    # Get specific database role
    api_call "GET" "$BASE_URL/DatabaseRole/$TEST_DATABASE_ROLE/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Get Specific Database Role" || true
    
    # Get role members
    api_call "GET" "$BASE_URL/DatabaseRole/$TEST_DATABASE_ROLE/database/$DATABASE_NAME/server/$SERVER_INSTANCE/members" "" "Get Database Role Members" || true
    
    # Add member to database role (if we have a user)
    if [ -n "$created_user" ] && [ "$created_user" != "null" ]; then
        add_db_role_member_request="{
          \"roleName\": \"$TEST_DATABASE_ROLE\",
          \"memberName\": \"$TEST_USER_NAME\",
          \"databaseName\": \"$DATABASE_NAME\",
          \"serverInstance\": \"$SERVER_INSTANCE\"
        }"
        api_call "POST" "$BASE_URL/DatabaseRole/members" "$add_db_role_member_request" "Add Member to Database Role" || true
        
        # Remove member from database role
        api_call "DELETE" "$BASE_URL/DatabaseRole/members" "$add_db_role_member_request" "Remove Member from Database Role" || true
    fi
    
    # Grant permission to role
    grant_permission_request="{
      \"roleName\": \"$TEST_DATABASE_ROLE\",
      \"permission\": \"SELECT\",
      \"databaseName\": \"$DATABASE_NAME\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"objectName\": \"Users\"
    }"
    api_call "POST" "$BASE_URL/DatabaseRole/permissions" "$grant_permission_request" "Grant Permission to Database Role" || true
    
    # Get role permissions
    api_call "GET" "$BASE_URL/DatabaseRole/$TEST_DATABASE_ROLE/database/$DATABASE_NAME/server/$SERVER_INSTANCE/permissions" "" "Get Database Role Permissions" || true
    
    # Revoke permission from role
    api_call "DELETE" "$BASE_URL/DatabaseRole/permissions" "$grant_permission_request" "Revoke Permission from Database Role" || true
fi

# Test 12: Bulk Operations
echo -e "\n${CYAN}🔍 Testing Bulk Operations...${NC}"

# Create additional test logins for bulk operations
bulk_logins=()
for i in {1..3}; do
    bulk_login_name="bulkuser${i}_$(date +%s)"
    bulk_logins+=("$bulk_login_name")
    
    bulk_login_request="{
      \"loginName\": \"$bulk_login_name\",
      \"password\": \"BulkPass123!\",
      \"loginType\": \"SQL\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"defaultDatabase\": \"master\"
    }"
    
    api_call "POST" "$BASE_URL/SqlLogin" "$bulk_login_request" "Create Bulk Test Login $i" || true
done

# Bulk disable operations
if [ ${#bulk_logins[@]} -gt 0 ]; then
    bulk_login_names=$(printf ',"%s"' "${bulk_logins[@]}")
    bulk_login_names="[${bulk_login_names:1}]"
    
    bulk_disable_request="{
      \"operation\": \"disable\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"loginNames\": $bulk_login_names
    }"
    
    api_call "POST" "$BASE_URL/SqlLogin/bulk" "$bulk_disable_request" "Bulk Disable Logins" || true
    
    # Bulk enable operations
    bulk_enable_request="{
      \"operation\": \"enable\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"loginNames\": $bulk_login_names
    }"
    
    api_call "POST" "$BASE_URL/SqlLogin/bulk" "$bulk_enable_request" "Bulk Enable Logins" || true
fi

# Cleanup: Delete all test objects
echo -e "\n${YELLOW}🧹 Cleaning up test objects...${NC}"

# Delete database user
if [ -n "$created_user" ] && [ "$created_user" != "null" ]; then
    api_call "DELETE" "$BASE_URL/DatabaseUser/$TEST_USER_NAME/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Delete Test Database User" || true
fi

# Delete database role
if [ -n "$created_db_role" ] && [ "$created_db_role" != "null" ]; then
    api_call "DELETE" "$BASE_URL/DatabaseRole/$TEST_DATABASE_ROLE/database/$DATABASE_NAME/server/$SERVER_INSTANCE" "" "Delete Test Database Role" || true
fi

# Delete server role
if [ -n "$created_server_role" ] && [ "$created_server_role" != "null" ]; then
    api_call "DELETE" "$BASE_URL/ServerRole/$TEST_SERVER_ROLE/server/$SERVER_INSTANCE" "" "Delete Test Server Role" || true
fi

# Delete bulk test logins
if [ ${#bulk_logins[@]} -gt 0 ]; then
    bulk_delete_request="{
      \"operation\": \"delete\",
      \"serverInstance\": \"$SERVER_INSTANCE\",
      \"loginNames\": $bulk_login_names
    }"
    
    api_call "POST" "$BASE_URL/SqlLogin/bulk" "$bulk_delete_request" "Bulk Delete Test Logins" || true
fi

# Delete test logins
if [ -n "$created_login" ] && [ "$created_login" != "null" ]; then
    api_call "DELETE" "$BASE_URL/SqlLogin/$TEST_LOGIN_NAME/server/$SERVER_INSTANCE" "" "Delete Test SQL Login" || true
fi

if [ -n "$created_windows_login" ] && [ "$created_windows_login" != "null" ]; then
    # URL encode the windows login name
    encoded_windows_login=$(echo "$TEST_WINDOWS_LOGIN" | sed 's/\\/\%5C/g')
    api_call "DELETE" "$BASE_URL/SqlLogin/$encoded_windows_login/server/$SERVER_INSTANCE" "" "Delete Test Windows Login" || true
fi

echo -e "\n${GREEN}✅ API Testing Complete!${NC}"
echo -e "${YELLOW}Check the output above for any failed tests.${NC}"