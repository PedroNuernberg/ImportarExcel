namespace DesafioImportarExcel.Models
{
    public class Pedido
    {
        public int Id { get; set; }
        public string CodigoCliente { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string SkuProduto { get; set; } = string.Empty;
        public string Data { get; set; } = string.Empty;
        public int Quantidade { get; set; } = 0;
        public decimal ValorFaturamento { get; set; } = 0;

    }
}
