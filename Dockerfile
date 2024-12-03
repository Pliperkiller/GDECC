FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["MigrationAPI.csproj", "."]
RUN dotnet restore "./MigrationAPI.csproj"

COPY . .

WORKDIR "/src/."
RUN dotnet build "./MigrationAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MigrationAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Copiar los archivos SQL a la ruta deseada dentro del contenedor
COPY ./Queries/SQL_ResetDB.sql /app/Queries/SQL_ResetDB.sql
COPY ./Queries/SQL_QuarterKpiIndicators.sql /app/Queries/SQL_QuarterKpiIndicators.sql
COPY ./Queries/SQL_DepartmentKpiHiredDescriptor.sql /app/Queries/SQL_DepartmentKpiHiredDescriptor.sql

ENTRYPOINT ["dotnet", "MigrationAPI.dll"]
