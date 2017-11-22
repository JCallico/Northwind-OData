using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.Http;

using GSA.Samples.Northwind.OData.Models;
using GSA.Samples.Northwind.OData.Filters;

namespace GSA.Samples.Northwind.OData.Controllers
{
    [KeyAndSecretBasicAuthentication] // Enable authentication via an ASP.NET Identity user name and password
    [Authorize] // Require some form of authentication
    public abstract class GenericODataController<E> : ODataController where E : class, IEntity
    {
        #region Fields

        #endregion

        #region Properties

        protected virtual NorthwindContext Db { get; } = new NorthwindContext();

        #endregion

        #region Public Methods

        [EnableQuery]
        public virtual IQueryable<E> Get()
        {
            return Db.Set<E>();
        }

        [EnableQuery]
        public virtual SingleResult<E> Get([FromODataUri] Guid key)
        {
            var result = Db.Set<E>().Where(p => p.ID == key);
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Post(E entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Db.Set<E>().Add(entity);

            await Db.SaveChangesAsync();

            return Created(entity);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Put([FromODataUri] Guid key, E entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (key != entity.ID)
            {
                return BadRequest();
            }

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
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] Guid key, Delta<E> entityPatch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Db.Set<E>().FindAsync(key);

            if (entity == null)
            {
                return NotFound();
            }

            entityPatch.Patch(entity);

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
        public virtual async Task<IHttpActionResult> Delete([FromODataUri] Guid key)
        {
            var product = await Db.Set<E>().FindAsync(key);

            if (product == null)
            {
                return NotFound();
            }

            Db.Set<E>().Remove(product);

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

        protected virtual bool EntityExists(Guid key)
        {
            return Db.Set<E>().Any(p => p.ID == key);
        }

        #endregion
    }
}