using System.Text.Json;
using System.Text.Json.Serialization;
using Refit;

namespace minhon;

/// <summary>
/// みんなの翻訳APIクライアント
/// </summary>
public interface IMachineTranslationClient
{
    /// <summary>
    /// 翻訳を実行します。
    /// </summary>
    /// <param name="mode">翻訳モード(<seealso cref="TranslateMode"/>)</param>
    /// <param name="srcLang">翻訳元言語(<seealso cref="LanguageCode"/>)</param>
    /// <param name="tarLang">翻訳先言語(<seealso cref="LanguageCode"/>)</param>
    /// <param name="request">翻訳リクエスト情報</param>
    /// <returns>翻訳結果</returns>
    [Post("/api/mt/{mode}_{srcLang}_{tarLang}/")]
    Task<Respoonse<TranslateResult>> Translate(string mode, string srcLang, string tarLang, [Body(BodySerializationMethod.UrlEncoded)] TranslateRequest request);
}

/// <summary>
/// 翻訳リクエスト情報
/// </summary>
/// <param name="Key">API Key</param>
/// <param name="Name">ログインID</param>
/// <param name="AccessToken">アクセストークン</param>
/// <param name="Text">翻訳対象テキスト</param>
public record TranslateRequest(
    [property: AliasAs("key")] string Key,
    [property: AliasAs("name")] string Name,
    [property: AliasAs("access_token")] string AccessToken,
    [property: AliasAs("text")] string Text)
{
    /// <summary>
    /// レスポンスの形式
    /// </summary>
    [AliasAs("type")]
    public string Type { get; } = "json";
    // 文脈利用するとレスポンスのinformationが空配列になって型が合わないので無効化
    // [AliasAs("history")]
    // public string History { get; init; } = "0";
}

/// <summary>
/// 汎用レスポンス
/// </summary>
/// <typeparam name="T">結果の型</typeparam>
/// <param name="ResultSet">結果の情報</param>
public record Respoonse<T>(ResultSet<T> ResultSet);

/// <summary>
/// レスポンスフィールド
/// </summary>
/// <typeparam name="T">結果の型</typeparam>
/// <param name="Code">処理コード</param>
/// <param name="Message">処理メッセージ</param>
/// <param name="Request">リクエストフィールド</param>
/// <param name="Result">結果情報</param>
public record ResultSet<T>(int Code, string Message, Request? Request, T? Result);

/// <summary>
/// リクエストフィールド
/// </summary>
/// <param name="Url">URL</param>
public record Request(Uri Url)
{
    /// <summary>
    /// デシリアライズされていないデータ
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

/// <summary>
/// 翻訳結果
/// </summary>
/// <param name="Text">翻訳テキスト</param>
/// <param name="Brank">翻訳テキスト空状態</param>
/// <param name="Information">自動翻訳処理内容</param>
public record TranslateResult(string Text, int Brank, TranslateInformation Information);

/// <summary>
/// 自動翻訳処理内容
/// </summary>
/// <param name="SourceText">原文</param>
/// <param name="TargetText">訳文</param>
/// <param name="Sentences">文章(改行区切り)</param>
public record TranslateInformation(
    [property: JsonPropertyName("text-s")] string SourceText,
    [property: JsonPropertyName("text-t")] string TargetText,
    [property: JsonPropertyName("sentence")] IReadOnlyList<TranslateSentence> Sentences)
{
    /// <summary>
    /// デシリアライズされていないデータ
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

/// <summary>
/// 文章(改行区切り)要素
/// </summary>
/// <param name="SourceText">原文</param>
/// <param name="TargetText">訳文</param>
public record TranslateSentence(
    [property: JsonPropertyName("text-s")] string SourceText,
    [property: JsonPropertyName("text-t")] string TargetText)
{
    /// <summary>
    /// デシリアライズされていないデータ
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Params { get; set; }
}

/// <summary>
/// 言語コード
/// </summary>
public static class LanguageCode
{
    /// <summary>
    /// 日本語
    /// </summary>
    public const string Ja = "ja";

    /// <summary>
    /// 英語
    /// </summary>
    public const string En = "en";

    /// <summary>
    /// ドイツ語
    /// </summary>
    public const string De = "de";

    /// <summary>
    /// スペイン語
    /// </summary>
    public const string Es = "es";

    /// <summary>
    /// フランス語
    /// </summary>
    public const string Fr = "fr";

    /// <summary>
    /// イタリア語
    /// </summary>
    public const string It = "it";

    /// <summary>
    /// ポルトガル語
    /// </summary>
    public const string Pt = "pt";

    /// <summary>
    /// ロシア語
    /// </summary>
    public const string Ru = "ru";

    /// <summary>
    /// 中国語(簡体字)
    /// </summary>
    public const string ZhCn = "zh-CN";

    /// <summary>
    /// 中国語(繁体字)
    /// </summary>
    public const string ZhTw = "zh-TW";
}

/// <summary>
/// 翻訳モード
/// </summary>
public static class TranslateMode
{
    /// <summary>
    /// 汎用NT
    /// </summary>
    public const string GeneralNT = "generalNT";

    /// <summary>
    /// みん翻PE
    /// </summary>
    public const string MinnaPE = "minnaPE";

    /// <summary>
    /// 翻訳LM
    /// </summary>
    public const string TransLM = "transLM";
}
