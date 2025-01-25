#!/usr/bin/sh
echo Export Developer Certification to StreamingAssets Folder
destloc=$(find Assets/StreamingAssets -print -quit)
dotnet dev-certs https -ep ../$destloc/ca.crt --format Pem
