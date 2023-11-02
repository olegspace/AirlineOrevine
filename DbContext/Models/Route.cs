using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class Route
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public Airport StartingPoint { get; set; }
    
    public Airport EndingPoint { get; set; }
    
    public ICollection<Airport> WayPoints { get; set; }
}