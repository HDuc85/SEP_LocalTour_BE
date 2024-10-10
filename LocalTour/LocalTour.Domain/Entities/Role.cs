using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LocalTour.Domain.Entities;

public partial class Role : IdentityRole<Guid>
{
    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
