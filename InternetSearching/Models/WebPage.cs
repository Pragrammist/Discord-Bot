using System;
using System.Collections.Generic;
using System.Text;

namespace OCHKO
{
    //These are web pages that you can see in search results, when you used a search engine but the class
    public class WebPage
    {
        public string Url { get; set; } //
        public string Name { get; set; }
        public string Snippet { get; set; }
        public string Type { get; set; }// Dont have overwall template and values. Depending on what kind of searcher you used have different value. 
        //For example, if was used buildSearcher then there's type of article(news, 'warframe', 'damage', 'specific warframe' itc)
        public string Id { get; set; }
        public bool IsFamilyFriendly { get; set; }
        public string DisplayUrl { get; set; }
        public DateTime DateLastCrawled { get; set; }
        public string Language { get; set; }
        public bool IsNavigational { get; set; }
    }
}
