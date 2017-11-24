namespace GSA.Samples.Northwind.OData.Swagger
{
    public class SwaggerFactory
    {
        public static SwaggerFactory CurrentFactory { get; set; } = new SwaggerFactory();

        public SwaggerDataDictionary CreateDataDictionary()
        {
            return new SwaggerDataDictionary();
        }
    }
}
