# ---------- STAGE 1: Restore & Publish ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj để tối ưu cache
COPY ["MadeHuman_Server/MadeHuman_Server.csproj", "MadeHuman_Server/"]
COPY ["Madehuman_Share/Madehuman_Share.csproj", "Madehuman_Share/"]
COPY ["MadeHuman_User/MadeHuman_User.csproj", "MadeHuman_User/"]

# Restore riêng từng project
RUN dotnet restore "MadeHuman_Server/MadeHuman_Server.csproj"
RUN dotnet restore "MadeHuman_User/MadeHuman_User.csproj"

# Copy toàn bộ source
COPY MadeHuman_Server/ MadeHuman_Server/
COPY Madehuman_Share/ Madehuman_Share/
COPY MadeHuman_User/ MadeHuman_User/

# Publish từng project (Release)
WORKDIR /src/MadeHuman_Server
RUN dotnet publish "MadeHuman_Server.csproj" -c Release -o /app/publish/server /p:UseAppHost=false

WORKDIR /src/MadeHuman_User
RUN dotnet publish "MadeHuman_User.csproj" -c Release -o /app/publish/user /p:UseAppHost=false


# ---------- STAGE 2: Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Render cung cấp PORT (ví dụ 10000). Phải bind 0.0.0.0:$PORT
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
EXPOSE 10000

# Copy output đã publish
COPY --from=build /app/publish/server ./server
COPY --from=build /app/publish/user ./user

# Copy cấu hình OAuth (dành cho Google Drive)
COPY MadeHuman_Server/Data/credentials_oauth.json /app/Data/credentials_oauth.json
COPY MadeHuman_Server/Data/token.json             /app/Data/token.json

# (Tuỳ chọn) Copy thêm credentials.json nếu vẫn hỗ trợ Service Account
COPY MadeHuman_Server/Data/credentials.json       /app/Data/credentials.json

# Mặc định chạy Server; có thể override bằng Start Command của Render để chạy User
WORKDIR /app/server
ENTRYPOINT ["dotnet", "MadeHuman_Server.dll"]




