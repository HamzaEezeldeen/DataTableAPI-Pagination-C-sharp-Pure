using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SportProductsWeb.Data;
using SportProductsWeb.Services;
using System.Text.Json;

namespace SportProductsWeb.Pages.Products
{
    public class HomeModel : PageModel
    {
        readonly ShopContext shopContext;
        private readonly ProductsRep productRep;

        public HomeModel(ShopContext shopContext, ProductsRep productRep)
        {
            this.shopContext = shopContext;
            this.productRep = productRep;
        }

        public IEnumerable<SelectListItem> PageSizeList { get; set; } = new List<SelectListItem>() {
            new SelectListItem { Value = "5", Text = "5" },
            new SelectListItem { Value = "10", Text = "10" },
            new SelectListItem { Value = "20", Text = "20" },
        };

        public IEnumerable<SelectListItem> SortList { get; set; } = new List<SelectListItem>() {
            new SelectListItem { Value = "Id", Text = "Id" },
            new SelectListItem { Value = "Name", Text = "Name" },
            new SelectListItem { Value = "Price", Text = "Price" },
        };

        public IEnumerable<Product> products { get; set; }
        public QueryPageResult pageResult { get; set; }

        [BindProperty(SupportsGet = true)]
        public ProductQueryParamters queryParamters { get; set; }

        public async Task OnGet()
        {



            //string url = $"https://localhost:7234/api/Product/search?MinPrice={queryParamters.MinPrice}&MaxPrice={queryParamters.MaxPrice}&CurPage={queryParamters.CurPage}&Size={queryParamters.Size}&SortBy={queryParamters.SortBy}&SortOrder={queryParamters.SortOrder}";
            //if (queryParamters.SearchTearm?.Length > 0)
            //    url += $"&SearchTearm={queryParamters.SearchTearm}";
            //url += "\r\n";
            //HttpResponseMessage response = await new HttpClient().GetAsync(url);
            ////display the list of products
            //if (response.IsSuccessStatusCode)
            //    products = await response.Content.ReadFromJsonAsync<IEnumerable<Product>>();

            //IEnumerable<string> headerValue;
            //if (response.Headers.TryGetValues("X-PageSortResult", out headerValue))
            //{
            //    pageResult = JsonSerializer.Deserialize<QueryPageResult>(headerValue.First());
            //}


            try
            {
                (products, pageResult) = await productRep.SearchForProduct(queryParamters);
            }
            catch (Exception ex)
            {

            }

        }
    }
}
