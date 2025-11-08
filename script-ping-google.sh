#!/bin/bash

# Script để ping Google với sitemap URL
# Sử dụng: ./script-ping-google.sh your-domain.run.app

if [ -z "$1" ]; then
    echo "Vui lòng nhập domain: ./script-ping-google.sh your-domain.run.app"
    exit 1
fi

DOMAIN=$1
SITEMAP_URL="https://${DOMAIN}/sitemap.xml"
PING_URL="https://www.google.com/ping?sitemap=${SITEMAP_URL}"

echo ""
echo "========================================"
echo "Ping Google với sitemap"
echo "========================================"
echo "Domain: $DOMAIN"
echo "Sitemap: $SITEMAP_URL"
echo "Ping URL: $PING_URL"
echo ""

echo "Đang ping Google..."
curl "$PING_URL"

echo ""
echo ""
echo "========================================"
echo "Hoàn thành!"
echo "========================================"
echo ""
echo "Bạn có thể kiểm tra trong Google Search Console"
echo "hoặc tìm kiếm: site:$DOMAIN"
echo ""

