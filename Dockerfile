
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build


WORKDIR /app


COPY *.csproj ./
RUN dotnet restore


COPY . ./
RUN dotnet publish -c Release -o /out


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80


COPY --from=build /out .


ENTRYPOINT ["dotnet", "PadelApp.dll"]