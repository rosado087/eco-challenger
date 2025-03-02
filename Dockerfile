# Build Angular app
FROM node:23.9.0-slim AS build-angular
WORKDIR /app
COPY ClientApp/ ./ClientApp/
WORKDIR /app/ClientApp
RUN npm install
RUN npm run build --prod

# Build .NET Core app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-dotnet
WORKDIR /src
COPY ["EcoChallenger/EcoChallenger.csproj", "EcoChallenger/"]
RUN dotnet restore "EcoChallenger/EcoChallenger.csproj"
COPY . .
WORKDIR "/src/EcoChallenger"
RUN dotnet publish -c Release -o /app/publish

# Create final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build-dotnet /app/publish .
COPY --from=build-angular /app/ClientApp/dist/client-app/browser ./wwwroot
EXPOSE 8080
ENTRYPOINT ["dotnet", "EcoChallenger.dll"]
