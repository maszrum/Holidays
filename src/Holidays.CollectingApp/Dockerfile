FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Holidays.CollectingApp/Holidays.CollectingApp.csproj", "Holidays.CollectingApp/"]
RUN dotnet restore "Holidays.CollectingApp/Holidays.CollectingApp.csproj"
COPY . .
WORKDIR "/src/Holidays.CollectingApp"
RUN dotnet build "Holidays.CollectingApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Holidays.CollectingApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Holidays.CollectingApp.dll"]
