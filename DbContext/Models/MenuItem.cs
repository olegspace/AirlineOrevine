using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class MenuItem
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    public int? ParentId { get; set; }
    
    public string? ItemName { get; set; }
    
    public string? DllName { get; set; }
    
    public string? FunctionName { get; set; }
    
    public int Priority { get; set; }
}