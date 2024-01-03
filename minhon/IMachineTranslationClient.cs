using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace minhon;

public interface IMachineTranslationClient
{
    [Post("/api/mt/{mode}_{srcLang}_{tarLang}/")]
    Task<Respoonse> Translate(string mode, string srcLang, string tarLang, [Body(BodySerializationMethod.UrlEncoded)] TranslateRequest request);
}

public record TranslateRequest(
    [property: AliasAs("key")] string Key,
    [property: AliasAs("name")] string Name,
    [property: AliasAs("access_token")] string AccessToken,
    [property: AliasAs("text")] string Text)
{
    [AliasAs("type")]
    public string Type { get; } = "json";
    // 文脈利用するとレスポンスのinformationが空配列になって型が合わないので無効化
    // [AliasAs("history")]
    // public string History { get; init; } = "0";
}

public record Respoonse(ResultSet ResultSet);
public record ResultSet(int Code, string Message, Request? Request, TranslateResult? Result);
public record Request(Uri Url)
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}
public record TranslateResult(string Text, int Brank, TranslateInformation Information);
public record TranslateInformation(
    [property: JsonPropertyName("text-s")] string SourceText,
    [property: JsonPropertyName("text-t")] string TargetText,
    [property: JsonPropertyName("sentence")] IReadOnlyList<TranslateSentence> Sentences)
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

public record TranslateSentence(
    [property: JsonPropertyName("text-s")] string SourceText,
    [property: JsonPropertyName("text-t")] string TargetText)
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

public static class LanguageCode
{
    public const string Ja = "ja";
    public const string En = "en";
    public const string De = "de";
    public const string Es = "es";
    public const string Fr = "fr";
    public const string It = "it";
    public const string Pt = "pt";
    public const string Ru = "ru";
    public const string ZhCn = "zh-CN";
    public const string ZhTw = "zh-TW";
}

public static class TranslateMode
{
    public const string GeneralNT = "generalNT";
    public const string MinnaPE = "minnaPE";
    public const string TransLM = "transLM";
}
