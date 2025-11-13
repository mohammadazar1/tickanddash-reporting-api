# Script to add IConfiguration constructor to all DAL classes
$dalPath = "TickAndDash\TickAndDashDAL\DAL"
$files = Get-ChildItem -Path $dalPath -Filter "*DAL.cs" -Exclude "BaseDAL.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $fileName = $file.Name
    
    # Skip if already has IConfiguration constructor
    if ($content -match "public.*DAL\(IConfiguration") {
        Write-Host "Skipping $fileName - already has IConfiguration constructor"
        continue
    }
    
    # Get class name
    if ($content -match "public class (\w+DAL)") {
        $className = $matches[1]
        
        # Check if it has using Microsoft.Extensions.Configuration
        $hasUsing = $content -match "using Microsoft\.Extensions\.Configuration"
        
        # Add using if not present
        if (-not $hasUsing) {
            $content = $content -replace "(using.*?;)", "`$1`r`nusing Microsoft.Extensions.Configuration;"
        }
        
        # Find the class declaration line
        if ($content -match "(public class $className : BaseDAL[^\r\n]*)") {
            $classLine = $matches[1]
            
            # Add constructor after class declaration
            $constructor = @"

        public $className(IConfiguration configuration) : base(configuration)
        {
        }

"@
            
            # Insert constructor after class declaration and opening brace
            $content = $content -replace "($classLine\s*\{)", "`$1`r`n$constructor"
            
            # Write back to file
            Set-Content -Path $file.FullName -Value $content -NoNewline
            Write-Host "Updated $fileName"
        }
    }
}

Write-Host "Done!"



