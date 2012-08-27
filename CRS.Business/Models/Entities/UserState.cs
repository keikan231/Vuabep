using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Business.Models.Entities
{
    public class UserState
    {
        #region Primitive properties

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        #endregion
    }
}