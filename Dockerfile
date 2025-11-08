# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Render.com và Cloud Run sẽ tự động set PORT environment variable
# ASP.NET Core sẽ tự động sử dụng PORT nếu được set
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV PORT=8080

ENTRYPOINT ["dotnet", "DoAnWebNC.dll"]

