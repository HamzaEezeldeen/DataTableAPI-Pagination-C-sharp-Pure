using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SportProductsWeb.Pages.DataTable
{
    public class IndexApiModel : PageModel
    {
        public readonly ShopContext _shopContext;

        public IndexApiModel(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        public IEnumerable<Product> ListOfProducts { get; set; }

        public async Task OnGet()
        {
            ListOfProducts = await _shopContext.Products.ToListAsync();

        }
    }
}
