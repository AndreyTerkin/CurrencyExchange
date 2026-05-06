FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FinanceService/FinanceService/FinanceService.csproj", "FinanceService/FinanceService/"]
RUN dotnet restore "FinanceService/FinanceService/FinanceService.csproj"
COPY . .
WORKDIR "/src/FinanceService/FinanceService"
RUN dotnet publish "FinanceService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FinanceService.dll"]
