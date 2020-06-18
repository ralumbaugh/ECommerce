using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Order
    {
    [Key]
    public int OrderId {get; set;}
    public int UserId {get; set;}
    public User User {get; set;}
    public int ProductId {get; set;}
    public Product Product {get; set;}
    [Required (ErrorMessage="Please input a quantity")]
    [Range (0,double.PositiveInfinity, ErrorMessage="Hey now, we're selling to you, not the other way around!")]
    public int? Quantity {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}