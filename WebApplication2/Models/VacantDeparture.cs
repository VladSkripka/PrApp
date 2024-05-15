using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication2.Models;

public class VacantDeparture
{
    [Key]
    public int vdID { get; set; }
    public int PID { get; set; }
    public string vacanDepartureName { get; set; }
    public string departureP { get; set; }
    public string departureCoord { get; set; }
    public string departureAdress { get; set; }
    public string arrivalP { get; set; }
    public DateTime departureD { get; set; }
    public DateTime arrivalD { get; set; }
    public int price { get; set; }
    public int seatsCount { get; set; }
    public int carriageCount { get; set; }


    [ValidateNever]
    public Train Train { get; set; } // from Train

    [ValidateNever]
    public IdentityUser provider { get; set; } //same as user from identity

    [ValidateNever]
    public ICollection<Ticket> Tickets { get; set; }
}