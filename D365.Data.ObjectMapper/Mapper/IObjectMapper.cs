namespace D365.Data.ObjectMapper.Mapper
{
    internal interface IObjectMapper<T>
         where T : class, new()
    {
        T Map(object entityOrEntityCollection);

        bool TryParseEntity<TEntity>(T obj, out TEntity entity)
             where TEntity : class, new();
    }
}
