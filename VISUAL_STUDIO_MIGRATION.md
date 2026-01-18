# Azure SQL Migration - Visual Studio Method

## Quick Setup (No Azure CLI Required)

Since you're using Visual Studio and already logged into Azure Portal, you can migrate directly using EF Core tools.

### Step 1: Verify You're Logged into Azure in Visual Studio
1. Open Visual Studio
2. Go to **Tools** ? **Options** ? **Azure Service Authentication**
3. Make sure you're signed in with your account (bhargavi.mc@outlook.com)

### Step 2: Grant Database Permissions

You need to grant your Azure AD account permissions on the database.

**Via Azure Portal Query Editor:**
1. Go to Azure Portal: https://portal.azure.com
2. Navigate to: **SQL databases** ? **free-sql-db-9771280**
3. Click **Query editor (preview)** in the left menu
4. Sign in with your Azure AD account
5. Run this SQL command:

```sql
CREATE USER [bhargavi.mc@outlook.com] FROM EXTERNAL PROVIDER;
ALTER ROLE db_owner ADD MEMBER [bhargavi.mc@outlook.com];
GO
```

### Step 3: Configure Firewall

**Via Azure Portal:**
1. Go to: **SQL servers** ? **taskifyserver**
2. Click **Networking** in the left menu under Security
3. Under **Public network access**, select **Selected networks**
4. Click **+ Add your client IPv4 address** (your current IP will be auto-detected)
5. Click **Save**

### Step 4: Run Migrations from Visual Studio

**Option A: Package Manager Console**
1. In Visual Studio, go to **Tools** ? **NuGet Package Manager** ? **Package Manager Console**
2. Run these commands:

```powershell
# Build the solution
dotnet build

# Apply migrations
Update-Database -Project Taskify.Data -StartupProject Taskify.Api -Verbose
```

**Option B: Command Line (from solution directory)**
```bash
# Build
dotnet build

# Apply migrations
dotnet ef database update --project Taskify.Data --startup-project Taskify.Api --verbose
```

### Step 5: Test the Connection

Run your API:
```bash
dotnet run --project Taskify.Api
```

Then test:
- Swagger: http://localhost:5014/swagger
- Register: POST /api/users/register
- Login: POST /api/users/login

## Troubleshooting

### Error: "Login failed" or "Cannot authenticate"

**Solution 1: Use SQL Authentication instead**
If Active Directory authentication is causing issues, you can use SQL authentication:

1. Get your SQL admin password from Azure Portal
2. Update `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=tcp:taskifyserver.database.windows.net,1433;Initial Catalog=free-sql-db-9771280;User ID=CloudSA4b483abd;Password=YOUR_PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

3. Re-run migrations

**Solution 2: Grant permissions via Azure Portal**
1. Go to the database in Azure Portal
2. Query editor
3. Run the CREATE USER and ALTER ROLE commands (see Step 2)

### Error: "Cannot open server"

**Cause**: Firewall blocking your IP

**Solution**: Add your IP to firewall (see Step 3)

### Error: "Database does not exist"

**Cause**: Database not created yet

**Solution**: Create the database in Azure Portal:
1. Go to SQL Server: **taskifyserver**
2. Click **+ Create database**
3. Name: **free-sql-db-9771280** (or use existing name)
4. Pricing tier: **Basic** (cheapest)
5. Click **Create**

## Verification Steps

After migration, verify:

1. **Check tables exist:**
   - Go to Azure Portal ? Database ? Query editor
   - Run: `SELECT * FROM INFORMATION_SCHEMA.TABLES`
   - You should see: Users, Tasks, __EFMigrationsHistory

2. **Test API:**
   ```bash
   # Register a user
   curl -X POST http://localhost:5014/api/users/register \
     -H "Content-Type: application/json" \
     -d '{"userEmail":"test@example.com","password":"Test123!","role":"User"}'
   
   # Login
   curl -X POST http://localhost:5014/api/users/login \
     -H "Content-Type: application/json" \
     -d '{"userEmail":"test@example.com","password":"Test123!"}'
   ```

## Configuration Summary

Your current setup:
- **Connection String**: Using Active Directory Default authentication
- **Server**: taskifyserver.database.windows.net
- **Database**: free-sql-db-9771280
- **Auth Method**: Azure AD (your Visual Studio account)

Files updated:
- ? `Taskify.Api/appsettings.json`
- ? `Taskify.Api/appsettings.Development.json`
- ? `Taskify.Api/Taskify.Api.csproj` (added Microsoft.Data.SqlClient)

## Next Steps

1. Run migrations (Step 4 above)
2. Test the API (Step 5)
3. Deploy to Azure (optional):
   - Right-click Taskify.Api project ? Publish
   - Choose Azure App Service
   - Use Managed Identity for authentication

## Need Help?

If you continue to have issues:
1. Check Visual Studio's Azure Account: Tools ? Options ? Azure Service Authentication
2. Verify firewall rules in Azure Portal
3. Try SQL authentication instead of Active Directory (see Troubleshooting above)
4. Check the database exists and is in the correct resource group
