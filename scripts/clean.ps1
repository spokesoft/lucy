# Remove directories matching gitignore patterns
Get-ChildItem -Path . -Recurse -Directory |
    Where-Object { $_.Name -imatch '^(bin|obj|artifacts)$' } |
    Remove-Item -Recurse -Force

# Remove files matching gitignore patterns
Get-ChildItem -Path . -Recurse -File |
    Where-Object {
        $_.Extension -eq '.db' -or
        $_.Name -like '*.db-shm' -or
        $_.Name -like '*.db-wal'
    } |
    Remove-Item -Force
