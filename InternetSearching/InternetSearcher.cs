using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OCHKO
{
    
    public class InternetSearcher
    {
        public InternetSearcher() { }
        #region methods to use api of internet search
        private List<WebPage> GetSitesAfterSearching(string json)
        {

            
            var o = JObject.Parse(json); // make jObject
            var jsPages = o["webPages"]["value"].ToList(); //take part, that needed
            List<WebPage> webPages = new List<WebPage>();
            //jSon parsing
            for (int i = 0; i < jsPages.Count; i++)
            {
                var name = jsPages[i]["name"].ToString().ToLower();

                var snippet = jsPages[i]["snippet"].ToString().ToLower();

                var url = jsPages[i]["url"].ToString();


                WebPage webPage = new WebPage();
                webPage.Name = name;
                webPage.Snippet = snippet;
                webPage.Url = url;
                webPage.DisplayUrl = jsPages[i]["displayUrl"].ToString();
                webPage.Id = jsPages[i]["id"].ToString();
                webPage.Language = jsPages[i]["language"].ToString();
                webPage.Type = jsPages[i]["_type"].ToString();

                
                webPage.IsFamilyFriendly = bool.Parse(jsPages[i]["isFamilyFriendly"].ToString());
                webPage.IsNavigational = bool.Parse(jsPages[i]["isNavigational"].ToString());
                webPage.DateLastCrawled = DateTime.Parse(jsPages[i]["dateLastCrawled"].ToString());

                webPages.Add(webPage);

            }
            return webPages;
        }

        //That method to get result of search
        public List<WebPage> MakeSearch(string site, string searchLine)
        {
            searchLine = searchLine.ToLower(); 
            var json = BingWebSearchAsync(searchLine, site).Result; // I take json string by using Bing search, where is result of searching. 
            //If will need you can add method that using a google or something yet
            List<WebPage> webPages = GetSitesAfterSearching(json); // Transform json to class
            return webPages;
        }
        //There's method to make internet search
        private async Task<string> BingWebSearchAsync(string site ,string searchLine)
        {
            string rapidapiKey = JObject.Parse(System.IO.File.ReadAllText(@"..\..\..\Config.json"))["x-rapidapi-key"].ToString(); //take apiKey


            // take from https://rapidapi.com/microsoft-azure-org-microsoft-cognitive-services/api/bing-web-search1/
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri("https://bing-web-search1.p.rapidapi.com/search?q=" + Uri.EscapeDataString($"{site} {searchLine}")),
                Headers =
                {
                    { "x-bingapis-sdk", "true" },
                    { "x-rapidapi-key", rapidapiKey },
                    { "x-rapidapi-host", "bing-web-search1.p.rapidapi.com"},
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                return body;
            }
        }
        #endregion

        #region methods to use a spacific website
        public IEnumerable<WebPage> MakeVlpWarframeSearch(string searchLine)
        {
            //
            string response;
            searchLine = searchLine.ToLower();


            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString("https://vlp-warframe.ru/?s=" + searchLine);
            }
            //parsing html that was gotten
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(response);
            var nodes = html.DocumentNode.SelectNodes(@"//article
                        [
                        contains(@class, 'type-post') 
                        ]")?.ToList() ?? new List<HtmlNode>();
            List<WebPage> pages = new List<WebPage>();


            //there's parsing of html to get needed information
            for (int i = 0; i < nodes.Count; i++)
            {
                var h = new HtmlDocument();
                h.LoadHtml(nodes[i].OuterHtml);
                var currentNode = h.DocumentNode;
                WebPage page = new WebPage();
                page.DateLastCrawled = DateTime.Now;
                page.Id = searchLine;
                page.IsFamilyFriendly = true;
                page.Language = "ru";
                page.IsNavigational = true;
                string snippet = currentNode.SelectSingleNode(@"//div[@class='cb-meta']/div[@class='cb-excerpt']").FirstChild.InnerText;
                string name = currentNode.SelectSingleNode(@"//div[@class='cb-meta']/h2[@class='cb-post-title']/a").InnerText;
                string url = currentNode.SelectSingleNode(@"//div[@class='cb-meta']/h2[@class='cb-post-title']/a").GetAttributeValue("href", "none-source");
                string type = currentNode.SelectSingleNode(@"//div[@class='cb-meta']/div[contains(@class,'cb-byline')]/div[contains(@class, 'cb-category')]/a")?.InnerText ?? "???";
                page.Snippet = snippet;
                page.Name = name;
                page.Url = url;
                page.DisplayUrl = url;
                page.Type = type;
                pages.Add(page);

            }



            return pages;
        }


        #endregion

    }
}
