$command=$args[0]

if ($command -eq "migrations") {
    $migrationName=$args[1]
    dotnet ef migrations add $migrationName --project .\document.lib.ef\document.lib.ef.csproj --startup-project .\document.lib.web\document.lib.web.csproj
}
if ($command -eq "update"){
    dotnet ef database update --project .\document.lib.ef\document.lib.ef.csproj --startup-project .\document.lib.web\document.lib.web.csproj
}