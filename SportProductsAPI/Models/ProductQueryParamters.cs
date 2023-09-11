namespace SportProductsAPI.Models
{
    public class ProductQueryParamters : QueryParameters
    {
        public decimal?  MinPrice { get; set; }
        public decimal?  MaxPrice { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string SearchTearm { get; set; } = string.Empty;
    }
}
