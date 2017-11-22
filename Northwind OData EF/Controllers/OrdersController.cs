using System;
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Controllers
{
    public class OrdersController : GenericODataController<Order, int>
    {
    }
}