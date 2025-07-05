using BaseProject.Model.Entities;
using Microsoft.AspNetCore.Identity;

namespace BaseProject.Model.Entities
{
    public class AppRole : IdentityRole<Guid>, IEntity
    {
    }
}
