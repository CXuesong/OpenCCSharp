using System.CommandLine;
using System.CommandLine.Invocation;
using OpenCCSharp.Conversion;
using TriesSharp.Collections;

namespace OpenCCSharp.BuildTools;

internal static class BuildPresetDictionariesCommand
{

    public static Command GetCommand()
    {
        var dictionaryListFileArg = new Argument<FileInfo>("dictionary-list-file");
        var sourceDirArg = new Argument<DirectoryInfo>("source-dir");
        var targetDirArg = new Argument<DirectoryInfo>("target-dir");
        var command = new Command("build-preset-dictionaries")
        {
            dictionaryListFileArg, sourceDirArg, targetDirArg
        };
        command.SetHandler(context => Execute(context,
            context.ParseResult.GetValueForArgument(dictionaryListFileArg),
            // Somehow the trailing quotation marks will be included in the parameter
            context.ParseResult.GetValueForArgument(sourceDirArg).FullName,
            context.ParseResult.GetValueForArgument(targetDirArg).FullName
        ));
        return command;
    }

    private static async Task Execute(InvocationContext context, FileInfo dictionaryListFile, string sourceDir, string targetDir)
    {
        if (!Directory.Exists(sourceDir)) throw new DirectoryNotFoundException($"Cannot find sourceDir: {sourceDir}.");
        Directory.CreateDirectory(targetDir);
        foreach (var line in File.ReadLines(dictionaryListFile.FullName))
        {
            var trimmed = line.Trim();
            if (trimmed.Length == 0 || trimmed.StartsWith('#'))
                continue;

            var segments = trimmed.Split("->");
            var srcOptions = segments[0].Split('|');
            var destOptions = segments[1].Split('|');
            var srcFile = Path.Join(sourceDir, srcOptions[0].Trim());
            var destFileName = destOptions[0].Trim();
            if (destFileName.StartsWith('.')) destFileName = Path.ChangeExtension(Path.GetFileName(srcFile), destFileName);
            var destFile = Path.Join(targetDir, destFileName);
            var reverse = srcOptions.Skip(1).Any(s => s.Trim().Equals("Reverse", StringComparison.OrdinalIgnoreCase));
            await EmitDictionary(context, srcFile, destFile, reverse);
        }
    }

    private static async ValueTask EmitDictionary(InvocationContext context, string sourceFile, string targetFile, bool reverse)
    {
        context.Console.Write($"{sourceFile} -> {targetFile}");
        if (reverse) context.Console.Write(" (Reverse)");

        var dict = new TrieStringPrefixDictionary();

        await using var sourceStream = new FileStream(sourceFile,
            FileMode.Open, FileAccess.Read, FileShare.Read, 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        await foreach (var (k, v) in PlainTextConversionLookupTable.EnumEntriesFromAsync(sourceStream))
        {
            if (reverse)
            {
                foreach (var v1 in v)
                {
                    var m = GC.AllocateUninitializedArray<char>(k.Length).AsMemory();
                    k.CopyTo(m);
                    dict.TryAdd(v1, m);
                }
            }
            else
            {
                var m = GC.AllocateUninitializedArray<char>(v[0].Length).AsMemory();
                v[0].CopyTo(m);
                dict.TryAdd(k, m);
            }
        }

        context.Console.WriteLine($" ({dict.Count:D} entries)");

        await using var targetStream = new FileStream(targetFile, FileMode.Create, FileAccess.Write, FileShare.Read, 4096,
            FileOptions.Asynchronous | FileOptions.RandomAccess);
        await TrieSerializer.Serialize(targetStream, dict.Trie);
        await targetStream.FlushAsync();
    }

}

