using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopSpeed.Domain.Models;

namespace TopSpeed.Infrastructure.Common
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {



        }
        public DbSet<Brand> Brand { get; set; }

        public DbSet<VehicleType> VehicleType { get; set; }

        public DbSet<Post> Post { get; set; }
    }
}
