FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy only the csproj file first
COPY api.csproj ./
RUN dotnet restore

# Copy source files (excluding bin, obj, etc.)
COPY *.cs ./
COPY Controllers/ ./Controllers/
COPY Database/ ./Database/
COPY Middleware/ ./Middleware/
COPY Models/ ./Models/
COPY Properties/ ./Properties/
COPY Repositories/ ./Repositories/
COPY Services/ ./Services/
COPY appsettings*.json ./

RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "api.dll"]
