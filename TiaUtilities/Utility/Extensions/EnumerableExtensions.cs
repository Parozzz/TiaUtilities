namespace TiaXmlReader.Utility.Extensions
{
    /// <summary>
    /// Helper methods for the lists.
    /// From https://stackoverflow.com/questions/11463734/split-a-list-into-smaller-lists-of-n-size
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IEnumerable<List<T>> ChunkBy<T>(this IEnumerable<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList());
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static T FirstOrElse<T>(this IEnumerable<T> source, Func<T> func)
        {
            try
            {
                return source.First();
            }
            catch
            {
                return func.Invoke();
            }
        }
    }
}
