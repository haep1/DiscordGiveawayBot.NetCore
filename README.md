# DiscordFunAndGiveawayBot

This is a Discord Bot, that ist written ASP.NET Core and C#.
The bot owns some different features, the main features is to organize giveaway-raffles in specific channels.

The difference to other Discord Bots is, that you can run a giveaway for a longer time - e.g. for some days. 
People can participate by entering a predefined keyword.

## Features
The bot supports following features at the moment:

### General features
* Multi language support: The bot supports one admin language and multiple outgoing languages for bot posts in the channel.
   At the moment two languages are completely integrated: English and German

### Fun actions
* Let the bot post an user defined message in a specific channel

### Giveaways
* Init new giveaways with a lot of settings:
   * Set the time of the current day OR set a specific datetime
   * Set how often the raffle should repeat (repeates every day at the same time if you want)
   * Set the codeword, that users must enter to participate
   * Set the price in all languages you want to support


## Dev information

### Change configs
To run the bot, you must enter your discord application bot token and define the UI languages.
Therefore open the **appsettings.json** file in the solution:

``` json
"DiscordToken": "{Enter token here}",
"OutputCultures": [ "de-DE", "en-US" ],
"AdminCulture": "en-US"
```
At the moment, german and english languages are implemented.
If you want to add further resources, you must a resouce-file at scr/DiscordFunAndGiveawayBot/BotClient/Resources.

### Deployment
As this designed as an ASP.NET Core app you can deploy it like a webpage - e.g. as AppService on Azure or simply in a docker container.
Dockerfile is added, tested on Heroku: Works fine!
