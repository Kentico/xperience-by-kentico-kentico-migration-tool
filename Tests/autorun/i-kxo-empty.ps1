#requires -RunAsAdministrator

<#
.Synopsis
    Installs KXO using CLI to desired location, installs DB and runs the site. Script also adds proper port to presentation url.
#>

param(
    # Path to wwwroot or other location for project installation
    [Parameter(Mandatory=$false)]
    [string] $InstallationPath = $null,

    # Project name - used as a folder, project & db name
    [Parameter(Mandatory=$false)]
    [string] $ProjectName = $null,

    # Template - choose which template to install from private nuget feed. Default is the latest
    # Use version name from https://dev.azure.com/kenticoxperience/CMS/_packaging?_a=package&feed=Kentico.Private&package=Kentico.Xperience.Templates&protocolType=NuGet&view=versions
    # For example 14.0.0-build-68541
    [Parameter(Mandatory=$false)]
	[string] $Template = "",

    # Database server
    [Parameter(Mandatory=$false)]
    [string] $DbServer = ".",

    # Path to a license file containing Kentico license for localhost. License will be imported automaticaly into database during db install
    [Parameter(Mandatory=$false)]    
	[string] $LicenseFilePath = $null,

    # Automatically confirm folder deletion
    [Parameter(Mandatory=$false)]
    [Alias('y')]
    [switch] $AutoConfirm = $true,

    [Parameter(Mandatory=$false)]
    [string] $NugetSource = "https://pkgs.dev.azure.com/kenticoxperience/CMS/_packaging/Kentico.Private/nuget/v3/index.json",

    [Parameter(Mandatory=$false)]
    [string] $CloudHashStringSalt = "",
	
    [Parameter(Mandatory=$false)]
    [string] $AdministratorPassword = "admin"
)


$ProjectPath = Join-Path $InstallationPath $ProjectName
$LicenseFilePath = Resolve-Path $LicenseFilePath
Push-Location

function Test-License {

    Write-Host "Checking licence file path '$LicenseFilePath'..." -ForegroundColor Cyan

    if (Test-Path $LicenseFilePath) {
        Write-Host "License file found..." -ForegroundColor Cyan
    } else {
        Write-Host "License file not found at '$LicenseFilePath'. Please provide a path to the license file using -LicenseFilePath parameter, or create a new one on default location '$LicenseFilePath'" -ForegroundColor Yellow
        Exit
    }

}

function Test-Directory {

    Write-Host "Preparing project directory..." -ForegroundColor Cyan

    if (Test-Path $ProjectPath) {
        if ($AutoConfirm -eq $true) {
            $deleteConfirmation = 'y'
        } else {
            Write-Host "Directory '$ProjectPath' is not empty. Do you want to clear it and reuse it? (Y/N)?: " -ForegroundColor Magenta -NoNewline
            $deleteConfirmation = Read-Host
        }

        if ($deleteConfirmation -eq 'y') {
            Write-Host "Deleting content of '$ProjectPath'..." -ForegroundColor Cyan
            try {
                Remove-Item $(Join-Path -Path $ProjectPath -ChildPath "*") -Recurse -ErrorAction stop
            }
            catch {
                Write-host "Directory content could not been deleted." -ForegroundColor Red
                Exit
            }

        } else {
            Exit
        }

    } else {
        Write-Host "Creating '$ProjectName' folder..." -ForegroundColor Cyan
        New-Item $ProjectPath -Type Directory | Out-Null
    }

    Write-Host "Project will be installed in '$ProjectPath'..." -ForegroundColor Cyan

}

function Update-Templates {

    Write-Host "Unistalling Kentico Xperience templates, if any..." -ForegroundColor Cyan
    $DotnetArgs = $(
    "new"
    "-u"
    "kentico.xperience.templates"
    )

    & "dotnet.exe" $DotnetArgs | Out-Null

    Write-Host "Installing Kentico Xperience templates..." -ForegroundColor Cyan
    if (!$Template) {
        $Template =  "kentico.xperience.templates"
        Write-Host "Latest template will be used..." -ForegroundColor Cyan
    } else {
        $Template =  "kentico.xperience.templates::$Template"
        Write-Host "$Template will be used..." -ForegroundColor Cyan
    }

    $DotnetArgs = $(
    "new"
    "-i"
    $Template
    "--nuget-source"
    $NugetSource
    )

     & "dotnet.exe" $DotnetArgs

}

function Install-Project {

    Set-Location -Path $ProjectPath

    $DotnetArgs = $(
        "new"
        "kentico-xperience-mvc"#"kentico-xperience-sample-mvc"
        )

    if ($CloudHashStringSalt) {
        $DotnetArgs += "--cloud"
        Write-Host "Project will be installed in a cloud version..." -ForegroundColor Cyan
    }

    Write-Host "Installing Kentico Xperience project into '$ProjectPath'..." -ForegroundColor Cyan
    'Y'|&"dotnet.exe" $DotnetArgs | Out-Null

}

function Install-Database {

    $DotnetArgs = $(
        "kentico-xperience-dbmanager"
        "-s"
        $DbServer
		"-a"
		$AdministratorPassword
        "--license-file"
        $LicenseFilePath
        "--database-name"
        $ProjectName
        "--recreate-existing-database"
        )
    
    if ($CloudHashStringSalt) {
        $DotnetArgs += $(
        "--hash-string-salt"
        $CloudHashStringSalt
        )
        Write-Host "Project will be installed in a cloud version..." -ForegroundColor Cyan
    }

    Write-Host "Installing database '$ProjectName'..." -ForegroundColor Cyan
     & "dotnet.exe" $DotnetArgs

}

function Start-Website {

    $applicationUrl = (Get-Content $(Join-Path -Path $ProjectPath -ChildPath "\Properties\launchSettings.json") | ConvertFrom-Json).iisSettings.iisExpress.applicationUrl

    Write-Host "Starting website..." -ForegroundColor Cyan
    Start-Process -FilePath cmd -verb runas -ArgumentList {/k dotnet.exe run} -WorkingDirectory $ProjectPath
    Start-Sleep -s 6
    Write-Host "Opening browser..." -ForegroundColor Green
    Start-Process $applicationUrl
    Pop-Location

}

Test-License
Test-Directory
Update-Templates
Install-Project
Install-Database
if (!$CloudHashStringSalt) {
    Start-Website
}
