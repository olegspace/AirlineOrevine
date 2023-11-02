using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using DbContext.Enums;

namespace DbContext.Models;

public class Employee
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Patronymic { get; set; }
    
    public ICollection<Crew> Crews { get; set; }
    
    public EmployeeTypes EmployeeType { get; set; }
}