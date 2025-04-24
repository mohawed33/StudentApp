#!/usr/bin/env bash

# Install .NET SDK
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 7.0
export PATH="$PATH:$HOME/.dotnet"

# Install MAUI workload
dotnet workload install maui

# Build the app
dotnet publish StudentApp/StudentApp.csproj -f net7.0-android -c Release

# Copy the APK to the output directory
mkdir -p $APPCENTER_OUTPUT_DIRECTORY
find StudentApp/bin/Release/net7.0-android -name "*-Signed.apk" -exec cp {} $APPCENTER_OUTPUT_DIRECTORY \;
