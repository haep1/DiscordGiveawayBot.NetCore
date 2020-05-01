# DiscordFunAndGiveawayBot

This is a Discord Bot, that ist written ASP.NET Core and C#.
The bot owns some different features, the main features is to organize giveaway-raffles in specific channels.

The difference to other Discord Bots is, that you can run a giveaway for a longer time - e.g. for some days. 
People can participate by entering a predefined keyword.


> **You can add the Bot to your Discord server using this URL:** <br/>
> https://discordapp.com/api/oauth2/authorize?client_id=569931884495110144&permissions=68608&scope=bot


## Features
The bot supports following features at the moment:

### General features
* Multi language support: The bot supports one admin language and multiple outgoing languages for bot posts in the channel.
   At the moment two languages are completely integrated: English and German

### Fun actions
* Let the bot post an user defined message in a specific channel
* Post messages that mention a specific person

### Giveaways
* Init new giveaways with a lot of settings:
   * Set the time of the current day OR set a specific datetime
   * Set the codeword, that users must enter to participate
   * Set the name of the prize in all languages you want to support
   
### Commands
`%sendmessage #channel` - Bot sends a message to that channel<br/>
`%sendmessage #channel @person` - Bot sends a message tot that channel and mentions the person<br/>
`%initgiveaway #channel` - Inits a new giveaway. All further steps are explained by the giveaway workflow<br/>
`%cancel` - Cancel the giveaway initiation<br/>
`%start` - Start the initiated giveaway


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
