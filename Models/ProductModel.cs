using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class Product
    {
    [Key]
    public int ProductId {get; set;}
    [Required (ErrorMessage="Product name is required")]
    public string ItemName {get; set;}
    [Required (ErrorMessage="Quantity is required")]
    [Range (0, double.PositiveInfinity, ErrorMessage="We can't really have a negative amount of things can we?")]
    public int? Quantity {get; set;}
    [Required (ErrorMessage="An Image URL is required")]
    [DataType (DataType.ImageUrl, ErrorMessage="Please make sure this is an image URL")]
    public string ImageURL {get; set;}
    [Required (ErrorMessage="A description is required")]
    [MinLength (5, ErrorMessage="Description should be at least 5 characters")]
    public string Description {get; set;}
    [Required (ErrorMessage="Everything has a price")]
    [Range (0,double.PositiveInfinity, ErrorMessage="Price should be a positive number or we'll go out of business!")]
    public double? Price {get; set;}
    public List<Order> Orders {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    }
}