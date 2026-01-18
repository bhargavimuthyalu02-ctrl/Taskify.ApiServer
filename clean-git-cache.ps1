# Clean Git Cache Script
# This will remove all cached files from git tracking and re-add them according to .gitignore

Write-Host "Cleaning Git cache to respect .gitignore..." -ForegroundColor Cyan

# Remove all files from Git's cache
git rm -r --cached .

# Re-add all files (respecting .gitignore)
git add .

Write-Host ""
Write-Host "Done! Files have been cleaned from cache." -ForegroundColor Green
Write-Host ""
Write-Host "Files that should no longer be tracked:" -ForegroundColor Yellow
Write-Host "  - bin/ and obj/ directories" -ForegroundColor Gray
Write-Host "  - *.dll, *.pdb, *.exe files" -ForegroundColor Gray
Write-Host "  - *.cache files" -ForegroundColor Gray
Write-Host "  - appsettings.Development.json" -ForegroundColor Gray
Write-Host "  - SQLQuery*.sql files" -ForegroundColor Gray
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "  1. Review the changes: git status" -ForegroundColor White
Write-Host "  2. Commit the changes: git commit -m 'Update .gitignore and remove cached files'" -ForegroundColor White
Write-Host "  3. Push to remote: git push" -ForegroundColor White
