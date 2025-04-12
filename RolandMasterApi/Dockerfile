FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["RolandMasterApi.csproj", "./"]
RUN dotnet restore "./RolandMasterApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "RolandMasterApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RolandMasterApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Define health check para Docker
HEALTHCHECK --interval=30s --timeout=3s --retries=3 CMD curl --fail http://localhost:80/swagger || exit 1

ENTRYPOINT ["dotnet", "RolandMasterApi.dll"]
