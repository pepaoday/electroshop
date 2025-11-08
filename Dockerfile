# Runtime only - app đã được build sẵn trên máy local
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published app từ thư mục publish (đã build sẵn)
COPY publish .

# Render.com và Cloud Run sẽ tự động set PORT environment variable
# ASP.NET Core sẽ tự động sử dụng PORT nếu được set
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_URLS=http://+:8080
ENV PORT=8080

ENTRYPOINT ["dotnet", "DoAnWebNC.dll"]

