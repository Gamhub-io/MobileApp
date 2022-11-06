using AresNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AresNews.Helpers.Tools
{
    /// <summary>
    /// Static class regrouping a bunch of useful stuff to deal with feeds
    /// </summary>
    public static class FeedToolkit
    {
        /// <summary>
        /// Compare the items of two Feed enumerable
        /// NOTE: We have to create this annoying/redundant checker to compare
        /// the items because of the Feed.IsLoaded property
        /// </summary>
        /// <param name="feedList1">First feef to compare</param>
        /// <param name="feedList2">Second feed to compare</param>
        /// <returns></returns>
        public static bool CampareItems (IEnumerable<Feed> feedList1, IEnumerable<Feed> feedList2)
        {
            return feedList1.All(f =>
            {
                return feedList2.All(cf => cf.Id == f.Id);
            });
        }
    }
}
