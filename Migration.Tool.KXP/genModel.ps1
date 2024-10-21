﻿param([string]$connection)
dotnet ef dbcontext scaffold $connection Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Context --context KxpContext --data-annotations --force # --use-database-names

# Remove connection string
#$workDir = $MyInvocation.MyCommand.Path
#$workDir = [System.IO.Path]::GetDirectoryName($workDir)
#$contextPath = "$workDir\Context\KxpContext.cs"
#$contextText = [System.IO.File]::ReadAllText($contextPath)
#$result = $contextText -replace '(?i-mn).*if \(\!optionsBuilder\.IsConfigured\)[\s\S]*?\}','';
#$result = $result -replace '(?i-mn).*protected override void OnConfiguring[\s\S]*?\}','';
#[System.IO.File]::WriteAllText($contextPath, $result)

# ./genModel.ps1 -connection "Data Source=.;Initial Catalog=XK28_4_1_BP;Integrated Security=True;Persist Security Info=False;Connect Timeout=60;Encrypt=False;Current Language=English;"