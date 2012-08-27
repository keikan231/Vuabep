using System;
using CRS.Business.Models;

namespace CRS.Business.DataSorting
{
    public class Sort
    {
        public static Order GetSortKey(string sort)
        {
            // Default is sort by recent
            if (sort == null)
            {
                return Order.Recent;
            }

            if (sort.Equals("comments", StringComparison.OrdinalIgnoreCase))
                return Order.Comments;

            if (sort.Equals("rates", StringComparison.OrdinalIgnoreCase))
                return Order.Rates;

            if (sort.Equals("votes", StringComparison.OrdinalIgnoreCase))
                return Order.Votes;

            if (sort.Equals("views", StringComparison.OrdinalIgnoreCase))
                return Order.Views;

            if (sort.Equals("approval", StringComparison.OrdinalIgnoreCase))
                return Order.Approval;

            return Order.Recent;
        }
    }
}