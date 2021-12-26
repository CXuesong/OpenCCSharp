using System;
using System.Collections.Concurrent;
using System.Text.Json;
using OpenCCSharp.Conversion;

namespace OpenCCSharp.Presets
{

    /// <summary>
    /// Provides <see cref="ScriptConverterBase"/> factories for conversion between different variants of Chinese text.
    /// </summary>
    public static class ChineseConversionPresets
    {
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
        /// <item><term>Hans-Hani</term></item>
        /// <item><term>Hans-HK</term></item>
        /// <item><term>Hans-TW</term></item>
        /// <item><term>Hant-Hans</term></item>
        /// <item><term>Hant-Hani</term></item>
        /// <item><term>HK-Hans</term></item>
        /// <item><term>TW-Hans</term></item>
        /// </list>
        /// </remarks>
        public static async ValueTask<ScriptConverterBase> GetConverterAsync(ChineseConversionVariant fromVariant, ChineseConversionVariant toVariant)
        {
            var configFileName = (fromVariant, toVariant) switch
            {
                (ChineseConversionVariant.Hans, ChineseConversionVariant.Hant) => "Hans-Hant.json",
                (ChineseConversionVariant.Hans, ChineseConversionVariant.Hani) => "Hans-Hani.json",
                (ChineseConversionVariant.Hans, ChineseConversionVariant.HK) => "Hans-HK.json",
                (ChineseConversionVariant.Hans, ChineseConversionVariant.TW) => "Hans-TW.json",
                (ChineseConversionVariant.Hant, ChineseConversionVariant.Hans) => "Hant-Hans.json",
                (ChineseConversionVariant.Hant, ChineseConversionVariant.Hani) => "Hant-Hani.json",
                (ChineseConversionVariant.Hani, ChineseConversionVariant.Hant) => "Hani-Hant.json",
                (ChineseConversionVariant.HK, ChineseConversionVariant.Hans) => "HK-Hans.json",
                (ChineseConversionVariant.TW, ChineseConversionVariant.Hans) => "TW-Hans.json",
                _ => throw new ArgumentException($"Invalid conversion pair: {fromVariant} -> {toVariant}."),
            };
            var converter = await ConversionPresetHelper.CreateConverterFromAsync(configFileName);
            return converter;
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
        /// <summary>Japanese Kanji (Shinjitai).【新字体（しんじたい）】</summary>
        Hani,
        /// <summary>Traditional Chinese, with character/phrase variants in Hongkong.（香港繁體）</summary>
        HK,
        /// <summary>Traditional Chinese, with character/phrase variants in Taiwan.（台灣正體）</summary>
        TW,
    }

}
