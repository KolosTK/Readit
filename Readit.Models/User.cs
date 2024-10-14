using Microsoft.AspNetCore.Identity;
namespace Readit.Models;

public class User: IdentityUser
{
    //IdentityUser has these properties:
    //
    //Id
    //UserName NormalizedUserName
    //Email NormalizedEmail EmailConfirmed
    //PhoneNumer, PhoneNumberConfirmed
    
    public string Name { get; set; }
    
    public string? City { get; set; }
    
    public DateTime DayOfBirth { get; set; }
}