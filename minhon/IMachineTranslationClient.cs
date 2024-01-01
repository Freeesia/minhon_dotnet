using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace minhon;

[Headers("Authorization: OAuth")]
public interface IMachineTranslationClient
{
    [Post("/api/mt/generalNT_{source}_{target}/")]
    Task<Respoonse> Translate(Language source, Language target, [Body(BodySerializationMethod.UrlEncoded)] TranslateRequest request);
}

public record TranslateRequest(
    [property: AliasAs("key")] string Key,
    [property: AliasAs("name")] string Name,
    [property: AliasAs("text")] string Text)
{
    [AliasAs("type")]
    public string Type { get; } = "json";
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
    [property: JsonPropertyName("text-s")] string TextS,
    [property: JsonPropertyName("text-t")] string TextT)
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

public enum Language : int
{
    none = -1,
    ja = 0,        // 日本語
    en = 1,        // 英語
    cn = 2,        // 中国語（簡体）
    tw = 3,        // 中国語（繁体）
    ko = 4,        // 韓国語
}
