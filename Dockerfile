# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj trước
COPY qlnv.sln ./
COPY Entities/Entities.csproj Entities/
COPY Repository/Repository.csproj Repository/
COPY Service/Service.csproj Service/
COPY Service.Contracts/Service.Contracts.csproj Service.Contracts/
COPY Contracts/Contracts.csproj Contracts/
COPY qlnv.Presentation/qlnv.Presentation.csproj qlnv.Presentation/

# restore nuget
RUN dotnet restore qlnv.sln --disable-parallel

# copy toàn bộ source
COPY . .

RUN dotnet build qlnv.sln -c Release -o /app/build
RUN dotnet publish qlnv.sln -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "qlnv.Presentation.dll"]
