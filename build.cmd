dotnet publish -r win-x64 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishReadyToRun=false
dotnet publish -r linux-x64 -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishReadyToRun=false
