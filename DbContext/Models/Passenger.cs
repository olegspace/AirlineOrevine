using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class Passenger
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Patronymic { get; set; }
    
    public int PassportSeries { get; set; }
    
    public int PassportId { get; set; }
    
    public DateOnly DateOfIssue { get; set; }
    
    public string IssuedBy { get; set; }

}