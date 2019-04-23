FROM microsoft/aspnetcore-build AS base
WORKDIR /app
EXPOSE 80

COPY . .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll