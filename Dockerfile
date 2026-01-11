# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 1. Copia APENAS o csproj primeiro (para cachear as dependências)
# Note que agora incluímos o caminho "src/BookStoreApi/"
COPY ["src/BookStoreApi/BookStoreApi.csproj", "src/BookStoreApi/"]

# 2. Restaura as dependências
RUN dotnet restore "src/BookStoreApi/BookStoreApi.csproj"

# 3. Copia todo o resto do código da solução
COPY . .

# 4. Define o diretório de trabalho para a pasta do projeto antes de publicar
WORKDIR "/app/src/BookStoreApi"

# 5. Compila para produção
RUN dotnet publish "BookStoreApi.csproj" -c Release -o /app/publish

# Estágio 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia os arquivos compilados do estágio anterior
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BookStoreApi.dll"]