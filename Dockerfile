# ----- Build -----
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and restore as distinct layers
COPY *.sln ./
COPY MadeHuman_Server/*.csproj ./MadeHuman_Server/
COPY Madehuman_Share/*.csproj ./Madehuman_Share/
COPY MadeHuman_Admin/*.csproj ./MadeHuman_Admin/
COPY MadeHuman_User/*.csproj ./MadeHuman_User/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /app/MadeHuman_Server
RUN dotnet publish -c Release -o /out

# ----- Runtime -----
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "MadeHuman_Server.dll"]
