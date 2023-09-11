using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace SportProductsWeb.Pages.DataTable
{
    public class IndexModel : PageModel
    {
        public readonly ShopContext _shopContext;

        public IndexModel(ShopContext shopContext)
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
