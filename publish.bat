@echo off
REM This file automatically deploys the assemblies to my Raspberry Pi.
REM You'll need to set up SSH key authentication and make changes
REM specific to your environment.
REM

echo Deploying Executive...
scp pi@automation:/home/pi/executive/appsettings.json ./
pushd Puppet.Executive
dotnet publish -r linux-arm -c Release
ssh pi@automation kill $(pidof Puppet.Executive)
ssh pi@automation rm /home/pi/executive/*
scp ./bin/Release/netcoreapp3.1/linux-arm/publish/* pi@automation:/home/pi/executive
popd 
scp ./appsettings.json pi@automation:/home/pi/executive/
del appsettings.json 
ssh pi@automation chmod 755 /home/pi/executive/Puppet.Executive

echo Deploying Automation Handlers...
pushd Puppet.Automation
dotnet publish -r linux-arm -c Release
scp ./bin/Release/netcoreapp3.1/linux-arm/publish/Puppet.Automation.* pi@automation:/home/pi/executive
scp ./devicemap.json pi@automation:/home/pi/executive
popd 

echo Rebooting!
ssh pi@automation sudo reboot