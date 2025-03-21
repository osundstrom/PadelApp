
# bygga
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# kopiera 
COPY *.csproj ./

RUN dotnet restore

# kopiera hela 
COPY . ./

# publish
RUN dotnet publish -c Release -o /out

# runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# port 80
EXPOSE 80

# miljövariabel
ENV ASPNETCORE_URLS=http://+:$PORT

# kopier för bilder, db och publicerade
COPY --from=build /out ./
COPY wwwroot/Images /app/wwwroot/Images
COPY PadelApp.db ./


ENTRYPOINT ["dotnet", "PadelApp.dll"]