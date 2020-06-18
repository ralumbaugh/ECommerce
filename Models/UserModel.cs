using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    public class User
    {
    [Key]
    public int UserId {get; set;}
    [Required (ErrorMessage="First Name is Required")]
    public string FirstName {get;set;}
    [Required (ErrorMessage="Last Name is Required")]
    public string LastName {get;set;}
    [Required (ErrorMessage="Email is Required")]
    [EmailAddress (ErrorMessage="Please enter a valid email address")]
    public string Email {get; set;}
    [Required (ErrorMessage="Email is Required")]
    [DataType (DataType.Password)]
    [MinLength (8, ErrorMessage="Password must be at least 8 characters!")]
    public string Password {get; set;}
    public bool IsAdmin {get; set;} = false;
    public List<Order> Orders {get; set;}
    public DateTime CreatedAt {get; set;} = DateTime.Now;
    public DateTime UpdatedAt {get; set;} = DateTime.Now;
    [NotMapped]
    [Compare("Password")]
    [DataType(DataType.Password)]
    public string Confirm {get; set;}
    }
}