FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Unify.Api/Unify.Api.csproj", "src/Unify.Api/"]
COPY ["src/Unify.Application/Unify.Application.csproj", "src/Unify.Application/"]
COPY ["src/Unify.Domain/Unify.Domain.csproj", "src/Unify.Domain/"]
COPY ["src/Unify.Infrastructure/Unify.Infrastructure.csproj", "src/Unify.Infrastructure/"]
RUN dotnet restore "./src/Unify.Api/./Unify.Api.csproj"
COPY . .
WORKDIR "/src/src/Unify.Api"
RUN dotnet build "./Unify.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Unify.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unify.Api.dll"]