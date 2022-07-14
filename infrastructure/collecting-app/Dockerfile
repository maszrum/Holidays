FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env

WORKDIR /app
COPY ./../src/ .

WORKDIR /app
RUN dotnet restore Holidays.NoTests.sln
RUN dotnet publish -c Release -o ./out Holidays.NoTests.sln

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY --from=build-env /app/out .
COPY ./infrastructure/collecting-app/appsettings.json .

ENTRYPOINT ["dotnet", "Holidays.Cli.dll"]
