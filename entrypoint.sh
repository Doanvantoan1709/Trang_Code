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

echo "Directories created successfully:"
ls -la /app/ | head -10
echo "wwwroot contents:"
ls -la /app/wwwroot/

echo "=== Starting .NET Application ==="
# Start the application
exec dotnet BaseProjectNetCore.dll 