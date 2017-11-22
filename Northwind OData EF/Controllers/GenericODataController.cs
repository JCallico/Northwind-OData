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
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Controllers
{
    [KeyAndSecretBasicAuthentication] // Enable authentication via an ASP.NET Identity user name and password
    [Authorize] // Require some form of authentication
    public abstract class GenericODataController<E, TKey> : ODataController where E : class, IODataEntity<TKey>
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
        public virtual SingleResult<E> Get([FromODataUri] TKey key)
        {
            //// Can't compare using key == p.ID because of error: operator '==' cannot be applied to operands of type 'TKey' and 'TKey'
            //// using Contains instead which will be translated to SQL as an equality comparison if the list contains one element
            var result = Db.Set<E>().Where(p => new[] { key }.Contains(p.ID));
            return SingleResult.Create(result);
        }

        [EnableQuery]
        public virtual async Task<IHttpActionResult> Post(E entity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Db.Set<E>().Add(entity);

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
        public virtual async Task<IHttpActionResult> Put([FromODataUri] TKey key, Delta<E> fullEntity)
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

            var entity = await Db.Set<E>().FirstOrDefaultAsync(p => new[] { key }.Contains(p.ID));

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
        public virtual async Task<IHttpActionResult> Patch([FromODataUri] TKey key, Delta<E> partialEntity)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entity = await Db.Set<E>().FirstOrDefaultAsync(p => new[] { key }.Contains(p.ID));

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
            var product = await Db.Set<E>().FirstOrDefaultAsync(p => new[] { key }.Contains(p.ID));

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

        protected virtual bool EntityExists(TKey key)
        {
            return Db.Set<E>().Any(p => new[] { key }.Contains(p.ID));
        }

        #endregion
    }
}