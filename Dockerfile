# ----- STAGE 1: Build -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Chỉ copy các .csproj cần thiết để restore
COPY ["MadeHuman_Server/MadeHuman_Server.csproj", "MadeHuman_Server/"]
COPY ["Madehuman_Share/Madehuman_Share.csproj", "Madehuman_Share/"]

# Restore packages cho MadeHuman_Server (bao gồm cả Share)
RUN dotnet restore "MadeHuman_Server/MadeHuman_Server.csproj"

# Chỉ copy đúng thư mục cần thiết để tránh file trùng
COPY MadeHuman_Server/ MadeHuman_Server/
COPY Madehuman_Share/ Madehuman_Share/

WORKDIR /src/MadeHuman_Server

# Build và publish
RUN dotnet publish "MadeHuman_Server.csproj" -c Release -o /app/publish

# ----- STAGE 2: Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy output từ build stage
COPY --from=build /app/publish .

# Start the app
ENTRYPOINT ["dotnet", "MadeHuman_Server.dll"]
