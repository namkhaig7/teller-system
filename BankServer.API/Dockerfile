FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["BankServer.API/BankServer.API.csproj", "BankServer.API/"]
COPY ["BankServer.Shared/BankServer.Shared.csproj", "BankServer.Shared/"]
RUN dotnet restore "BankServer.API/BankServer.API.csproj"
COPY BankServer.API/ BankServer.API/
COPY BankServer.Shared/ BankServer.Shared/
WORKDIR /src/BankServer.API
RUN dotnet publish "BankServer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BankServer.API.dll"]
