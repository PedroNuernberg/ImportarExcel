using DesafioImportarExcel.Models;
using Microsoft.EntityFrameworkCore;

namespace DesafioImportarExcel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Pedido> Pedidos { get; set; }
    }
}
