# --- Build  ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src


COPY EdgeOpcClient.csproj ./
RUN dotnet restore


COPY . .
RUN dotnet publish -c Release -o /app/publish

# --- Runtime  ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# API port
EXPOSE 5000
ENV ASPNETCORE_URLS=http://0.0.0.0:5000

ENTRYPOINT ["dotnet", "EdgeOpcClient.dll"]
