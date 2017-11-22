# Northwind OData

Exploring what it takes to expose a MS-SQL database as an OData Service.

Implementations included:

1. Microsoft.AspNet.OData and Entity Framework (Most complete).
2. Microsoft.AspNet.OData and Dapper (Doesn't fully support certain commands because needed IQueryable functionality not implemented out of box by Dapper).
3. Breeze and Entity Framework (Doesn't currently support OData commands like $count,  $skip amoung other).

See Product entity for an example of how to support different key properties for EF and OData.

- The property ProductID is the internal EF key used for navigation properties and relationships.
- The property ID, which maps to the ProductUniqueID column, is used as key for OData queries.

### Change log

- Only the Products table is supported now.
- Added new column named ProductUniqueID of type uniqueidentifier (Required by Dynamics 365 OData Sources).
- Added new columns ReferenceUniqueID and ProductUri to test Dynamics 365 Virtual Entities.
- Added implementation using Breeze and Entity Framework.
- Added all DB entities to model.
- Added generic controller which now supports entities with keys of different types.
- Added OrdersController and CustomersController to also test keys of types string and int (ProductsController uses a key of type Guid).



