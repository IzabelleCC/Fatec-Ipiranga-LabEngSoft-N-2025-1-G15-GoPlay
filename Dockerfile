# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY *.sln .
COPY GoPlay_App/*.csproj ./GoPlay_App/
COPY GoPlay_Core/*.csproj ./GoPlay_Core/
COPY GoPlay_Infra/*.csproj ./GoPlay_Infra/

# Restaurar dependências
RUN dotnet restore

# Copiar o restante do código
COPY . .

# Publicar a aplicação
WORKDIR /src/GoPlay_App
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

CMD ASPNETCORE_URLS="http://*:$PORT" dotnet GoPlay_App.dll

