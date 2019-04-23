FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /source
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
EXPOSE 80

COPY --from=builder /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll