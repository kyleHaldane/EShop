﻿using EShop.Core.Contracts;
using EShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;

namespace EShop.Services
{
    public class BasketService
    {
        IRepository<Product> ProductContext;
        IRepository<Basket> basketContext;

        public const string BasketSessionName = "eCommerceBasket";
        public BasketService(IRepository<Product> ProductContext, IRepository<Basket> basketContext)
        {
            this.basketContext = basketContext;
            this.ProductContext = ProductContext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if(cookie != null)
            {
                string basketId = cookie.Value;
                if(!string.IsNullOrEmpty(basketId))
                {
                    basket = basketContext.Find(basketId);
                }
                else
                {
                    if(createIfNull)
                    {
                        basket = CreateNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = CreateNewBasket(httpContext);
                }
            }

            return basket;
        }

        private Basket CreateNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            basketContext.Insert(basket);
            basketContext.Commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires= DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AdddToBasket(HttpContextBase httpContext, string productId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.Basketitems.FirstOrDefault(i => i.ProductId == productId);

            if( item == null)
            {
                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productId,
                    Quantity = 1
                };

                basket.Basketitems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }

            basketContext.Commit();
        }

        public void RemoveFromBasket(HttpContextBase httpContext, string itemId)
        {
            Basket basket = GetBasket(httpContext, true);
            BasketItem item = basket.Basketitems.FirstOrDefault(i => i.Id == itemId);

            if(item != null)
            {
                basket.Basketitems.Remove(item);
                basketContext.Commit();
            }
        }
    }
}
