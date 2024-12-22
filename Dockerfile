FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MA.RewardService.Host/MA.RewardService.Host.csproj", "MA.RewardService.Host/"]
RUN dotnet restore "MA.RewardService.Host/MA.RewardService.Host.csproj"
COPY . .
WORKDIR "/src/MA.RewardService.Host"
RUN dotnet build "MA.RewardService.Host.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MA.RewardService.Host.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS 80
ENTRYPOINT ["dotnet", "MA.RewardService.Host.dll"]
