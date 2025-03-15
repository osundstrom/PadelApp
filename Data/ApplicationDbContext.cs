using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PadelserviceApp.Models;

namespace PadelApp.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<PadelBooking> PadelBookings {get; set; }
    public DbSet<PadelCourt> PadelCourts {get; set; }
}
