
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GoPlay_APP/GoPlay_App.csproj", "GoPlay_APP/"]
COPY ["GoPlay_Core/GoPlay_Core.csproj", "GoPlay_Core/"]
COPY ["GoPlay_Infra/GoPlay_Infra.csproj", "GoPlay_Infra/"]
RUN dotnet restore "./GoPlay_APP/GoPlay_App.csproj"
COPY . .
WORKDIR "/src/GoPlay_APP"
RUN dotnet build "./GoPlay_App.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GoPlay_App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GoPlay_App.dll"]