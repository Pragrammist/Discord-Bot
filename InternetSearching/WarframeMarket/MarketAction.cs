using System;
using System.Collections.Generic;
using System.Linq;


namespace OCHKO
{
    // enum that mean what type of offer you wonna take: buy, sell or all type
    public enum order_type { buy, sell, all }
    public enum status { ingame, online, offline, all };

    //Its kind of api for WMInternetSearcher. WMInternetSearcher give just a orders, and MarketAction give only definitely orders, for example you can got only buyers, which is online
    //You can
    public class MarketAction
    {
        public List<Order> AllOrders { get; private set; } // all orders that warframe market gave


        //IMPORTANT
        //IMPORTANT
        //IMPORTANT
        //IMPORTANT
        //IMPORTANT

        //firstly call this method and then others. If you will not use this method you get exception
        public void SearchProduct(string item)
        {
            WMInterteSearcher searcher = new WMInterteSearcher();
            AllOrders = searcher.GetProducts(item);
        }
        public MarketAction()
        {
            
        }
        public List<Order> GetDataFromWarframeMarket(order_type type, status status)
        {
            if (AllOrders == null)
                throw new Exception("Не был произведен интернет поиск"); //The Exeption

            Order[] buffer = new Order[AllOrders.Count];
            AllOrders.CopyTo(buffer);
            List<Order> result = buffer.ToList(); // copy
            //Depending on type and status of order return different result
            //In each condition do a selection and sorting  
            if (type == order_type.buy)
            {
                result = result.Where((player) => player.OrderType == "buy").OrderByDescending((p) => int.Parse(p.Platinum)).ToList();
            }
            else if (type == order_type.sell)
            {
                result = result.Where((player) => player.OrderType == "sell").OrderBy((p) => int.Parse(p.Platinum)).ToList();
            }
            else
            {
                result = result.OrderByDescending((p) => int.Parse(p.Platinum)).ToList();
            }

            if (status == status.online)
            {
                result = result.Where((player) => player.Status == "online").ToList();
            }
            else if (status == status.offline)
            {
                result = result.Where((player) => player.Status == "offline").ToList();
            }
            else if (status == status.ingame)
            {
                result = result.Where((player) => player.Status == "ingame").ToList();
            }
            return result;
        }
    }
}
