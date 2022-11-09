#requires -RunAsAdministrator
# call example: .\run-test.ps1 -LicenseFilePath "c:\WORK\license_XbK.txt" -i $true

param(  
    # Path to wwwroot or other location for project installation
    [Parameter(Mandatory = $false)]
    [string] $InstallationPath = $null,

    # Project name - used as a folder, project & db name
    [Parameter(Mandatory = $false)]
    [string] $ProjectName = $null,

    [Parameter(Mandatory = $false)]
    [string] $Template = "",

    # Path to a license file containing Kentico license for localhost. License will be imported automaticaly into database during db install
    [Parameter(Mandatory = $false)]    
    [Alias('l')]
    [string] $LicenseFilePath = $null,

    [Parameter(Mandatory = $false)]    
    [Alias('i')]
    [string] $InstallXbK = $false
)

Write-Host $LicenseFilePath

function Get-ScriptDirectory {
    Split-Path -parent $PSCommandPath
}

$runDate = Get-Date -Format "yyyyMMdd_HH-mm"

$currentDir = Get-ScriptDirectory
$toolkitDir = Join-Path -Path $currentDir -ChildPath "../../Migration.Toolkit.CLI/"
$toolkitBuildDir = Join-Path -Path $currentDir -ChildPath "mt_build"
$commandOutputDir = Join-Path -Path $currentDir -ChildPath "run-$runDate/"
$commandOutputLog = Join-Path -Path $currentDir -ChildPath "run-$runDate/run.log"
$results = Join-Path -Path $currentDir -ChildPath "run-$runDate/results.log"

$installPath = $InstallationPath
if ($InstallationPath -eq $null -or $installPath -eq '') {
    $installPath = Join-Path -Path $currentDir -ChildPath "last-instance"
}

if ($ProjectName -eq $null -or $ProjectName -eq '') {
    $ProjectName = "XbK_Test"
}

[System.IO.Directory]::CreateDirectory($commandOutputDir) | Out-Null;

try {
    Write-Output "Set location $toolkitDir"
    set-location $toolkitDir   

    Write-Output "Starting transcript"
    Start-Transcript -Path $commandOutputLog -NoClobber -Append 

    # build toolkit
    Write-Output "dotnet publish -c Release -f net6.0 --self-contained false --runtime win-x64 -o '$toolkitBuildDir'"    
    dotnet publish -c Release -f net6.0 --self-contained false --runtime win-x64 -o "$toolkitBuildDir" 

    # install XbK
    if($InstallXbK -eq $true) {
        $installScrPath = Join-Path -Path $currentDir -ChildPath "i-kxo-empty.ps1"
        $installCmd = "$installScrPath -InstallationPath:""$installPath"" -ProjectName:""$ProjectName"" -LicenseFilePath:""$LicenseFilePath"" -Template:""$Template"""
        Write-Output $installCmd;
        Invoke-Expression $installCmd
    }

    set-location $toolkitBuildDir
    .\Migration.Toolkit.CLI.exe migrate --nowait --siteId 1 --contact-management --custom-modules --settings-keys --page-types --pages --forms --attachments --culture en-US  --sites --media-libraries --users | Out-Host # forcing transcript to pick up stream with Out-Host
        
    Write-Output "Reading log"

    # $iRegex = '( fail: )|( warn: )';
    $iRegex = '( fail: )';
    $hasFail = 0
    foreach ($line in Get-Content $commandOutputLog) {
        if ($line -match $iRegex) {
            $line | Add-Content -Path $results
            $hasFail = 1
        }
    }  
    
    if ($hasFail -eq 1) {
        Write-Host "ERRORS FOUND - review file $results"
    }
}
finally {
    Stop-Transcript
    set-location $currentDir
}
