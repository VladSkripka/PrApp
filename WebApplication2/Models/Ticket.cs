using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication2.Models;

public class Ticket
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

    [ValidateNever]
    public VacantDeparture vacDep { get; set; } //from Vacant Departure

    [ValidateNever]
    public IdentityUser user { get; set; } //user from identity
}