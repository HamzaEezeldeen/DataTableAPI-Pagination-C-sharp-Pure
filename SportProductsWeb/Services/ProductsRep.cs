using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SportProductsWeb.Services
{
    public class ProductsRep
    {

        readonly ShopContext _shopContext;

        public ProductsRep(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }

        public async Task<(List<Product>, QueryPageResult)> SearchForProduct( ProductQueryParamters pqParameters)
        {

            IQueryable<Product> products = _shopContext.Products;


            if (pqParameters.MinPrice != null)
            {
                products = products.Where(p => p.Price >= pqParameters.MinPrice);
            }

            if (pqParameters.MaxPrice != null)
            {
                products = products.Where(p => p.Price <= pqParameters.MaxPrice);
            }


            if (!string.IsNullOrEmpty(pqParameters.SearchTearm))
            {
                products = products.Where(p =>
                    p.Name.ToLower().Contains(pqParameters.SearchTearm.ToLower()) ||
                    p.Sku.ToLower().Contains(pqParameters.SearchTearm.ToLower())
                 );
            }

            if (!string.IsNullOrEmpty(pqParameters.Name))
            {
                products = products.Where(p => p.Name.ToLower() == pqParameters.Name.ToLower());
            }

            if (!string.IsNullOrEmpty(pqParameters.Sku))
            {
                products = products.Where(p => p.Sku.ToLower() == pqParameters.Sku.ToLower());
            }



            products = products.OrderByCustom(pqParameters.SortBy, pqParameters.SortOrder);


            //if(! string.IsNullOrEmpty(pqParameters.SortBy))
            //{
            //    if(pqParameters.SortBy.Equals("Price",StringComparison.OrdinalIgnoreCase))
            //    {
            //        if(pqParameters.SortOrder.Equals("asc",StringComparison.OrdinalIgnoreCase))
            //            products=products.OrderBy(p => p.Price);
            //        else if (pqParameters.SortOrder.Equals("desc", StringComparison.OrdinalIgnoreCase))
            //            products = products.OrderByDescending(p => p.Price);

            //    }
            //}


            QueryPageResult pr = new QueryPageResult();
            //get the total count
            pr.TotalCount = products.Count();
            //find the number of pages
            pr.TotalPages = (int)Math.Ceiling(pr.TotalCount / (double)pqParameters.Size);

            //find previous and next page number
            if (pqParameters.CurPage - 1 > 0)
                pr.PreviousPage = pqParameters.CurPage - 1;
            if ((pqParameters.CurPage + 1) <= pr.TotalPages)
                pr.NextPage = pqParameters.CurPage + 1;

            //find first row and last row on the page
            if (pr.TotalCount == 0)  //if no record found
                pr.FirstRowOnPage = pr.LastRowOnPage = 0;
            else
            {
                pr.FirstRowOnPage = (pqParameters.CurPage - 1) * pqParameters.Size + 1;
                pr.LastRowOnPage = Math.Min(pqParameters.CurPage * pqParameters.Size, pr.TotalCount);
            }


            products = products.Skip(pqParameters.Size * (pqParameters.CurPage - 1))
              .Take(pqParameters.Size);

            return (await products.ToListAsync(),pr);

        }



    }
}
