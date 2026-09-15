FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Devsu.Accounts/src/Api/Api.csproj", "Devsu.Accounts/src/Api/"]
COPY ["Devsu.Accounts/src/Application/Application.csproj", "Devsu.Accounts/src/Application/"]
COPY ["Devsu.Accounts/src/Domain/Domain.csproj", "Devsu.Accounts/src/Domain/"]
COPY ["Devsu.Accounts/src/Infrastructure/Infrastructure.csproj", "Devsu.Accounts/src/Infrastructure/"]
COPY ["Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "Devsu.Accounts/src/Api/Api.csproj"
COPY . .
WORKDIR "/src/Devsu.Accounts/src/Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
