using Microsoft.EntityFrameworkCore;
using WebAppApi.domain.entities;

namespace WebAppApi.data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Contact> Contacts => Set<Contact>();
    public DbSet<Message> Messages => Set<Message>();
}