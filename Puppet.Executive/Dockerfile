#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["Puppet.Executive/Puppet.Executive.csproj", "Puppet.Executive/"]
COPY ["Puppet.Common/Puppet.Common.csproj", "Puppet.Common/"]
COPY ["Puppet.Automation/Puppet.Automation.csproj", "Puppet.Automation/"]
RUN dotnet restore "Puppet.Executive/Puppet.Executive.csproj"
COPY . .
WORKDIR "/src/Puppet.Executive"
RUN dotnet build "Puppet.Executive.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Puppet.Executive.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Puppet.Executive.dll"]