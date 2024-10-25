FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

COPY ./*.vbproj ./
RUN dotnet restore VBAspCoreAdoPg.vbproj

COPY . .
RUN dotnet publish VBAspCoreAdoPg.vbproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS runtime
WORKDIR /app

RUN export ASPNETCORE_ENVIRONMENT=Production
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
RUN chown -R appuser:appgroup /app
USER appuser

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VBAspCoreAdoPg.dll"]

EXPOSE 8001
