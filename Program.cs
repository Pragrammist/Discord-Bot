using System;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using Discord.Commands;
using System.IO;
using Newtonsoft.Json.Linq;
namespace OCHKO
{
   
    class Program
    {
        string token;
        private DiscordSocketClient client;
        public static void Main(string[] args)
        {
            //I took this code part from official documentation of discord
            new Program().MainAsync().GetAwaiter().GetResult(); //Starting program

            
        }
        //I took this code part from official documentation of discord
        public async Task MainAsync()
        {
            client = new DiscordSocketClient();
            
            client.Log += Log;

            //  You can assign your bot token to a string, and pass that in to connect.
            //  This is, however, insecure, particularly if you plan to have your code hosted in a public repository.


            // Some alternative options would be to keep your token in an Environment Variable or a standalone file.
            // var token = Environment.GetEnvironmentVariable("NameOfYourEnvironmentVariable");
            // var token = File.ReadAllText("token.txt");
            // var token = JsonConvert.DeserializeObject<AConfigurationClass>(File.ReadAllText("config.json")).Token;


            var str = File.ReadAllText(@"..\..\..\Config.json");
            JObject jObject = JObject.Parse(str);
            token = jObject["token"].ToString();

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();
            

            client.MessageUpdated += MessageUpdated;
            client.Ready += () =>
            {
                Console.WriteLine("Bot is connected!");
                return Task.CompletedTask;
            };
            
            
            
            CommandService service = new CommandService();
            // Block this task until the program is closed.
            Initialize initialize = new Initialize(service, client);
            var s = initialize.BuildServiceProvider();
            CommandHandler command = new CommandHandler(client, service, s);
            await command.InstallCommandsAsync();
            Console.ReadLine();
            Console.ReadLine();
        }
        //I took this code part from official documentation of discord
        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }
        //I took this code part from official documentation of discord
        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }


    



}
