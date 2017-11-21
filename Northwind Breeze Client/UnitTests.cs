using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading;


using Breeze.Sharp.Core;
using Breeze.Sharp;
using GSA.Samples.Northwind.OData.Models;
using System.Threading.Tasks;
using System.Linq;

namespace Northwind_Breeze_Client
{


    [TestClass]
    public class QueryTests
    {
        // Useful well-known data
        private readonly Guid _tofuID = Guid.Parse("DB052D81-B81B-4A97-BDF0-0F917733F5F7");

        private String _serviceName;

        [TestInitialize]
        public void TestInitializeMethod()
        {
            Configuration.Instance.ProbeAssemblies(typeof(Product).Assembly);
            _serviceName = "http://localhost:57088/breeze/Northwind/";
        }

        [TestCleanup]
        public void TearDown()
        {
        }

        #region Metadata

        [TestMethod]
        public void MetadataNeededToGetEntityKey()
        {

            // Metadata is necessary to get entity key
            // Must be first test to be meaningful, since CanFetchMetadata() below fetches 
            // metadata into the static instance of MetadataStore
            var entityManager = new EntityManager(_serviceName);


            var ProductType = entityManager.MetadataStore.GetEntityType(typeof(Product));
            Assert.IsNotNull(ProductType);
            EntityKey key;
            try
            {
                key = new EntityKey(ProductType, _tofuID);
                Assert.Fail("EntityKey constructor should fail if metadata not fetched");
            }
            catch (Exception e)
            {
                Assert.IsTrue(e.Message.Contains("There are no KeyProperties yet defined"), "Thrown exception should indicated key property is not defined.  Instead it says \"" + e.Message + "\"");
            }

            var etb = new EntityTypeBuilder<Product>(entityManager.MetadataStore);
            etb.DataProperty(c => c.ID).IsPartOfKey();
            key = new EntityKey(ProductType, _tofuID);
        }

        [TestMethod]
        public async Task CanGetMetadata()
        {

            // Confirm the metadata can be fetched from the server
            var entityManager = new EntityManager(_serviceName);
            var dataService = await entityManager.FetchMetadata();
            Assert.IsNotNull(dataService);
        }

        #endregion Metadata

        #region Basic queries

        [TestMethod]
        public async Task AllProducts_Task()
        {
            var entityManager = new EntityManager(_serviceName);

            // All instances of Product
            var query = new EntityQuery<Product>();                        // Alternate way to create a basic EntityQuery

            // Handle async Task results explicitly
            await entityManager.ExecuteQuery(query).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var message = TestFns.FormatException(task.Exception);
                    Assert.Fail(message);
                }
                else
                {
                    var count = task.Result.Count();
                    Assert.IsTrue(count > 0, "Product query returned " + count + " Products");
                }
            });
        }

        [TestMethod]
        public async Task Products_Slice()
        {
            var entityManager = new EntityManager(_serviceName);

            // All instances of Product
            var query = new EntityQuery<Product>().OrderBy(i=>i.ProductName).Skip(10).Take(20);                        

            // Handle async Task results explicitly
            await entityManager.ExecuteQuery(query).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var message = TestFns.FormatException(task.Exception);
                    Assert.Fail(message);
                }
                else
                {
                    var count = task.Result.Count();
                    Assert.IsTrue(count == 20, "Product query returned " + count + " Products");
                }
            });
        }

        [TestMethod]
        public async Task Products_InlineCount()
        {
            var entityManager = new EntityManager(_serviceName);

            // All instances of Product
            var query = new EntityQuery<Product>().Where(i=>i.ProductName.StartsWith("A")).InlineCount();

            // Handle async Task results explicitly
            await entityManager.ExecuteQuery(query).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    var message = TestFns.FormatException(task.Exception);
                    Assert.Fail(message);
                }
                else
                {
                    var count = ((IHasInlineCount)task.Result).InlineCount;
                    Assert.IsTrue(count == 2, "There are " + count + " products with name starting with 'A'");
                }
            });
        }

        [TestMethod]
        public async Task AllProducts_Exceptions()
        {
            var entityManager = new EntityManager(_serviceName);

            // All instances of Product
            var query = new EntityQuery<Product>();

            // Capture result using try-catch
            try
            {
                var results = await entityManager.ExecuteQuery(query);
                var count = results.Count();
                Assert.IsTrue(count > 0, "Product query returned " + count + " Products");
            }
            catch (Exception e)
            {
                var message = TestFns.FormatException(e);
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public async Task CanFetchEntityTwice()
        {
            var entityManager = new EntityManager(_serviceName);
            await entityManager.FetchMetadata();

            var ProductType = entityManager.MetadataStore.GetEntityType(typeof(Product));
            var key = new EntityKey(ProductType, _tofuID);

            // Fetch same entity twice
            var result = await entityManager.FetchEntityByKey(key);
            Assert.IsNotNull(result.Entity, "Entity fetched by key should not be null");
            Assert.IsFalse(result.FromCache, "Entity fetched remotely should not be from cache");

            result = await entityManager.FetchEntityByKey(key);
            Assert.IsNotNull(result.Entity, "Entity re-fetched by key should not be null");
            Assert.IsFalse(result.FromCache, "Entity re-fetched remotely should not be from cache");
        }

        [TestMethod]
        public async Task QueryEntityTwice()
        {
            var entityManager = new EntityManager(_serviceName);
            await entityManager.FetchMetadata();

            // Query entity twice
            var query1 = new EntityQuery<Product>().Where(c => c.ID == _tofuID);
            var tofu1 = (await entityManager.ExecuteQuery(query1)).FirstOrDefault();
            Assert.IsNotNull(tofu1, "Alfred should be found by Id");

            var query2 = new EntityQuery<Product>().Where(c => c.ProductName == tofu1.ProductName);
            var alfred2 = (await entityManager.ExecuteQuery(query2)).FirstOrDefault();
            Assert.IsTrue(ReferenceEquals(tofu1, alfred2), "Successive queries should return same entity");
        }


        #endregion Basic queries


        [TestMethod]
        public async Task SaveNewEntity()
        {
            var entityManager = await TestFns.NewEm(_serviceName);

            // Create a new customer
            var product = new Product();
            product.ID = Guid.NewGuid();
            product.ProductName = "Test1 " + DateTime.Now.ToString();
            product.Discontinued = false;
            entityManager.AddEntity(product);
            Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Added, "State of new entity should be Added");

            try
            {
                var saveResult = await entityManager.SaveChanges();
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Unchanged, "State of saved entity should be Unchanged");
            }
            catch (Exception e)
            {
                var message = "Server should not have rejected save of Customer entity with the error " + e.Message;
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public async Task SaveModifiedEntity()
        {
            var entityManager = await TestFns.NewEm(_serviceName);

            // Create a new customer
            var product = new Product { ID= Guid.NewGuid() };
            entityManager.AddEntity(product);
            product.ProductName = "Test2A " + DateTime.Now.ToString();
            Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Added, "State of new entity should be Added");

            try
            {
                var saveResult = await entityManager.SaveChanges();
                var savedEntity = saveResult.Entities[0];
                Assert.IsTrue(savedEntity is Product && savedEntity == product, "After save, added entity should still exist");
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Unchanged, "State of saved entity should be Unchanged");

                // Modify customer
                product.ProductName = "Test2M " + DateTime.Now.ToString();
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Modified, "State of modified entity should be Modified");

                saveResult = await entityManager.SaveChanges();
                savedEntity = saveResult.Entities[0];
                Assert.IsTrue(savedEntity is Product && savedEntity == product, "After save, modified entity should still exist");
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Unchanged, "State of saved entity should be Unchanged");

            }
            catch (Exception e)
            {
                var message = string.Format("Save of customer {0} should have succeeded;  Received {1}: {2}",
                                            product.ProductName, e.GetType().Name, e.Message);
                Assert.Fail(message);
            }
        }

        [TestMethod]
        public async Task SaveDeletedEntity()
        {
            var entityManager = await TestFns.NewEm(_serviceName);

            // Create a new customer
            var product = new Product { ID = Guid.NewGuid() };
            entityManager.AddEntity(product);
            product.ProductName = "Test3A " + DateTime.Now.ToString();
            Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Added, "State of new entity should be Added");

            try
            {
                var saveResult = await entityManager.SaveChanges();
                var savedEntity = saveResult.Entities[0];
                Assert.IsTrue(savedEntity is Product && savedEntity == product, "After save, added entity should still exist");
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Unchanged, "State of saved entity should be Unchanged");

                // Delete customer
                product.EntityAspect.Delete();
                Assert.IsTrue(product.EntityAspect.EntityState == EntityState.Deleted,
                              "After delete, entity state should be deleted, not " + product.EntityAspect.EntityState.ToString());
                saveResult = await entityManager.SaveChanges();
                savedEntity = saveResult.Entities[0];
                Assert.IsTrue(savedEntity.EntityAspect.EntityState == EntityState.Detached,
                              "After save of deleted entity, entity state should be detached, not " + savedEntity.EntityAspect.EntityState.ToString());

            }
            catch (Exception e)
            {
                var message = string.Format("Save of deleted customer {0} should have succeeded;  Received {1}: {2}",
                                            product.ProductName, e.GetType().Name, e.Message);
                Assert.Fail(message);
            }
        }

    }
}

