using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CRS.Business.Models.Entities
{
    public class Location
    {
        #region Primitive properties

        public int Id { get; set; }
        public string Name { get; set; }

        #endregion

        #region Navigation properties

        public virtual ICollection<User> Users { get; set; }

        #endregion
    }
}