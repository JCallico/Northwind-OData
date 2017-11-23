using System.Web.OData.Query;

namespace GSA.Samples.Northwind.OData.Common.Extensions
{
    public static class ODataQueryOptionsExtensions
    {
        public static void GetPagingOptions(this ODataQueryOptions queryOptions, out int page, out int pageSize, int defaultPageSize = 50)
        {
            pageSize = (queryOptions.Skip != null ? int.Parse(queryOptions.Skip.RawValue) : 0) +
                       (queryOptions.Top != null ? int.Parse(queryOptions.Top.RawValue) : 0);

            if (pageSize == 0)
            {
                pageSize = defaultPageSize;
            }

            page = (queryOptions.Skip?.Value ?? 0) / pageSize + 1;
        }
    }
}