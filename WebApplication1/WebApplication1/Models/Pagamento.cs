namespace WebApplication1.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string CodBarras { get; set; }
        public DateOnly DataVencimento { get; set; }
        public DateOnly DataPagamento { get; set; }
        public bool Ativo { get; set; }
    }
}
