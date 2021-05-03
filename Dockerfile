#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim-arm64v8 AS base
WORKDIR /app
EXPOSE 8099
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0.102-ca-patch-buster-slim-amd64 AS build
WORKDIR /src
RUN apt-get update && apt-get -y install nodejs npm
RUN npm install --save-dev @types/jquery
#COPY ["package-lock.json", "./"]
#RUN npm install

COPY ["AqualogicJumper.csproj", ""]
RUN dotnet restore "./AqualogicJumper.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "AqualogicJumper.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AqualogicJumper.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:8099
ENTRYPOINT ["dotnet", "AqualogicJumper.dll"]