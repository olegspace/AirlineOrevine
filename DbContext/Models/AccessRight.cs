using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DbContext.Models;

public class AccessRight
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    public bool Read { get; set; }
    
    public bool Write { get; set; }
    
    public bool Edit { get; set; }
    
    public bool Delete { get; set; }

    public FormTypes Form { get; set; }
    
    public User User { get; set; }
}