using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace SportProductsWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        readonly ShopContext _shopContext;

        public ProductController(ShopContext shopContext)
        {
            _shopContext = shopContext;
        }


        [HttpGet]
        public string Count()
        {
            return _shopContext.Products.Count().ToString();
        }


        [HttpGet]
        [Route("search")]
        public async Task<ActionResult> SearchForProduct([FromQuery] ProductQueryParamters pqParameters)
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

            Response.Headers.Add("X-PageSortResult", JsonSerializer.Serialize(pr));

            products = products.Skip(pqParameters.Size * (pqParameters.CurPage - 1))
              .Take(pqParameters.Size);

            return Ok(await products.ToArrayAsync());

        }

        [HttpGet]
        [Route("/{id:int}")]
        [Route("{id:int}")]
        public ActionResult<Product> getProduct(int id)
        {
            Product product = _shopContext.Products.Find(id);
            if (product == null)
            {
                return NotFound();

                //return StatusCode(StatusCodes.Status204NoContent, $"No Product with id={id}");
            }

            return Ok(product);
            //return StatusCode(StatusCodes.Status200OK, product);

        }

        [HttpGet("all")]
        public async Task<ActionResult<List<Product>>> getAll()
        {
            //return await _shopContext.Products.Include(c=> c.Category).ToListAsync();  
            return Ok(await _shopContext.Products.ToArrayAsync());
        }


        [HttpGet("/products_in/{categoryid}")]
        public async Task<ActionResult<List<Product>>> getProductsByCategory(int categoryid)
        {
            if (_shopContext.Categories.Find(categoryid) == null)
                return NotFound();

            return Ok(await _shopContext.Products.Where(p => p.CategoryId == categoryid).ToListAsync());

        }

        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct([FromForm] Product product)
        {

            await _shopContext.Products.AddAsync(product);

            try
            {
                await _shopContext.SaveChangesAsync();

            }
            catch (Exception er)
            {
                return BadRequest(new { message = er.Message });
            }

            return Ok(product);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            // Product oldp = _shopContext.Products.Where(p=> p.Id==id).FirstOrDefault();

            //if (oldp == null) return BadRequest();

            if (id != product.Id) return BadRequest();

            //_shopContext.Products.Update(product);s

            //oldp.Sku = product.Sku;
            //oldp.Price = product.Price;
            //oldp.Description = product.Description;
            //oldp.CategoryId = product.CategoryId;
            //oldp.Name = product.Name;
            //oldp.IsAvailable = product.IsAvailable;

            try
            {
                _shopContext.Entry(product).State = EntityState.Modified;

                await _shopContext.SaveChangesAsync();

            }
            catch (Exception er)
            {
                return Ok(new { message = er.Message });
            }

            return Ok();

        }


        [HttpDelete("{id}")]
        public async Task<ActionResult<Product>> DeleteProduct(int id)
        {
            Product p = _shopContext.Products.Find(id);

            if (p == null) return NotFound();

            try
            {
                _shopContext.Products.Remove(p);

                await _shopContext.SaveChangesAsync();

            }
            catch (Exception er)
            {
                return Ok(new { message = er.Message });
            }

            return Ok(p);

        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<List<Product>>> DeleteMultiple([FromForm] int[] ids)
        {
            var plist = new List<Product>();
            foreach (int id in ids)
            {
                var product = _shopContext.Products.Find(id);
                if (product == null)
                {
                    return NotFound();
                }

                plist.Add(product);
            }

            try
            {
                _shopContext.Products.RemoveRange(plist);
                await _shopContext.SaveChangesAsync();

            }
            catch (Exception er)
            {
                return Ok(new { message = er.Message });
            }

            return Ok(plist);
        }
        [HttpPost]
        [Route("advdatatable")]
        public async Task<ActionResult<List<Product>>> DataForDatatable()
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            string? searchValue = Request.Form["search[Value]"].FirstOrDefault();


            var sortColIndex = Request.Form["order[0][column]"].FirstOrDefault();
            var sortDir = Request.Form[$"order[0][dir]"].FirstOrDefault();


            string? sortCol = Request.Form[$"columns[{sortColIndex}][data]"].FirstOrDefault();




            var data = _shopContext.Products.AsQueryable();


            int totalOfRecord = data.Count();
            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(x =>
                 x.Name.ToLower().Contains(searchValue.ToLower()) ||
                 x.Sku.ToLower().Contains(searchValue.ToLower())
                );

            }
            int FilteredRecord = data.Count();

            if (!string.IsNullOrEmpty(sortCol) || !string.IsNullOrEmpty(sortDir))
            {
                sortCol = char.ToUpperInvariant(sortCol[0]) + sortCol.Substring(1);
                data = data.OrderByCustom(sortCol, sortDir);
            }

            var products = data.Skip(skip).Take(pageSize).ToList();
            var dataTableObj = new
            {
                draw = draw,
                recordsTotal = totalOfRecord,
                recordsFiltered = FilteredRecord,
                data = products
            };
            return Ok(dataTableObj);
        }






    }
}
