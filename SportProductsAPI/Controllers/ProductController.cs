using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportProductsAPI.Models;

namespace SportProductsAPI.Controllers
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

         IQueryable<Product> products  = _shopContext.Products;


        if(pqParameters.MinPrice!=null)
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

            if (!string.IsNullOrEmpty( pqParameters.Name))
            {
                products=products.Where(p=> p.Name.ToLower()==pqParameters.Name.ToLower());
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


            products = products.Skip(pqParameters.Size * (pqParameters.Page - 1))
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
        public async Task<ActionResult> UpdateProduct(int id,Product product)
        {
           // Product oldp = _shopContext.Products.Where(p=> p.Id==id).FirstOrDefault();

            //if (oldp == null) return BadRequest();

             if (id!=product.Id) return BadRequest();

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
            foreach(int id in ids)
            {
                var product = _shopContext.Products.Find(id);
                if(product==null)
                {
                    return NotFound();
                }

                plist.Add(product);
            }

            try
            {
                _shopContext.Products.RemoveRange(plist);
                await _shopContext.SaveChangesAsync();

            }catch(Exception er)
            {
                return Ok(new { message = er.Message });
            }

            return Ok(plist);
        }






    }
}
