# Use the .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0.401 AS build-env
WORKDIR /app

# Copy the entire solution file
COPY ./WhiteLagoon.sln ./
COPY ./WhiteLagoon.Application/*.csproj ./WhiteLagoon.Application/
COPY ./WhiteLagoon.Domain/*.csproj ./WhiteLagoon.Domain/
COPY ./WhiteLagoon.Infrastructure/*.csproj ./WhiteLagoon.Infrastructure/
COPY ./WhiteLagoon.Web/*.csproj ./WhiteLagoon.Web/

# Restore dependencies
RUN dotnet restore ./WhiteLagoon.Web/WhiteLagoon.Web.csproj

# Copy the entire application source code
COPY . .

# Publish the application
RUN dotnet publish -c Release -o out ./WhiteLagoon.Web/WhiteLagoon.Web.csproj

# Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final-env
WORKDIR /app

# Copy the published output to the final image
COPY --from=build-env /app/out ./

# Set the entry point for the application
ENTRYPOINT ["dotnet", "WhiteLagoon.Web.dll"]
