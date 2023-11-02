using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class Departure //Вылет
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public Crew Crew { get; set; }
    
    public Flight Flight { get; set; }
    
    public Liner Liner { get; set; }
    
    public DateTime DepartureTime { get; set; }
    
    public DateTime ArrivalTime { get; set; }
}