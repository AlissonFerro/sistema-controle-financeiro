using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class FinanceiroContext : DbContext
    {
        public DbSet<Pagamento> Pagamentos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=SNCCH01LABF118\TEW_SQLEXPRESS;Database=SistemaFinanceiro;Trusted_Connection=True;");
        }
    }
}
