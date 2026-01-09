# Estágio 1: Build (Compilação)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de projeto e restaura as dependências
COPY ["BookStoreApi.csproj", "./"]
RUN dotnet restore "BookStoreApi.csproj"

# Copia todo o resto do código
COPY . .

# Compila para produção
RUN dotnet publish "BookStoreApi.csproj" -c Release -o /app/publish

# Estágio 2: Runtime (Execução - Imagem mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# Copia os arquivos compilados do estágio anterior
COPY --from=build /app/publish .

# Comando que inicia a API
ENTRYPOINT ["dotnet", "BookStoreApi.dll"]