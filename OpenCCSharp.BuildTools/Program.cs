using OpenCCSharp.BuildTools;
using System.CommandLine;

var rootCommand = new RootCommand("OpenCCSharp build tools. This tool has dependency on OpenCCSharp.Conversion.")
{
    BuildPresetDictionariesCommand.GetCommand(),
};

return await rootCommand.InvokeAsync(args);
