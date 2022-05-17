using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Pagamento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public string CodBarras { get; set; }

        [Display(Name = "Data de Vencimento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DataVencimento { get; set; }

        [Display(Name = "Data de Pagamento")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DataPagamento { get; set; }
        public bool Pago { get; set; }
        public bool Ativo { get; set; }
    }
}
