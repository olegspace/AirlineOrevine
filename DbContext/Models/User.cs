using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DbContext.Enums;

namespace DbContext.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    
    [MinLength(5)]
    public string Login { get; set; }
    
    [MinLength(5)]
    public string Password { get; set; }
    
    public UserTypes UserType { get; set; }
    
    public List<AccessRight> AccessRights { get; set; }

}