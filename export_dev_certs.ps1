Write-Output "Export Developer Certification to StreamingAssets Folder"
dotnet dev-certs https -ep Assets\StreamingAssets\ca.crt --format Pem
