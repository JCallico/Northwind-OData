using System;
using System.Collections.Generic;
using GSA.Samples.Northwind.OData.Model;

namespace GSA.Samples.Northwind.OData.Swagger
{
    public class SwaggerDataDictionary
    {
        private static readonly Dictionary<Type, object> _defaultValues = new Dictionary<Type, object>();
        private static readonly Dictionary<Tuple<Type, string>, object> _values = new Dictionary<Tuple<Type, string>, object>();

        static SwaggerDataDictionary()
        {
            //// Registering default values

            RegisterDefaultValue<Category>(new
            {
                CategoryID =  1000,
                CategoryName = "Computers",
                Description = "Microsoft Surface Pro 4 i5 (128GB, 4GB RAM)"
            });

            RegisterDefaultValue<Product>(new
            {
                ID = Guid.Parse("3bccb948-1da6-419b-831f-02fea5207abe"),
                ProductID = 26,
                ProductName = "Gumbär Gummibärchen",
                SupplierID =  11,
                CategoryID = 3,
                QuantityPerUnit = "100 - 250 g bags",
                UnitPrice = 31.23,
                UnitsInStock= 15,
                UnitsOnOrder = 0,
                ReorderLevel = 0,
                Discontinued = false,
                ReferenceUniqueID = Guid.Parse("1bc2c388-c795-4506-ad2a-ecee44f858e9"),
                ProductUri = "http://www.pdf995.com/samples/pdf.pdf"
            });
        }

        public virtual object GetDefaultValue<T>()
        {
            return GetDefaultValue(typeof(T));
        }

        public virtual object GetDefaultValue(Type type)
        {
            object value;
            if (_defaultValues.TryGetValue(type, out value))
            {
                return value;
            }

            return null;
        }

        public virtual object GetValue<T>(string key)
        {
            return GetValue(typeof(T), key);
        }

        public virtual object GetValue(Type type, string key)
        {
            object value;
            if (_values.TryGetValue(new Tuple<Type, string>(type, key), out value))
            {
                return value;
            }

            return null;
        }

        protected static void RegisterDefaultValue<T>(T value)
        {
            _defaultValues.Add(typeof(T), value);
        }

        protected static void RegisterDefaultValue<T>(object value)
        {
            _defaultValues.Add(typeof(T), value);
        }

        protected static void RegisterValue<T>(string key, T value)
        {
            _values.Add(new Tuple<Type, string>(typeof(T), key), value);
        }

        protected static void RegisterValue<T>(string key, object value)
        {
            _values.Add(new Tuple<Type, string>(typeof(T), key), value);
        }
    }
}
