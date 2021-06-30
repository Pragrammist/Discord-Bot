using System;
using System.Collections.Generic;
using System.Text;

namespace OCHKO
{
    public class Order
    {
        public WebPage WebPage { get; set; } //Web page that was taken from array, which is returned by internet searcher.
        public string Item { get; set; } //Item
        public string Status { get; set; } //Status of player that buy or sell item
        public string NickName { get; set; } //His nickname
        public string OrderType { get; set; } //information about buy or sell item. There's can be only two value: buy or sell
        public string Platinum { get; set; } //The worth of purchase 

    }
    
}
