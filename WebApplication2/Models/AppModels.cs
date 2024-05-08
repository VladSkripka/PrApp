using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class Search
    {
        public string departurePoint { get; set; }
        public string destinationPoint { get; set; }
        public DateTime departureDate { get; set; }
        public DateTime ?returnDate { get; set; }
        public DateTime searchTime { get; set;}
        public string searchType { get; set; }
    }

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

    public class Train
    {
        [Key]
        public int trainTypeID { get; set; }
        public string typeName { get; set; }
        public int trainSeats { get; set; }
        public int trainRows { get; set; }
        public string schemaPath { get; set; }

        [ValidateNever]
        public ICollection<VacantDeparture> Departures { get; set; }
    }

    //Chaged on TicketModel
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


}
