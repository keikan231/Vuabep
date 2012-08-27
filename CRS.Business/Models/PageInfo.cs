using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Business.Models
{
    public class PageInfo
    {
        public int PageSize { get; set; }
        public int PageNo { get; set; }

        public PageInfo(int pageSize, int pageNo)
        {
            PageSize = pageSize;
            PageNo = pageNo;
        }
    }
}