# ------------ Build stage ------------
    FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
    WORKDIR /src
    
    COPY BaseProjectNetCore.sln ./
    COPY BaseProjectNetCore/BaseProjectNetCore.csproj BaseProjectNetCore/
    COPY CommonHelper/CommonHelper.csproj CommonHelper/
    COPY Model/BaseProject.Model.csproj Model/
    COPY Repository/BaseProject.Repository.csproj Repository/
    COPY Service/BaseProject.Service.csproj Service/
    
    RUN dotnet restore "BaseProjectNetCore.sln"
    
    COPY . .
    RUN mkdir -p BaseProjectNetCore/wwwroot/uploads
    RUN dotnet publish "BaseProjectNetCore/BaseProjectNetCore.csproj" -c Release -o /app/publish
    
    # ------------ Runtime stage ------------
    FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
    WORKDIR /app
    
    COPY --from=build /app/publish .
    
    # Ensure the uploads folder exists
    RUN mkdir -p /app/wwwroot/uploads
    
    COPY entrypoint.sh /app/entrypoint.sh
    RUN chmod +x /app/entrypoint.sh
    
    ENV ASPNETCORE_URLS=http://+:7294;https://+:7295 \
        DOTNET_RUNNING_IN_CONTAINER=true \
        ASPNETCORE_ENVIRONMENT=Production \
        ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx \
        ASPNETCORE_Kestrel__Certificates__Default__Password=YourPassword123
    
    EXPOSE 7294 7295
    ENTRYPOINT ["/app/entrypoint.sh"]
    