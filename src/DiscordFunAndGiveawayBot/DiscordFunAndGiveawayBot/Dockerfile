FROM microsoft/aspnetcore-build AS builder
WORKDIR /source
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release

FROM microsoft/aspnetcore
WORKDIR /app
EXPOSE 80

COPY --from=builder /app .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet DiscordFunAndGiveawayBot.dll