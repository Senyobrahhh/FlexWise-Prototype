using Microsoft.AspNetCore.Identity;

namespace FlexWise_Prototype.Entities;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = "";
    public bool IsTrainer { get; set; }

    public int FollowersCount { get; set; }
    public int SubscribersCount { get; set; } // relevant only if IsTrainer == True;
    
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }

    public DateTime DateCreated { get; set; } =  DateTime.UtcNow;
}