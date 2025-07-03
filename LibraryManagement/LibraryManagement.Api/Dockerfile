# Use the official .NET SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and restore as distinct layers
COPY LibraryManagement.sln ./
COPY LibraryManagement.Api/ ./LibraryManagement.Api/
COPY LibraryManagement.Core/ ./LibraryManagement.Core/
COPY LibraryManagement.Data/ ./LibraryManagement.Data/
COPY LibraryManagement.Books/ ./LibraryManagement.Books/
COPY LibraryManagement.Users/ ./LibraryManagement.Users/

# Restore dependencies
RUN dotnet restore LibraryManagement.Api/LibraryManagement.Api.csproj

# Build and publish
RUN dotnet publish LibraryManagement.Api/LibraryManagement.Api.csproj -c Release -o /app/publish

# Use the official .NET runtime image for the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LibraryManagement.Api.dll"]