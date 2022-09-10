using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenCCSharp.UnitTest
{
    internal static class Utility
    {

        public static ValueTask<T[]> WhenAll<T>(IEnumerable<ValueTask<T>> tasks)
            => tasks is IReadOnlyList<ValueTask<T>> list ? WhenAll(list) : WhenAll(tasks.ToList());

        // https://stackoverflow.com/questions/45689327/task-whenall-for-valuetask
        public static async ValueTask<T[]> WhenAll<T>(IReadOnlyList<ValueTask<T>> tasks)
        {
            // We don't allocate the list if no task throws
            List<Exception>? exceptions = null;

            var results = new T[tasks.Count];
            for (var i = 0; i < tasks.Count; i++)
                try
                {
                    results[i] = await tasks[i].ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions ??= new List<Exception>(tasks.Count);
                    exceptions.Add(ex);
                }

            return exceptions is null
                ? results
                : throw new AggregateException(exceptions);
        }

    }
}
