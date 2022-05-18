using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;

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
            return View("Index", _context.Pagamentos.ToList());
        }

        public IActionResult Listar()
        {
            return View("Listagem", _context.Pagamentos.ToList());
        }

        public IActionResult Adicionar(Pagamento pagamento)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return View("Formulario");
            }

            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == pagamento.Id);

            if(pagamentoEncontrado == null)
            {
                _context.Pagamentos.Add(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 1;
                TempData["Mensagem"] = "Pagamento cadastrado com sucesso";
                return View("Listagem", _context.Pagamentos.ToList());
            }
            else
            {
                pagamento.Ativo = true;
                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                TempData["Alterado"] = 2;
                TempData["Mensagem"] = "Pagamento alterado com sucesso";
                return View("Listagem", _context.Pagamentos.ToList());
            }
        }

        public IActionResult Editar(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);
            if (pagamentoEncontrado == null)
            {
                return View("Erro", "Pagamento não encontrado");
            }
            if (pagamentoEncontrado.Ativo == false)
            {
                return View("AtualizarInativo", pagamentoEncontrado);
            }
            ViewBag.Name = "Editar";
            return View("Formulario", pagamentoEncontrado);
        }

        public IActionResult FormularioExcluir(int id)
        {
            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);

            if (pagamentoEncontrado == null || pagamentoEncontrado.Ativo == false)
            {
                return View("Erro", "Pagamento não encontrado");
            }
            if (pagamentoEncontrado.Pago)
            {
                return View("Erro", "Título pago não pode ser excluído");
            }
            ViewBag.Name = "Excluir";
            return View("FormularioExcluir", pagamentoEncontrado);
        }

        public IActionResult Excluir(int id)
        {
            TempData["Alterado"] = 3;
            TempData["Mensagem"] = "Pagamento excluido com sucesso";

            Pagamento pagamentoEncontrado = new Pagamento();
            pagamentoEncontrado = _context.Pagamentos.FirstOrDefault(a => a.Id == id);

            if (pagamentoEncontrado != null)
            {
                Pagamento pagamento = new Pagamento();
                pagamento = pagamentoEncontrado;
                pagamento.Ativo = false;

                _context.Entry(pagamentoEncontrado).CurrentValues.SetValues(pagamento);
                _context.SaveChanges();
                return RedirectToAction("Listagem");
            }
            else
            {
                return View("Erro", "Pagamento não encontrado");
            }
        }

        /*
        public IActionResult Cadastrar()
        {
            
        }*/

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}