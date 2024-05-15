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

    //Chaged on TicketModel
}
