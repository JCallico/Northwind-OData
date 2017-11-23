using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Query;
using Dapper;
using GSA.Samples.Northwind.OData.Common.Extensions;
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Controllers
{
    public class ProductsController : BaseController<Product, Guid>
    {
        private static readonly int _defaultPageSize = 50;

        public virtual async Task<IHttpActionResult> Get(ODataQueryOptions<Product> queryOptions)
        {
            int page, pageSize;
            queryOptions.GetPagingOptions(out page, out pageSize, _defaultPageSize);

            // executing two queries at once in order to return total count and results 
            // using one database query 
            var sql = @"SELECT COUNT(1) FROM [Products];
                        SELECT * FROM 
                        (
                            SELECT 
                                ROW_NUMBER() OVER(ORDER BY ProductUniqueID DESC) AS PagedNumber, 
                                [ProductUniqueID] as [ID],
                                [ProductName],
                                [UnitPrice],
                                [ReferenceUniqueID],
                                [ProductUri]
                            FROM [Products] 
                        ) AS u 
                        WHERE PagedNUMBER BETWEEN ((@Page-1) * @PageSize + 1) AND (@Page * @PageSize)";

            var results = await Db.QueryMultipleAsync(sql, new DynamicParameters(new Dictionary<string, object>
            {
                { "@Page", page },
                { "@PageSize", pageSize }
            }));

            var totalCount = results.Read<int>().Single();
            var products = results.Read<Product>().ToList();

            var pagedResult = new PageResult<Product>(
                products,
                Request.GetNextPageLink(pageSize),
                totalCount);

            return Ok(pagedResult);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Get([FromODataUri] Guid key)
        {
            return Ok((await Db.GetListAsync<Product>(new { ID = key })).FirstOrDefault());
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Post(Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                await Db.InsertAsync<Guid>(product);
            }
            catch (SqlException e)
            {
                if (e.Message.IndexOf("duplicate", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    return Conflict();
                }

                throw;
            }

            return Created(product);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<Product> patch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var product = (await Db.GetListAsync<Product>(new { ID = key })).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            patch.Patch(product);

            await Db.UpdateAsync(product);

            return Updated(product);
        }

        [EnableQuery]
        public async Task<IHttpActionResult> Put([FromODataUri] Guid key, Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != product.ID)
            {
                return BadRequest();
            }

            await Db.UpdateAsync(product);

            return Updated(product);
        }

        public async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            var product = (await Db.GetListAsync<Product>(new { ID = key })).FirstOrDefault();

            if (product == null)
            {
                return NotFound();
            }

            await Db.DeleteAsync(product);

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}