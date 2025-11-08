@echo off
REM Script để ping Google với sitemap URL
REM Sử dụng: script-ping-google.bat YOUR_DOMAIN

if "%1"=="" (
    echo Vui long nhap domain: script-ping-google.bat your-domain.run.app
    exit /b 1
)

set DOMAIN=%1
set SITEMAP_URL=https://%DOMAIN%/sitemap.xml
set PING_URL=https://www.google.com/ping?sitemap=%SITEMAP_URL%

echo.
echo ========================================
echo Ping Google voi sitemap
echo ========================================
echo Domain: %DOMAIN%
echo Sitemap: %SITEMAP_URL%
echo Ping URL: %PING_URL%
echo.

echo Dang ping Google...
curl "%PING_URL%"

echo.
echo.
echo ========================================
echo Hoan thanh!
echo ========================================
echo.
echo Ban co the kiem tra trong Google Search Console
echo hoac tim kiem: site:%DOMAIN%
echo.
pause

