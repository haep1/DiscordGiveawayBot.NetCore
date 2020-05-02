using Bot.Giveaway;
using Bot.Interfaces;
using BotClient;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DiscordFunAndGiveawayBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IDiscordClient, DiscordClient>();
            services.AddSingleton<ICultureHelper, CultureHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDiscordClient discordClient, ICultureHelper cultureHelper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("DiscordFunAndGiveawayBot is alive!");

                if (!discordClient.IsRunning)
                {
                    // You can add the token as an environment variable - e.g. as a Config Var in Heroku
                    string discordToken = Environment.GetEnvironmentVariable("DiscordToken");
                    await discordClient.RunBot(app.ApplicationServices, Configuration, cultureHelper, discordToken);
                }
            });
        }
    }
}
