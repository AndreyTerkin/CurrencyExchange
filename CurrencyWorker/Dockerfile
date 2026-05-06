FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CurrencyWorker/CurrencyWorker.csproj", "CurrencyWorker/"]
RUN dotnet restore "CurrencyWorker/CurrencyWorker.csproj"
COPY . .
WORKDIR "/src/CurrencyWorker"
RUN dotnet publish "CurrencyWorker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CurrencyWorker.dll"]
