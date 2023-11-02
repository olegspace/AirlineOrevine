using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class Flight // Рейс
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Title { get; set; }
    
    public Route Route { get; set; }
}