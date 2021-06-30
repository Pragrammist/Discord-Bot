using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCHKO
{
    //Take from documentation
    public class Initialize
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _client;
        
        private readonly DiscrodUsersContext _db;

        // Ask if there are existing CommandService and DiscordSocketClient
        // instance. If there are, we retrieve them and add them to the
        // DI container; if not, we create our own.
        public Initialize(CommandService commands = null, DiscordSocketClient client = null, DiscrodUsersContext db = null)
        {
            _commands = commands ?? new CommandService();
            _client = client ?? new DiscordSocketClient();
            _db = db ?? new DiscrodUsersContext();
        }

        public IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton<CommandHandler>()
            .AddSingleton(_db)
            .AddSingleton<BotApiInternetSearfer>()
            .BuildServiceProvider();
    }
}
