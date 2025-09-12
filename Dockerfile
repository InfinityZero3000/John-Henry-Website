# Use the official .NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["JohnHenryFashionWeb.csproj", "."]
RUN dotnet restore "./JohnHenryFashionWeb.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "JohnHenryFashionWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JohnHenryFashionWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JohnHenryFashionWeb.dll"]
