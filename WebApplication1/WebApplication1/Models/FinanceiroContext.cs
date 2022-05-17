using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class FinanceiroContext : DbContext
    {
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=SERVIDOR\SQLEXPRESS01;Database=SistemaFinanceiro;Trusted_Connection=True;");
        }
    }
}
