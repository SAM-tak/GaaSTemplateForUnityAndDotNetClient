Write-Output "Export Developer Certification to StreamingAssets Folder"
$destloc = Get-ChildItem Assets\StreamingAssets
dotnet dev-certs https -ep $destloc\ca.crt --format Pem
