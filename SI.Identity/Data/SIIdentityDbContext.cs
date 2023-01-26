using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SI.Identity.Models;

namespace SI.Identity.Data
{
    public class SIIdentityDbContext: IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public SIIdentityDbContext(DbContextOptions<SIIdentityDbContext> options)
            : base(options)
        {
        }
    }
}