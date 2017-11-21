# Northwind OData

Exploring what it takes to expose a MS-SQL database as an OData Service.

Implementations included:

1. Microsoft.AspNet.OData and Entity Framework (Most complete).
2. Microsoft.AspNet.OData and Dapper (Doesn't fully support certain commands because needed IQueryable functionality not implemented out of box by Dapper).
3. Breeze and Entity Framework (Doesn't currently support OData commands like $count,  $skip amoung other).

### Change log

- Only the Products table is supported now.
- Added new column named ProductUniqueID of type uniqueidentifier (Required by Dynamics 365 OData Sources).
- Added new columns ReferenceUniqueID and ProductUri to test Dynamics 365 Virtual Entities.
- Added implementation using Breeze and Entity Framework.