param([string]$connection)
dotnet ef dbcontext scaffold $connection Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --context-dir Context --context K11Context --data-annotations --force #--use-database-names

# Remove connection string
$workDir = $MyInvocation.MyCommand.Path
$workDir = [System.IO.Path]::GetDirectoryName($workDir)
$contextPath = "$workDir\Context\K11Context.cs"
$contextText = [System.IO.File]::ReadAllText($contextPath)
$result = $contextText -replace '.*optionsBuilder.UseSqlServer.*', '';
$result = $result -replace '.*protected override void OnConfiguring.*', '';
$result = $result -replace '.*#warning To protect potentially sensitive information in your connection string, .*', '';
[System.IO.File]::WriteAllText($contextPath, $result)

# ./genModel.ps1 -connection "Data Source=.;Initial Catalog=Kentico11_DG;Integrated Security=True;Persist Security Info=False;Connect Timeout=60;Encrypt=False;Current Language=English;"