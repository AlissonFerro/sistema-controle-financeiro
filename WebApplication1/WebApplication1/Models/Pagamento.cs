namespace WebApplication1.Models
{
    public class Pagamento
    {
        // id, descrição do pagamento, valor, código de barras, data de vencimento e data de pagamento.
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string CodBarras { get; set; }
        public DateOnly DataVencimento { get; set; }
        public DateOnly DataPagamento { get; set; }
    }
}
