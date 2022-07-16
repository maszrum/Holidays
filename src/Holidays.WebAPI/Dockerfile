FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Holidays.WebAPI/Holidays.WebAPI.csproj", "Holidays.WebAPI/"]
RUN dotnet restore "Holidays.WebAPI/Holidays.WebAPI.csproj"
COPY . .
WORKDIR "/src/Holidays.WebAPI"
RUN dotnet build "Holidays.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Holidays.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_URLS=https://+;http://+
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=payAlot
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Holidays.WebAPI.dll"]
