# Azure SQL Migration Guide

## Updated Configuration
- **Server**: taskifyserver.database.windows.net
- **Database**: free-sql-db-9771280
- **Authentication**: Active Directory Default (no password needed)
- **Location**: Central India

## Prerequisites
- Azure CLI installed (`az --version` to check)
- Logged into Azure in Visual Studio or Azure CLI
- Your Azure AD account has permissions on the SQL database

## Quick Migration (Recommended)

### Run the PowerShell script:
```powershell
.\migrate-to-azure-sql.ps1
```

This script will:
1. Log you into Azure
2. Configure firewall rules
3. Verify the database exists
4. Build the solution
5. Apply all EF Core migrations using your Azure AD credentials

## Manual Migration Steps

### Step 1: Login to Azure
```bash
# Login to Azure CLI
az login

# Set the subscription
az account set --subscription b5eddd23-de05-443d-ae9d-6d78398b8974
```

### Step 2: Configure Firewall
Add your IP address to the firewall rules:

**Via Azure CLI:**
```bash
# Get your IP
$myIp = (Invoke-WebRequest -Uri "https://api.ipify.org").Content

# Add firewall rule
az sql server firewall-rule create \
  --resource-group taskify_Db \
  --server taskifyserver \
  --name AllowMyIP \
  --start-ip-address $myIp \
  --end-ip-address $myIp
```

**Via Azure Portal:**
1. Go to SQL Server: **taskifyserver**
2. Click **Networking** (left menu)
3. Click **+ Add your client IPv4 address**
4. Click **Save**

### Step 3: Grant Your Account Database Permissions

If you get permission errors, you need to grant your Azure AD account access to the database.

**Option A: Via Azure Portal Query Editor**
1. Go to the database **free-sql-db-9771280**
2. Click **Query editor (preview)**
3. Sign in with your Azure AD account
4. Run this SQL:
```sql
CREATE USER [your-email@outlook.com] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [your-email@outlook.com];
```

**Option B: Ask the SQL Server admin to run**
```sql
CREATE USER [bhargavi.mc@outlook.com] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [bhargavi.mc@outlook.com];
```

### Step 4: Update Connection String
The connection string is already configured in:
- `appsettings.json`
- `appsettings.Development.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;"
}
```

### Step 5: Install Required NuGet Package
```bash
dotnet add Taskify.Api package Microsoft.Data.SqlClient --version 5.1.5
```

### Step 6: Build the Solution
```bash
dotnet build
```

### Step 7: Apply Migrations
```bash
dotnet ef database update --project Taskify.Data --startup-project Taskify.Api --verbose
```

### Step 8: Run and Test
```bash
# Start the API
dotnet run --project Taskify.Api

# Test endpoints
# Swagger UI: http://localhost:5014/swagger
# Register: POST http://localhost:5014/api/users/register
# Login: POST http://localhost:5014/api/users/login
```

## Troubleshooting

### Error: "Login failed for user"
**Cause**: Your Azure AD account doesn't have permissions on the database.

**Solution**: Grant permissions via Query Editor (see Step 3 above)

### Error: "Cannot open server"
**Cause**: Firewall not configured or public access disabled.

**Solutions**:
1. Add your IP to firewall rules (see Step 2)
2. Enable public network access:
   ```bash
   az sql server update --resource-group taskify_Db --server taskifyserver --enable-public-network true
   ```

### Error: "ActiveDirectoryInteractive authentication is not supported"
**Cause**: Missing or outdated Microsoft.Data.SqlClient package.

**Solution**: Ensure you have Microsoft.Data.SqlClient 5.1.5+ installed

### Error: "The device code authentication flow failed"
**Solution**: Try different login methods:
```bash
az login --use-device-code
# OR
az login --allow-no-subscriptions
```

### Verify Your Login
```bash
# Check current account
az account show

# List available subscriptions
az account list --output table

# Set correct subscription if needed
az account set --subscription b5eddd23-de05-443d-ae9d-6d78398b8974
```

## Migration Success Checklist
- [ ] Logged into Azure CLI (`az login`)
- [ ] Firewall rule added for your IP
- [ ] Azure AD account has db_owner permissions on the database
- [ ] Microsoft.Data.SqlClient NuGet package installed
- [ ] Connection string updated in appsettings files
- [ ] Solution builds successfully
- [ ] EF migrations applied without errors
- [ ] API runs and connects to Azure SQL
- [ ] Can register and login users
- [ ] Can create and retrieve tasks

## Benefits of Active Directory Authentication
? **No password management** - Uses your Azure AD identity  
? **Better security** - Leverages Azure AD security features  
? **Centralized access control** - Manage permissions in Azure AD  
? **Audit trail** - All database access is logged with your identity  
? **Multi-factor authentication** - Inherits your Azure AD MFA settings

## Database Information
- **Database Name**: free-sql-db-9771280
- **Server**: taskifyserver.database.windows.net
- **Resource Group**: taskify_Db
- **Location**: Central India
- **Authentication**: Active Directory Default
- **Encryption**: TLS 1.2 (Encrypt=True)

## Next Steps After Migration
1. **Test all endpoints** - Register, login, create tasks
2. **Remove old LocalDB connection string** from code
3. **Add appsettings.Development.json to .gitignore**
4. **Configure CI/CD** - Use Managed Identity for production
5. **Enable auditing** - Track all database operations
6. **Set up alerts** - Monitor database performance and errors
7. **Configure backups** - Set retention policies

## Production Deployment
For production on Azure App Service:
1. Enable **Managed Identity** on your App Service
2. Grant the Managed Identity access to the database
3. Use this connection string:
```
Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;Encrypt=True;Authentication=Active Directory Managed Identity;
```

## Cost Information
Your database tier: **Free tier** (DTU-based)
- Storage: Up to 32 GB
- Backup retention: 7 days
- Cost: Free

## Support
If you encounter issues:
1. Check Azure Portal ? SQL database ? Monitoring ? Diagnostic logs
2. Review EF Core migration logs (--verbose flag)
3. Verify your Azure AD permissions
4. Check firewall rules are active

## Connection String Examples

**Development (current):**
```
Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;Authentication=Active Directory Default;
```

**Production (Managed Identity):**
```
Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;Encrypt=True;Authentication=Active Directory Managed Identity;
```

**Alternative (with username/password):**
```
Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;User ID=CloudSA4b483abd;Password=YOUR_PASSWORD;Encrypt=True;
