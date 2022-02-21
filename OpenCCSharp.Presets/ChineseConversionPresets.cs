using System;
using System.Collections.Concurrent;
using System.Text.Json;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.Presets;

/// <summary>
/// Provides <see cref="ScriptConverterBase"/> factories for conversion between different variants of Chinese text.
/// </summary>
public static class ChineseConversionPresets
{
    private static string? GetConversionDefinitionFileName(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
        => (fromVariant, toVariant) switch
        {
            (ChineseConversionVariant.Hans, ChineseConversionVariant.Hant) => "Hans-Hant.json",
            (ChineseConversionVariant.Hans, ChineseConversionVariant.HK) => "Hans-HK.json",
            (ChineseConversionVariant.Hans, ChineseConversionVariant.TW) => "Hans-TW.json",
            (ChineseConversionVariant.Hant, ChineseConversionVariant.Hans) => "Hant-Hans.json",
            (ChineseConversionVariant.Hant, ChineseConversionVariant.HK) => "Hant-HK.json",
            (ChineseConversionVariant.Hant, ChineseConversionVariant.TW) => "Hant-TW.json",
            (ChineseConversionVariant.HK, ChineseConversionVariant.Hans) => "HK-Hans.json",
            (ChineseConversionVariant.HK, ChineseConversionVariant.Hant) => "HK-Hant.json",
            (ChineseConversionVariant.TW, ChineseConversionVariant.Hans) => "TW-Hans.json",
            (ChineseConversionVariant.TW, ChineseConversionVariant.Hant) => "TW-Hant.json",
            (ChineseConversionVariant.Kyujitai, ChineseConversionVariant.Shinjiatai) => "kyujitai-shinjitai.json",
            (ChineseConversionVariant.Shinjiatai, ChineseConversionVariant.Kyujitai) => "shinjitai-kyujitai.json",
            _ => null
        };

    /// <summary>
    /// Determines whether the specified conversion pair is supported in the preset.
    /// </summary>
    public static bool IsConversionSupported(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
        => GetConversionDefinitionFileName(fromVariant, toVariant) != null;

    /// <summary>
    /// Gets a <see cref="ScriptConverterBase"/> instance to convert the Chinese text from the specific variant.
    /// </summary>
    /// <param name="fromVariant">the variant to convert from.</param>
    /// <param name="toVariant">the variant to convert to.</param>
    /// <exception cref="ArgumentException">The specified conversion pair is not supported.</exception>
    /// <returns>
    /// a <see cref="ScriptConverterBase"/> instance to convert the Chinese text from the specific variant.
    /// Depending on the implementation, this can be a prior returned instance, or a completely new instance.
    /// </returns>
    /// <remarks>
    /// Currently, only the following <paramref name="fromVariant"/>-<paramref name="toVariant"/> conversion pairs are supported:
    /// <list type="bullet">
    /// <item><term>Hans-Hant</term></item>
    /// <item><term>Hans-HK</term></item>
    /// <item><term>Hans-TW</term></item>
    /// <item><term>Hant-Hans</term></item>
    /// <item><term>Hant-HK</term></item>
    /// <item><term>Hant-TW</term></item>
    /// <item><term>HK-Hans</term></item>
    /// <item><term>HK-Hant</term></item>
    /// <item><term>TW-Hans</term></item>
    /// <item><term>TW-Hant</term></item>
    /// <item><term>Kyujitai-Shinjiatai</term></item>
    /// <item><term>Shinjiatai-Kyujitai</term></item>
    /// </list>
    /// <para>Use <see cref="IsConversionSupported"/> to determine whether a conversion pair is supported.</para>
    /// </remarks>
    public static async ValueTask<ScriptConverterBase> GetConverterAsync(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
    {
        var configFileName = GetConversionDefinitionFileName(fromVariant, toVariant);
        if (configFileName == null)
            throw new ArgumentException($"Unsupported conversion pair: {fromVariant} -> {toVariant}.");
        var converter = await ConversionPresetHelper.CreateConverterFromAsync(configFileName);
        return converter;
    }

    /// <summary>
    /// Clears cached conversion dictionaries from runtime.
    /// </summary>
    public static void ClearConverterCache()
    {
        ConversionPresetHelper.ClearCache();
    }
}

public enum ChineseConversionVariant
{
    /// <summary>Invalid enum value.</summary>
    Invalid = 0,
    /// <summary>Simplified Chinese.（简体中文）</summary>
    Hans,
    /// <summary>(OpenCC standard) Traditional Chinese.（繁體中文）</summary>
    Hant,
    /// <summary>Japanese Kanji (Kyujitai).【旧字体（きゅうじたい）】</summary>
    Kyujitai,
    /// <summary>Japanese Kanji (Shinjitai).【新字体（しんじたい）】</summary>
    Shinjiatai,
    /// <summary>Traditional Chinese, with character/phrase variants in Hongkong.（香港繁體）</summary>
    HK,
    /// <summary>Traditional Chinese, with character/phrase variants in Taiwan.（台灣正體）</summary>
    TW,
}