#!/bin/bash

# Create necessary directories
mkdir -p /app/wwwroot/uploads
mkdir -p /app/uploads  
mkdir -p /app/Logs

# Set permissions
chmod -R 755 /app/wwwroot
chmod -R 755 /app/uploads
chmod -R 755 /app/Logs

# Create .keep files to ensure directories persist
touch /app/wwwroot/uploads/.keep
touch /app/uploads/.keep
touch /app/Logs/.keep

echo "Directories created successfully:"
ls -la /app/wwwroot/
ls -la /app/

# Start the application
exec dotnet BaseProjectNetCore.dll 