FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["DiscordFunAndGiveawayBot/DiscordFunAndGiveawayBot.csproj", "DiscordFunAndGiveawayBot/"]
COPY ["BotClient/BotClient.csproj", "BotClient/"]
RUN dotnet restore "DiscordFunAndGiveawayBot/DiscordFunAndGiveawayBot.csproj"
COPY . .
WORKDIR "/src/DiscordFunAndGiveawayBot"
RUN dotnet build "DiscordFunAndGiveawayBot.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DiscordFunAndGiveawayBot.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll