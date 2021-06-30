using System.Linq;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Net;
using System;
namespace OCHKO
{
    //Search build
    public class BuildSearcher
    {
        InternetSearcher _searcher;
        public BuildSearcher()
        {
            _searcher = new InternetSearcher();
        }
        public IEnumerable<Build> GetBuilds(string item)
        {
            var pages = _searcher.MakeVlpWarframeSearch(item); //take pages, that search engine of site give
            List<Build> builds = new List<Build>();
            for (int i = 0; i < pages.Count(); i++)
            {
                var el = pages.ElementAt(i);
                
                if (el.Type.ToLower().Contains(item.ToLower())) //checks is it a suitable type of article or not. True if it is suitable
                {
                    var url = el.Url; //Got url to article
                    var tBuilds = ParseCurrentPage(url); //parsing url
                    builds.AddRange(tBuilds);//adds a buld in array result
                }
            }
            return builds;
        }
        private List<Build> ParseCurrentPage(string url)
        {
            string response;
            using (var webClient = new WebClient())
            {
                response = webClient.DownloadString(url);
            }
            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(response);
            var nodes = html.DocumentNode.SelectSingleNode(@"//section[contains(@class, 'cb-entry-content')]").ChildNodes;
            List<Build> builds = new List<Build>();
            //Parsing page
            for (int i = 0; i < nodes.Count; i++)
            {
                HtmlDocument h = new HtmlDocument();
                h.LoadHtml(nodes[i].OuterHtml);

                var node = h.DocumentNode.SelectSingleNode(@"//img");
                var imgUrl = node?.GetAttributeValue("src", "none-img");
                if (node != null && !imgUrl.Contains("avatar"))
                {
                    Build build = new Build();
                    build.SourceUrl = url;
                    build.ImgUrl = imgUrl;
                    build.Note = "";
                    build.ShortDescription = "";
                    if (i > 1)
                    {
                        var j = i - 2;
                        var n = nodes[j];
                        string text = GetAllInnerText(n);
                        build.ShortDescription = text;
                    }
                    if (i < nodes.Count - 2)
                    {
                        var j = i + 2;
                        var n = nodes[j];
                        string text = GetAllInnerText(n); //Get all inner text in paragraph
                        build.Note = text;
                    }
                    builds.Add(build);
                }
            }
            return builds;
        }
        
        private string GetAllInnerText(HtmlNode node)
        {
            if (node.ChildNodes.Count == 1)
            {
                return node.InnerText;
            }
            else
            {
                var nodes = node.ChildNodes;
                string res = "";
                for (int i = 0; i < nodes.Count; i++)
                {
                    if (nodes[i].Name == "#text")
                    {
                        res += " " + nodes[i].InnerText;
                    }
                    else
                    {
                        res += " " + GetAllInnerText(nodes[i]);
                    }
                }
                return res;
            }
        }
    }
}
