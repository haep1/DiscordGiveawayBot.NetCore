FROM microsoft/aspnetcore-build AS builder
WORKDIR /source
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

COPY --from=builder /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll