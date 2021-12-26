using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenCCSharp.Presets;

internal static class Utility
{

    public static async IAsyncEnumerable<TResult> SelectAsync<T, TResult>(this IEnumerable<T> sequence, Func<T, ValueTask<TResult>> selector)
    {
        foreach (var item in sequence)
        {
            yield return await selector(item);
        }
    }

    public static async ValueTask<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> sequence)
    {
        var list = new List<T>();
        await foreach (var item in sequence)
        {
            list.Add(item);
        }
        return list;
    }

}