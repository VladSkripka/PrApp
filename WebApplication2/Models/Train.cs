using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace WebApplication2.Models;

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

public class CreateTrainModelIn
{
    public string TypeName { get; set; }
    public int TrainSeats { get; set; }
    public int TrainRows { get; set; }
}