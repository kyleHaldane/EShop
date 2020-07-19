﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Core.Models
{
    public class Basket : BaseEntity
    {
        public virtual ICollection<BasketItem> Basketitems { get; set; }

        public Basket()
        {
            this.Basketitems = new List<BasketItem>();
        }
    }
}
