using System;
using System.ComponentModel.DataAnnotations.Schema;
using DbContext.Enums;

namespace DbContext.Models;

public class Ticket
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public Passenger Passenger { get; set; }
        
    public Employee Employee { get; set; }
        
    public DateTime PurchaseDate { get; set; }
        
    public int Price { get; set; }
        
    public int CheckoutNumber { get; set; }

    public Departure Departure { get; set; }
        
    public ServiceTypes ServiceClass { get; set; }
        
    public string Place { get; set; }
}