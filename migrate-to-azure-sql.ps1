# Azure SQL Migration Script with Active Directory Authentication
# This script will migrate your database to Azure SQL using Azure AD authentication

Write-Host "====================================" -ForegroundColor Cyan
Write-Host "Azure SQL Migration Script" -ForegroundColor Cyan
Write-Host "Database: free-sql-db-9771280" -ForegroundColor Cyan
Write-Host "Auth: Active Directory Default" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Configuration
$resourceGroup = "taskify_Db"
$serverName = "taskifyserver"
$databaseName = "free-sql-db-9771280"
$subscriptionId = "b5eddd23-de05-443d-ae9d-6d78398b8974"

Write-Host "[Step 1/7] Logging into Azure..." -ForegroundColor Yellow
az login

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Azure login failed" -ForegroundColor Red
    exit 1
}
Write-Host "? Logged in successfully" -ForegroundColor Green

Write-Host ""
Write-Host "[Step 2/7] Setting subscription..." -ForegroundColor Yellow
az account set --subscription $subscriptionId
Write-Host "? Subscription set" -ForegroundColor Green

Write-Host ""
Write-Host "[Step 3/7] Enabling public network access..." -ForegroundColor Yellow
az sql server update `
    --resource-group $resourceGroup `
    --name $serverName `
    --enable-public-network true
Write-Host "? Public network access enabled" -ForegroundColor Green

Write-Host ""
Write-Host "[Step 4/7] Adding firewall rule for your IP..." -ForegroundColor Yellow
$myIp = (Invoke-WebRequest -Uri "https://api.ipify.org" -UseBasicParsing).Content
Write-Host "Your IP: $myIp" -ForegroundColor Cyan

az sql server firewall-rule create `
    --resource-group $resourceGroup `
    --server $serverName `
    --name "AllowMyIP-$myIp" `
    --start-ip-address $myIp `
    --end-ip-address $myIp `
    2>$null

Write-Host "? Firewall rule added" -ForegroundColor Green

Write-Host ""
Write-Host "[Step 5/7] Verifying database exists..." -ForegroundColor Yellow
$dbCheck = az sql db show `
    --resource-group $resourceGroup `
    --server $serverName `
    --name $databaseName `
    2>$null

if ($dbCheck) {
    Write-Host "? Database '$databaseName' exists" -ForegroundColor Green
} else {
    Write-Host "? Database '$databaseName' not found" -ForegroundColor Red
    Write-Host "Please create the database in Azure Portal first" -ForegroundColor Yellow
    exit 1
}

Write-Host ""
Write-Host "[Step 6/7] Building solution..." -ForegroundColor Yellow
dotnet build --no-incremental

if ($LASTEXITCODE -ne 0) {
    Write-Host "? Build failed" -ForegroundColor Red
    exit 1
}
Write-Host "? Build successful" -ForegroundColor Green

Write-Host ""
Write-Host "[Step 7/7] Applying EF Core migrations..." -ForegroundColor Yellow
Write-Host "This will use Azure AD authentication from your logged-in account" -ForegroundColor Cyan
Write-Host ""

dotnet ef database update --project Taskify.Data --startup-project Taskify.Api --verbose

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "====================================" -ForegroundColor Green
    Write-Host "? Migration completed successfully!" -ForegroundColor Green
    Write-Host "====================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Your database is now ready!" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Next steps:" -ForegroundColor Yellow
    Write-Host "  1. Run API: dotnet run --project Taskify.Api" -ForegroundColor White
    Write-Host "  2. Test endpoints via Swagger: http://localhost:5014/swagger" -ForegroundColor White
    Write-Host "  3. Register user: POST /api/users/register" -ForegroundColor White
    Write-Host "  4. Login: POST /api/users/login" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "====================================" -ForegroundColor Red
    Write-Host "? Migration failed" -ForegroundColor Red
    Write-Host "====================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Common issues:" -ForegroundColor Yellow
    Write-Host "  1. Make sure you're logged into Azure with correct permissions" -ForegroundColor White
    Write-Host "  2. Verify your Azure AD user has db_owner role on the database" -ForegroundColor White
    Write-Host "  3. Check if firewall allows your IP" -ForegroundColor White
    Write-Host "  4. Try running: az login --allow-no-subscriptions" -ForegroundColor White
    Write-Host ""
    Write-Host "To grant yourself database permissions, run in Azure Portal Query Editor:" -ForegroundColor Yellow
    Write-Host "  CREATE USER [your-email@domain.com] FROM EXTERNAL PROVIDER;" -ForegroundColor Cyan
    Write-Host "  ALTER ROLE db_owner ADD MEMBER [your-email@domain.com];" -ForegroundColor Cyan
    exit 1
}
