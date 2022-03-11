FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Noodle.App/Noodle.App.csproj", "Noodle.App/"]
RUN dotnet restore "Noodle.App/Noodle.App.csproj"
COPY src .
WORKDIR "/src/Noodle.App"
RUN dotnet build "Noodle.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Noodle.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Noodle.App.dll"]