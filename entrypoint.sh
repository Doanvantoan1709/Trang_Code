#!/bin/bash
set -e

echo "=== Entrypoint Script Started ==="

# Create necessary directories
echo "Creating directories..."
mkdir -p /app/wwwroot/uploads
mkdir -p /app/uploads  
mkdir -p /app/Logs

# Set permissions
echo "Setting permissions..."
chmod -R 755 /app/wwwroot
chmod -R 755 /app/uploads
chmod -R 755 /app/Logs

# Create .keep files to ensure directories persist
touch /app/wwwroot/uploads/.keep
touch /app/uploads/.keep
touch /app/Logs/.keep

# Generate self-signed certificate for HTTPS
echo "=== Generating SSL Certificate ==="
if [ ! -f /app/aspnetapp.pfx ]; then
    echo "Creating self-signed certificate..."
    openssl req -x509 -newkey rsa:4096 -keyout /app/key.pem -out /app/cert.pem \
        -days 365 -nodes -subj "/C=VN/ST=HN/L=Hanoi/O=BaseProject/OU=IT/CN=62.146.236.71"
    
    openssl pkcs12 -export -out /app/aspnetapp.pfx -inkey /app/key.pem -in /app/cert.pem \
        -passout pass:YourPassword123
    
    chmod 644 /app/aspnetapp.pfx
    echo "Certificate created successfully"
else
    echo "Certificate already exists"
fi

echo "Directories created successfully:"
ls -la /app/ | head -10
echo "wwwroot contents:"
ls -la /app/wwwroot/

echo "=== Starting .NET Application ==="
# Start the application
exec dotnet BaseProjectNetCore.dll 