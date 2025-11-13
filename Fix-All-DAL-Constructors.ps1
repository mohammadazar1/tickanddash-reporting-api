# Script to add IConfiguration constructor to all DAL classes
$dalPath = "TickAndDash\TickAndDashDAL\DAL"
$files = Get-ChildItem -Path $dalPath -Filter "*DAL.cs" -Exclude "BaseDAL.cs"

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw -Encoding UTF8
    $fileName = $file.Name
    
    # Skip if already has IConfiguration constructor
    if ($content -match "public.*DAL\(IConfiguration") {
        Write-Host "✓ $fileName - already has IConfiguration constructor"
        continue
    }
    
    # Get class name
    if ($content -match "public class (\w+DAL)") {
        $className = $matches[1]
        
        # Add using Microsoft.Extensions.Configuration if not present
        if ($content -notmatch "using Microsoft\.Extensions\.Configuration") {
            # Find the last using statement
            if ($content -match "(using[^;]+;[\r\n]+)(namespace)") {
                $content = $content -replace "($matches[1])", "`$1using Microsoft.Extensions.Configuration;`r`n"
            }
        }
        
        # Find class declaration and add constructor
        # Pattern 1: public class XDAL : BaseDAL, IXDAL { (no constructor)
        if ($content -match "(public class $className : BaseDAL[^\r\n]*\s*\{)") {
            $classDecl = $matches[1]
            $constructor = @"

        public $className(IConfiguration configuration) : base(configuration)
        {
        }

"@
            $content = $content -replace "($classDecl)", "`$1$constructor"
        }
        # Pattern 2: public class XDAL : BaseDAL, IXDAL { public XDAL() : base() { }
        elseif ($content -match "(public class $className : BaseDAL[^\r\n]*\s*\{[^\}]*public $className\(\) : base\(\) \{ \})") {
            $oldConstructor = $matches[1]
            $newConstructor = $oldConstructor -replace "public $className\(\) : base\(\) \{ \}", "public $className(IConfiguration configuration) : base(configuration) { }"
            $content = $content -replace [regex]::Escape($oldConstructor), $newConstructor
        }
        # Pattern 3: public class XDAL : BaseDAL, IXDAL { public XDAL() { }
        elseif ($content -match "(public class $className : BaseDAL[^\r\n]*\s*\{[^\}]*public $className\(\) \{)") {
            $oldConstructor = $matches[1]
            $newConstructor = $oldConstructor -replace "public $className\(\) \{", "public $className(IConfiguration configuration) : base(configuration) {"
            $content = $content -replace [regex]::Escape($oldConstructor), $newConstructor
        }
        
        # Write back to file
        [System.IO.File]::WriteAllText($file.FullName, $content, [System.Text.Encoding]::UTF8)
        Write-Host "✓ Updated $fileName"
    }
    else {
        Write-Host "✗ Could not find class in $fileName"
    }
}

Write-Host "`nDone!"

