FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

COPY . .
FROM microsoft/aspnetcore-build AS builder
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll