FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build

WORKDIR /source

COPY igx-service-external-csharp.csproj .
RUN dotnet restore --use-current-runtime

COPY Properties .
COPY *.cs .
COPY appsettings.* .
RUN dotnet publish --use-current-runtime --self-contained false --no-restore -o /app

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./igx-service-external-csharp"]
