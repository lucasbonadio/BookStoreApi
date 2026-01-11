# Estágio 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ["src/BookStoreApi.csproj", "src/"]

RUN dotnet restore "src/BookStoreApi.csproj"

COPY . .

WORKDIR "/app/src"

RUN dotnet publish "BookStoreApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "BookStoreApi.dll"]