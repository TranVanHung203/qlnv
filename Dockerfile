# =========================
# Stage 1: Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy solution
COPY qlnv.sln ./

# copy từng project .csproj
COPY Contracts/Contracts.csproj Contracts/
COPY Service.Contracts/Service.Contracts.csproj Service.Contracts/
COPY qlnv.Presentation/qlnv.Presentation.csproj qlnv.Presentation/
COPY Service/Service.csproj Service/
COPY Repository/Repository.csproj Repository/
COPY Entities/Entities.csproj Entities/
COPY qlnv/qlnv.csproj qlnv/

# restore dependencies
RUN dotnet restore qlnv.sln

# copy toàn bộ source code
COPY . .

# publish web project (self-contained)
WORKDIR /src/qlnv.Presentation
RUN dotnet publish qlnv.Presentation.csproj -c Release -o /app/out --no-restore --self-contained false

# =========================
# Stage 2: Runtime
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "qlnv.Presentation.dll"]
