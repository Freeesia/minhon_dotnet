using Refit;

namespace minhon;

public interface IMachineTranslationClient
{
    [Post("/api/mt/general_{source}_{target}/")]
    Task<string> Translate(Language source, Language target, TranslateRequest request);
}

public record TranslateRequest(string Key, string Name, string Text)
{
    public string Type { get; } = "json";
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
