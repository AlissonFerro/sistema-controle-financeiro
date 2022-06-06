using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;

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

            decimal[] meses = new decimal[12];
            //dynamic meses = new JsonArray(12);
            List<Pagamento> pagamentos = _context.Pagamentos.ToList();

            foreach (var pagamento in pagamentos)
            {
                if (pagamento.Pago)
                {
                    totalPagos++;
                    valorTotalPago += pagamento.Valor;
                    switch (pagamento.DataPagamento.Month)
                    {
                        case 1:
                            meses[0] += pagamento.Valor;
                            break;
                        case 2:
                            meses[1] += pagamento.Valor;
                            break;
                        case 3:
                            meses[2] += pagamento.Valor;
                            break;
                        case 4:
                            meses[3] += pagamento.Valor;
                            break;
                        case 5:
                            meses[4] += pagamento.Valor;
                            break;
                        case 6:
                            meses[5] += pagamento.Valor;
                            break;
                        case 7:
                            meses[6] += pagamento.Valor;
                            break;
                        case 8:
                            meses[7] += pagamento.Valor;
                            break;
                        case 9:
                            meses[8] += pagamento.Valor;
                            break;
                        case 10:
                            meses[9] += pagamento.Valor;
                            break;
                        case 11:
                            meses[10] += pagamento.Valor;
                            break;
                        case 12:
                            meses[11] += pagamento.Valor;
                            break;
                    }
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
            ViewBag.Jan = meses[0];
            ViewBag.Fev = meses[1];
            ViewBag.Mar = meses[2];
            ViewBag.Abr = meses[3];
            ViewBag.Mai = meses[4];
            ViewBag.Jun = meses[5];
            ViewBag.Jul = meses[6];
            ViewBag.Ago = meses[7];
            ViewBag.Set = meses[8];
            ViewBag.Out = meses[9];
            ViewBag.Nov = meses[10];
            ViewBag.Dez = meses[11];

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
            int teste = pagamento.CodBarras.Length;
            ViewBag.Error = null;

            if (!ModelState.IsValid || pagamento.CodBarras.Length<40 || pagamento.CodBarras.Length>41)
            {
                ViewBag.Error = "Confira o código digitado";
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
                List<Pagamento> pagamentosDescricao = _context.Pagamentos.ToList();
                for (int i = 0; i < pagamentosDescricao.Count; i++)
                {
                    if (pagamentosDescricao[i].Descricao.ToUpper() == pagamento.Descricao.ToUpper() && pagamentosDescricao[i].DataVencimento.Month == pagamento.DataVencimento.Month)
                    {
                        ViewBag.Erro = "Erro: Título já cadastrado neste mês";
                        return View("Blank");
                    }
                }
                pagamento.Ativo = true;
                _context.Pagamentos.Add(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 1;
                TempData["Mensagem"] = "Pagamento cadastrado com sucesso";
                return RedirectToAction("Index");
            }
            else
            {
                pagamento.Ativo = true;
                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 2;
                TempData["Mensagem"] = "Pagamento alterado com sucesso";
                return RedirectToAction("Index");
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
            if (pagamentoEncontrado.Pago)
            {
                ViewBag.Erro = "Título pago não pode ser editado";
                return View("Blank");
            }
            ViewBag.Name = "Editar";
            return View("Formulario", pagamentoEncontrado);
        }

        public IActionResult Pagar(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);
            if (pagamentoEncontrado == null)
            {
                return View("Blank", "Pagamento não encontrado");
            }
            if (pagamentoEncontrado.Pago)
            {
                ViewBag.Erro = "Título já pago";
                return View("Blank");
            }
            pagamentoEncontrado.DataPagamento = DateTime.Now;
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
                TempData["Alterado"] = 1;
                TempData["Mensagem"] = "Pagamento realizado com sucesso";
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


        public IActionResult Excluir(int id)
        {
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