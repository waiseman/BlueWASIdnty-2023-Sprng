using Microsoft.AspNetCore.Identity;

namespace BlueWASIdnty.Server.Models
{
    public class ApplicationUser : IdentityUser
    {
    public string? ResgistrationNo{ get; set; }    
    }
}