using System;

namespace GSA.Samples.Northwind.OData.Common.Extensions
{
    public static class GuidExtensions 
    {
        public static Guid ToGuid(this int value, Guid baseValue = default(Guid))
        {
            byte[] bytes = baseValue.ToByteArray();
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        public static int ToInt(this Guid value, Guid baseValue = default(Guid))
        {
            byte[] bytes = baseValue.ToByteArray();
            value.ToByteArray().CopyTo(bytes, 0);
            int bint = BitConverter.ToInt32(bytes, 0);
            return bint;
        }
    }
}