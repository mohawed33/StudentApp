@echo off
echo Building StudentApp APK...

REM Create keystore if it doesn't exist
if not exist studentapp.keystore (
    echo Creating keystore...
    keytool -genkey -v -keystore studentapp.keystore -alias studentapp -keyalg RSA -keysize 2048 -validity 10000 -storepass studentapp123 -keypass studentapp123 -dname "CN=StudentApp, OU=Development, O=SchoolTracker, L=City, S=State, C=SA"
)

REM Build and publish the APK
dotnet publish -f net7.0-android -c Release -p:AndroidKeyStore=true -p:AndroidSigningKeyStore=studentapp.keystore -p:AndroidSigningStorePass=studentapp123 -p:AndroidSigningKeyAlias=studentapp -p:AndroidSigningKeyPass=studentapp123

echo APK build complete!
echo The APK file is located at: %CD%\bin\Release\net7.0-android\publish\
pause
