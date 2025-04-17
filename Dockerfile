FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KoineCrm.WebApi/KoineCrm.WebApi.csproj", "KoineCrm.WebApi/"]
COPY ["KoineCrm.Application/KoineCrm.Application.csproj", "KoineCrm.Application/"]
COPY ["KoineCrm.Core/KoineCrm.Core.csproj", "KoineCrm.Core/"]
RUN dotnet restore "KoineCrm.WebApi/KoineCrm.WebApi.csproj"
COPY . .
WORKDIR "/src/KoineCrm.WebApi"
RUN dotnet build "KoineCrm.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "KoineCrm.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KoineCrm.WebApi.dll"]
