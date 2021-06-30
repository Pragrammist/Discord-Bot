using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OCHKO
{
    public class WMInterteSearcher
    {
        
        //this method get a neede information about every orders
        public List<Order> GetProducts(string item)
        {
            string json;
            var webPage = MakeSearchToMarket(item);
            var source = webPage.Url;
            using (var webClient = new WebClient())
            {
                json = webClient.DownloadString(source);
            }
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(json);
            string res = html.GetElementbyId("application-state").InnerText;
            JObject o = JObject.Parse(res);
            var jsonPreporties = o.Properties().First().First.First.First.ToList();
            var players = new List<Order>();
            for (int i = 0; i < jsonPreporties.Count; i++)
            {
                var props = jsonPreporties[i];
                string platinum = props["platinum"].ToString();
                string status = props["user"]["status"].ToString();
                string orderType = props["order_type"].ToString();
                string nickName = props["user"]["ingame_name"].ToString();
                string item2 = item;
                Order order = new Order();
                order.OrderType = orderType;
                order.Platinum = platinum;
                order.Status = status;
                order.NickName = nickName;
                order.Item = item2;
                order.WebPage = webPage;
                players.Add(order);
            }
            return players;
        }

        //Uses internet searcher to get first link to needed a item
        private WebPage MakeSearchToMarket(string item)
        {
            InternetSearcher _internet = new InternetSearcher();
            var webPages = _internet.MakeSearch("https://warframe.market/ru/", item);
            var webPage = webPages.FirstOrDefault() ?? throw new Exception("Поисковик выдал ничего");
            return webPage;
        }
       
        
    }
}
