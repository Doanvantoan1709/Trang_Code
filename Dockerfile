FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
    
# Copy solution and restore as distinct layers
COPY BaseProjectNetCore.sln ./
COPY BaseProjectNetCore/BaseProjectNetCore.csproj BaseProjectNetCore/
COPY CommonHelper/CommonHelper.csproj CommonHelper/
COPY Model/BaseProject.Model.csproj Model/
COPY Repository/BaseProject.Repository.csproj Repository/
COPY Service/BaseProject.Service.csproj Service/
    
    # Restore dependencies
RUN dotnet restore "BaseProjectNetCore.sln"
    
    # Copy all source code
COPY . .
    
    # Build the project
RUN dotnet publish "BaseProjectNetCore/BaseProjectNetCore.csproj" -c Release -o /app/publish
    
    # -------------------------------------
    # STAGE 2: RUNTIME
    # -------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
    
    # Copy compiled app from build stage
COPY --from=build /app/publish .
    
    # Set environment variables if needed
ENV ASPNETCORE_URLS=http://+:7294 \
        DOTNET_RUNNING_IN_CONTAINER=true
    
    # Expose port
EXPOSE 7294  
    # Run the application
ENTRYPOINT ["dotnet", "BaseProjectNetCore.dll"]
    