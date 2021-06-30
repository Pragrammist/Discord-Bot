using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OCHKO
{
    public class BotCommandsModel : ModuleBase<SocketCommandContext>
    {
        //this is command module, that you can read about on official documentation
        private readonly BotApiInternetSearfer _iSerfer; 
        private readonly DiscordSocketClient _client; // this field haven't used but i left it anyway, it can will need
        private readonly DiscrodUsersContext _db; //Database where i store a discord users to write his options.

        public BotCommandsModel(DiscrodUsersContext db, DiscordSocketClient client, BotApiInternetSearfer iSerfer)
        {
            _client = client;
            _db = db;
            _iSerfer = iSerfer;
        }



        [Command("sell")]
        [Summary("searching sellings item in warframeMarket")]
        public async Task SearchSellingsWarframeItem([Remainder] string item)
        {
            
            order_type orderType = order_type.sell;

            await SearchingProducts(item, orderType);
        }

        [Command("buy")]
        [Summary("searching sellings item in warframeMarket")]
        public async Task SearchBuyingsWarframeItem([Remainder] string item)
        {
            order_type orderType = order_type.buy;

            await SearchingProducts(item, orderType);

        }

        [Command("all")]
        [Summary("searching sellings item in warframeMarket")]
        public async Task SearchAllWarframeItem([Remainder] string item)
        {

            order_type orderType = order_type.all;
            await SearchingProducts(item, orderType);
        }

        [Command("status")]
        [Summary("set player status")]
        public async Task SetStatus([Remainder] status status)
        {
            await UpdateCurrentUser(new DiscordUser {OptionalStatus = status });
            await ReplyAsync("Статус был изменен на " + status.ToString());
        }
        //For checking bot
        [Command("say")]
        [Summary("echo")]
        public async Task Echo([Remainder] string echo) => await ReplyAsync(echo);

        [Command("build")]
        [Summary("get warframe builds from VlpWarframe")]
        public async Task GetBuild([Remainder] string warframe)
        {
            
            var builds = _iSerfer.GetBuild(warframe);
            
            
            foreach (var b in builds)
            {
                
                var bImgUrl = System.Net.WebUtility.HtmlDecode(b.ImgUrl);
                var bNote = System.Net.WebUtility.HtmlDecode(b.Note);
                var bTitle = System.Net.WebUtility.HtmlDecode(b.ShortDescription);
                var bUrl = System.Net.WebUtility.HtmlDecode(b.SourceUrl);
                var embed = new EmbedBuilder
                {
                    ImageUrl = bImgUrl,
                    Description = bNote,
                    Title = bTitle,
                    Url = bUrl,
                }.Build(); 


                
                await ReplyAsync(embed:embed);
                
                //Thread.Sleep(100);
            }
           
           
        }

        private async Task SearchingProducts(string item, order_type orderType)
        {

            var dUser = GetCurrentDiscordUser(false);
            status status = dUser?.OptionalStatus ?? status.ingame;



            _iSerfer.MakeInternetSearch(item);
            var products = _iSerfer.GetProducts(status, orderType).ToList();

            

            string message = $"";
            for (int i = 0; i < products.Count; i++)
            {
                string player = products[i].NickName;
                string platinum = products[i].Platinum;
                string item2 = products[i].Item;


                string orderT = products[i].OrderType;
                string status2 = products[i].Status;
                if (orderT == "buy")
                {
                    orderT = ":regional_indicator_b:";
                }
                else
                {
                    orderT = ":regional_indicator_s:";
                }

                if (status2 == "online")
                {
                    status2 = ":green_circle:";
                }
                else if (status2 == "offline")
                {
                    status2 = ":new_moon::";
                }
                else
                {
                    status2 = ":video_game:";
                }

                message += $"{player}\t({status2})\t{orderT}\t{item2}:\t{platinum}:coin:\n";
            }
            await ReplyAsync(message);
        }        
        private DiscordUser GetCurrentDiscordUser(bool isCreateNew = true)
        {

            //Current user, which enter command
            var dUserId = Context.User.Id; // write his id

            DiscordUser dUser = _db.Users.FirstOrDefault(u => u.Id == dUserId); // search throw id

            if (dUser == null && isCreateNew) // if null create new users
            {
                dUser = new DiscordUser { Id = dUserId, OptionalStatus = status.ingame};
                _db.Add(dUser);
                _db.SaveChanges();
            }
            return dUser;
        }
        private async Task UpdateCurrentUser(DiscordUser newData)
        {
            var dUser = GetCurrentDiscordUser(true);
            dUser.OptionalStatus = newData.OptionalStatus;
            _db.Update(dUser);
            await _db.SaveChangesAsync();
        }

    }    
}
