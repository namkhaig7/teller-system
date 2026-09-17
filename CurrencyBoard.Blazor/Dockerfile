FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["CurrencyBoard.Blazor/CurrencyBoard.Blazor.csproj", "CurrencyBoard.Blazor/"]
COPY ["BankServer.Shared/BankServer.Shared.csproj", "BankServer.Shared/"]
RUN dotnet restore "CurrencyBoard.Blazor/CurrencyBoard.Blazor.csproj"
COPY CurrencyBoard.Blazor/ CurrencyBoard.Blazor/
COPY BankServer.Shared/ BankServer.Shared/
WORKDIR /src/CurrencyBoard.Blazor
RUN dotnet publish "CurrencyBoard.Blazor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CurrencyBoard.Blazor.dll"]
