# ---------- STAGE 1: Restore & Publish ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj để tối ưu cache
COPY ["MadeHuman_User/MadeHuman_User.csproj", "MadeHuman_User/"]
COPY ["Madehuman_Share/Madehuman_Share.csproj", "Madehuman_Share/"]

# Restore
RUN dotnet restore "MadeHuman_User/MadeHuman_User.csproj"

# Copy source
COPY MadeHuman_User/ MadeHuman_User/
COPY Madehuman_Share/ Madehuman_Share/

# Publish (Release)
WORKDIR /src/MadeHuman_User
RUN dotnet publish "MadeHuman_User.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- STAGE 2: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render sẽ set $PORT; Kestrel listen 0.0.0.0:$PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000

# Copy publish output
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "MadeHuman_User.dll"]

