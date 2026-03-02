#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Matterway.Sales.Api/Matterway.Sales.Api.csproj", "src/Matterway.Sales.Api/"]
RUN dotnet restore "src/Matterway.Sales.Api/Matterway.Sales.Api.csproj"
COPY . .
WORKDIR "/src/src/Matterway.Sales.Api"
RUN dotnet build "Matterway.Sales.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Matterway.Sales.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ARG ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
COPY --from=publish /app/publish .
COPY src/Matterway.Sales.Api/Properties/ ./Properties/
ENTRYPOINT ["dotnet", "Matterway.Sales.Api.dll"]
