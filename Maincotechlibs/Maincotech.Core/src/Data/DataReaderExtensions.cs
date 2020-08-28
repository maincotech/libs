namespace System.Data
{
    public static class DataReaderExtensions
    {
        public static T? GetNullableValue<T>(this IDataRecord reader, string fieldName) where T : struct
        {
            var index = reader.GetOrdinal(fieldName);
            T? nullable;
            if (reader.IsDBNull(index))
            {
                nullable = null;
            }
            else
            {
                nullable = (T)reader.GetValue(index);
            }
            return nullable;
        }
    }
}