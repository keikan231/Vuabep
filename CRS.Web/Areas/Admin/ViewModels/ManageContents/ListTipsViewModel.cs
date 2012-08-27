using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CRS.Business.Models.Entities;

namespace CRS.Web.Areas.Admin.ViewModels.ManageContents
{
    public class ListTipsViewModel
    {
        public IList<Tip> Tips { get; set; } 
    }
}