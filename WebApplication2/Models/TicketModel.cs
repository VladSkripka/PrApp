using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication2.Models;

public class TicketModel
{
    [Key]
    public int ticketID { get; set; }
    public int ticketPID { get; set; }
    public string departureP { get; set; }
    public string arrivalP { get; set; }
    public DateTime departureD { get; set; }
    public DateTime arrivalD { get; set; }
    public string passengerFullName { get; set; }
    public int carriageNumber { get; set; }
    public int seat { get; set; }
    public string status { get; set; }
    public int vacantDepatureID { get; set; }
    public DateTime PurchaseDate { get; set; }
    public string ticketAgeType { get; set; }
    public string? addtionalOptions { get; set; }


    [ValidateNever]
    public IdentityUser user { get; set; } //user from identity
}