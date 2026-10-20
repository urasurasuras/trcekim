# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY TrCekim.slnx ./
COPY src/TrCekim.Core/TrCekim.Core.csproj src/TrCekim.Core/
COPY src/TrCekim.Data/TrCekim.Data.csproj src/TrCekim.Data/
COPY src/TrCekim.Web/TrCekim.Web.csproj src/TrCekim.Web/
RUN dotnet restore src/TrCekim.Web/TrCekim.Web.csproj
COPY src/ src/
RUN dotnet publish src/TrCekim.Web/TrCekim.Web.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
# Database, photos and Data Protection keys. Mount a volume here and back it up.
RUN mkdir -p /data && chown app:app /data
VOLUME /data
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DataDirectory=/data \
    DisableHttpsRedirection=true \
    TrustForwardedHeaders=true
COPY --from=build /app .
USER app
EXPOSE 8080
ENTRYPOINT ["dotnet", "TrCekim.Web.dll"]
