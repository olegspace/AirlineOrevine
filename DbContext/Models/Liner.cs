using System;
using System.ComponentModel.DataAnnotations.Schema;
using DbContext.Enums;

namespace DbContext.Models;

public class Liner
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public string Name { get; set; }
    
    public DateTime InspectionDate { get; set; }
    
    public DateTime DateOfIssue { get; set; }
    
    public string Photo { get; set; }
    
    public LinerTypes LinerType { get; set; }
}