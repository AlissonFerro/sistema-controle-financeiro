using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private readonly FinanceiroContext _context;

        public HomeController(FinanceiroContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            int totalPagos = 0;
            int aVencer = 0;
            int pagamentosVencidos = 0;
            decimal valorTotalPago = 0;

            List<Pagamento> pagamentos = _context.Pagamentos.ToList();

            foreach (var pagamento in pagamentos)
            {
                if (pagamento.Pago)
                {
                    totalPagos++;
                    valorTotalPago += pagamento.Valor;
                }
                if (!pagamento.Pago && pagamento.Ativo)
                {
                    aVencer++;
                }
                int data = DateTime.Now.Subtract(pagamento.DataVencimento).Days;
                if (data > 0 && !pagamento.Pago)
                {
                    pagamentosVencidos++;
                }
            }

            ViewBag.QuantidadeTotalPagos = totalPagos;
            ViewBag.AVencer = aVencer;
            ViewBag.PagamentosVencidos = pagamentosVencidos;
            ViewBag.TotalPago = valorTotalPago;

            return View("Index", pagamentos);
        }

        public IActionResult Listar()
        {
            return View("Listagem", _context.Pagamentos.ToList());
        }

        public IActionResult Formulario()
        {
            return View("Formulario");
        }

        public IActionResult Adicionar(Pagamento pagamento)
        {
            ModelState.Remove("Id");
            ModelState.Remove("DataPagamento");
            if (!ModelState.IsValid)
            {
                return View("Formulario");
            }

            Pagamento pagamentoEncontrado = new Pagamento();
            Pagamento pagamentoVerificado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == pagamento.Id);
            pagamentoVerificado = _context.Pagamentos.FirstOrDefault(a => a.CodBarras == pagamento.CodBarras);

            if (pagamentoEncontrado == null)
            {
                if (pagamentoVerificado != null)
                {
                    ViewBag.Erro = "Erro: Código de barras já cadastrado";
                    return View("Blank");
                };
                //var descricao = from des in _context.Pagamentos.ToList();
                //string descricao = _context.Pagamentos.Select(a => a.Descricao == pagamento.Descricao);
                pagamento.Ativo = true;
                _context.Pagamentos.Add(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 1;
                TempData["Mensagem"] = "Pagamento cadastrado com sucesso";
                return View("Index", _context.Pagamentos.ToList());
            }
            else
            {
                pagamento.Ativo = true;
                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 2;
                TempData["Mensagem"] = "Pagamento alterado com sucesso";
                return View("Index", _context.Pagamentos.ToList());
            }
        }

        public IActionResult Editar(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);
            if (pagamentoEncontrado == null)
            {
                ViewBag.Erro = "Pagamento não encontrado";
                return View("Blank");
            }
            if (pagamentoEncontrado.Ativo == false)
            {
                return View("AtualizarInativo", pagamentoEncontrado);
            }
            ViewBag.Name = "Editar";
            return View("Formulario", pagamentoEncontrado);
        }

        public IActionResult Pagar(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);
            pagamentoEncontrado.DataPagamento = DateTime.Now;
            if (pagamentoEncontrado == null)
            {
                return View("Erro", "Pagamento não encontrado");
            }
            return View("FormularioPagamento", pagamentoEncontrado);
        }

        public IActionResult ConfirmarPagamento(Pagamento pagamento)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == pagamento.Id);
            if (pagamentoEncontrado == null || !pagamentoEncontrado.Ativo)
            {
                ViewBag.Erro = "Pagamento não encontrado";
                return View("Blank");
            }
            if (pagamentoEncontrado.Pago)
            {
                ViewBag.Erro = "Título já pago";
                return View("Blank");
            }
            if (pagamentoEncontrado != null)
            {
                pagamento.Pago = true;
                pagamento.Ativo = true;
                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Erro = "Pagamento não encontrado";
                return View("Blank");
            }
        }

        /*public IActionResult FormularioExcluir(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);

            if (pagamentoEncontrado == null || pagamentoEncontrado.Ativo == false)
            {
                ViewBag.Erro = "Pagamento não encontrado";
                return View("Blank");
            }
            if (pagamentoEncontrado.Pago)
            {
                ViewBag.Erro = "Título pago não pode ser excluído";
                return View("Blank");
            }
            ViewBag.Name = "Excluir";
            return View("FormularioExcluir", pagamentoEncontrado);
        }*/

        public IActionResult Excluir(int id)
        {
            //CORRIGIR BUG MENSAGEM

            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);

            if (pagamentoEncontrado.Pago)
            {
                ViewBag.Erro = "Título pago não pode ser excluído";
                return View("Blank");
            }
            if (pagamentoEncontrado != null)
            {
                Pagamento pagamento = new Pagamento();
                pagamento = pagamentoEncontrado;
                pagamento.Ativo = false;

                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 3;
                TempData["Mensagem"] = "Pagamento excluido com sucesso";
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Erro = "Pagamento não encontrado";
                return View("Blank");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}