FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["IAMBuddy.MSSQLAccountManager/IAMBuddy.MSSQLAccountManager.csproj", "IAMBuddy.MSSQLAccountManager/"]
COPY ["IAMBuddy.Shared/IAMBuddy.Shared.csproj", "IAMBuddy.Shared/"]
RUN dotnet restore "IAMBuddy.MSSQLAccountManager/IAMBuddy.MSSQLAccountManager.csproj"
COPY . .
WORKDIR "/src/IAMBuddy.MSSQLAccountManager"
RUN dotnet build "IAMBuddy.MSSQLAccountManager.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "IAMBuddy.MSSQLAccountManager.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IAMBuddy.MSSQLAccountManager.dll"]