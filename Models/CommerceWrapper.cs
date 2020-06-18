using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class CommerceWrapper
    {
        public User CurrentUser {get; set;}
        public List<User> AllUsers {get; set;}
        public Product CurrentProduct {get; set;}
        public Order CurrentOrder {get; set;}
        public List<Product> AllProducts {get; set;}
        public List<Order> AllOrders {get;set;}
        public int ThingsShown {get;set;} = 15;
    }
}