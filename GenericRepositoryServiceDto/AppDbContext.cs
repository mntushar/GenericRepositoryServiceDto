using Microsoft.EntityFrameworkCore;

namespace GenericRepositoryServiceDto;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}