using System;
using System.Collections.Generic;
using System.Linq;

namespace OCHKO
{
    //This class uses in Discord model as handler or api of all functional in this project(build searching and order searching)
    public class BotApiInternetSearfer 
    {
        public const int TAKENUM = 10; //Limit. So in array can be 1000 orders :)
        MarketAction _action;
        BuildSearcher _buildSearcher;
        public BotApiInternetSearfer()
        {
            _action = new MarketAction();
            _buildSearcher = new BuildSearcher();
        }

        public IEnumerable<Build> GetBuild(string frame)
        {
            var builds = _buildSearcher.GetBuilds(frame);
            return builds;
        }
        public void MakeInternetSearch(string item)
        {
            _action.SearchProduct(item);
        }
        public IEnumerable<Order> GetProducts(status status, order_type type)
        {
            return _action.GetDataFromWarframeMarket(type, status).Take(TAKENUM); 
        }
    }
}
