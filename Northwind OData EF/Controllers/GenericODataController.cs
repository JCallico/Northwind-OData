using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.Http;
using GSA.Samples.Northwind.OData.Common.Filters;
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Controllers
{
    [KeyAndSecretBasicAuthentication] // Enable authentication via an ASP.NET Identity user name and password
    [Authorize] // Require some form of authentication
    public abstract class GenericODataController<TEntity, TKey> : ODataController where TEntity : class, IODataEntity<TEntity, TKey>, new()
    {
        #region Fields

        #endregion

        #region Properties

        protected virtual NorthwindContext Db { get; } = new NorthwindContext();

        #endregion

        #region Public Methods

        [EnableQuery]
        public virtual IQueryable<TEntity> Get()
        {
            return Db.Set<TEntity>();
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Get([FromODataUri] TKey key)
        {
            //// Can't compare using key == p.ID because of error: operator '==' cannot be applied to operands of type 'TKey' and 'TKey'
            var result = await Db.Set<TEntity>().Where(new TEntity().HasID(key)).FirstOrDefaultAsync();
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);

            //return SingleResult.Create(result);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Post(TEntity entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Db.Set<TEntity>().Add(entity);

                await Db.SaveChangesAsync();

                return Created(entity);
            }
            catch (Exception e)
            {
                while (e.InnerException != null)
                {
                    e = e.InnerException;
                }

                return BadRequest(e.Message);
            }
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] TKey key, Delta<TEntity> fullEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            object id;
            if (!fullEntity.TryGetPropertyValue("ID", out id) || id == null || !key.Equals(Guid.Parse(id.ToString())))
            {
                return BadRequest();
            }
            
            var entity = await Db.Set<TEntity>().FirstOrDefaultAsync(new TEntity().HasID(key));

            if (entity == null)
            {
                return NotFound();
            }

            fullEntity.Put(entity);

            Db.Entry(entity).State = EntityState.Modified;

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                {
                    return NotFound();
                }

                throw;
            }

            return Updated(entity);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] TKey key, Delta<TEntity> partialEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Db.Set<TEntity>().FirstOrDefaultAsync(new TEntity().HasID(key));

            if (entity == null)
            {
                return NotFound();
            }

            partialEntity.Patch(entity);

            try
            {
                await Db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EntityExists(key))
                {
                    return NotFound();
                }

                throw;
            }

            return Updated(entity);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Delete([FromODataUri] TKey key)
        {
            var product = await Db.Set<TEntity>().FirstOrDefaultAsync(new TEntity().HasID(key));

            if (product == null)
            {
                return NotFound();
            }

            Db.Set<TEntity>().Remove(product);

            await Db.SaveChangesAsync();

            return StatusCode(HttpStatusCode.NoContent);
        }

        #endregion

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            Db.Dispose();
            base.Dispose(disposing);
        }

        protected virtual bool EntityExists(TKey key)
        {
            return Db.Set<TEntity>().Any(new TEntity().HasID(key));
        }

        #endregion
    }
}