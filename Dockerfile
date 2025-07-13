# ----- STAGE 1: Build -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Chỉ copy các file .csproj cần thiết để restore
COPY ["MadeHuman_Server/MadeHuman_Server.csproj", "MadeHuman_Server/"]
COPY ["Madehuman_Share/Madehuman_Share.csproj", "Madehuman_Share/"]

# Khôi phục các dependency
RUN dotnet restore "MadeHuman_Server/MadeHuman_Server.csproj"

# Copy toàn bộ source code còn lại
COPY MadeHuman_Server/ MadeHuman_Server/
COPY Madehuman_Share/ Madehuman_Share/

WORKDIR /src/MadeHuman_Server

# Build và publish ứng dụng
RUN dotnet publish "MadeHuman_Server.csproj" -c Release -o /app/publish

# ----- STAGE 2: Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80

# Copy các file đã publish từ STAGE 1
COPY --from=build /app/publish .

# Copy cấu hình OAuth (dành cho Google Drive)
COPY MadeHuman_Server/Data/credentials_oauth.json /app/Data/credentials_oauth.json
COPY MadeHuman_Server/Data/token.json /app/Data/token.json

# (Tuỳ chọn) Copy thêm credentials.json nếu vẫn hỗ trợ Service Account
COPY MadeHuman_Server/Data/credentials.json /app/Data/credentials.json

# Khởi động ứng dụng
ENTRYPOINT ["dotnet", "MadeHuman_Server.dll"]
