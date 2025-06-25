# ----- STAGE 1: Build -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["MadeHuman_Server/MadeHuman_Server.csproj", "MadeHuman_Server/"]
COPY ["Madehuman_Share/Madehuman_Share.csproj", "Madehuman_Share/"]
RUN dotnet restore "MadeHuman_Server/MadeHuman_Server.csproj"

# Copy full source code
COPY . .
WORKDIR "/src/MadeHuman_Server"

# Build and publish
RUN dotnet publish "MadeHuman_Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ----- STAGE 2: Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "MadeHuman_Server.dll"]
