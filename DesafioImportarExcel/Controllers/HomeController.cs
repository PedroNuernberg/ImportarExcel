using DesafioImportarExcel.Data;
using DesafioImportarExcel.Models;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace DesafioImportarExcel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger,
                              ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public List<Pedido> Filtrar(string CodigoCliente, string Categoria, string SkuProduto)
        {
            var pedidos = from p in _context.Pedidos
                          select p;

            if (!String.IsNullOrEmpty(CodigoCliente))
            {
                pedidos = pedidos.Where(p => p.CodigoCliente.ToUpper().Contains(CodigoCliente.ToUpper()));
            }
            if (!String.IsNullOrEmpty(Categoria))
            {
                pedidos = pedidos.Where(p => p.Categoria.ToUpper().Contains(Categoria.ToUpper()));
            }
            if (!String.IsNullOrEmpty(SkuProduto))
            {
                pedidos = pedidos.Where(p => p.SkuProduto.ToUpper().Contains(SkuProduto.ToUpper()));
            }

            return pedidos.ToList();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult ImportarExcel(string CodigoCliente, string Categoria, string SkuProduto) 
        {
            List<Pedido> pedidos = Filtrar(CodigoCliente, Categoria, SkuProduto);

            return View(pedidos);
        }

        public List<Pedido> ListarTodos()
        {
            return _context.Pedidos.ToList();
        }

        [HttpPost]
        //[RequestSizeLimit(512 * 1024 * 1024)]
        public async Task<IActionResult> ImportarExcel(IFormFile file)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            if(file != null && file.Length > 0) 
            {
                var uploadsFolder = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads\\";

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, file.FileName);

                using(var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            bool conteudo = false;

                            while (reader.Read())
                            {
                                if (!conteudo)
                                {
                                    conteudo = true;
                                    continue;
                                }

                                Pedido pedido = new Pedido();

                                if (reader.GetValue(0) == null)
                                    break;                                

                                pedido.CodigoCliente = reader.GetValue(0).ToString();
                                pedido.Categoria = reader.GetValue(1).ToString();
                                pedido.SkuProduto = reader.GetValue(2).ToString();
                                pedido.Data = reader.GetValue(3).ToString();
                                pedido.Quantidade = Convert.ToInt32(reader.GetValue(4).ToString());
                                pedido.ValorFaturamento = (decimal)double.Parse(reader.GetValue(5).ToString(), CultureInfo.InvariantCulture);

                                _context.Add(pedido);
                                await _context.SaveChangesAsync();
                            }
                        } while (reader.NextResult());
                    }
                }
            }

            List<Pedido> pedidos = ListarTodos();

            return View(pedidos);
        }
    }
}