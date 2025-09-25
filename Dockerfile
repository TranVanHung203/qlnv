# =========================
# Stage 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj và restore
COPY *.sln .
COPY qlnv/*.csproj ./qlnv/
RUN dotnet restore

# copy toàn bộ code và build
COPY . .
WORKDIR /src/qlnv
RUN dotnet publish -c Release -o /app/out

# =========================
# Stage 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "qlnv.dll"]
