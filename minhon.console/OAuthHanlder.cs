using System.Net;
using System.Text;
using IdentityModel;


internal class OAuthHanlder(string consumerKey, string consumerSecret, HttpMessageHandler innerHandler) : DelegatingHandler(innerHandler)
{
    private static readonly Random NonceRandom = new();
    private readonly string consumerKey = consumerKey;
    private readonly string consumerSecret = consumerSecret;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Dictionary<string, string> parameter = new()
        {
            { "oauth_consumer_key", consumerKey },
            { "oauth_signature_method", "HMAC-SHA1" },
            //epoch秒
            { "oauth_timestamp", DateTime.UtcNow.ToEpochTime().ToString() },
            { "oauth_nonce", NonceRandom.Next(123400, 9999999).ToString() },
            { "oauth_version", "1.0" }
        };
        if (request.Content is { } content)
        {
            var @params = await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            foreach (var pair in @params.Split('&').Select(x => x.Split('=')))
            {
                parameter.Add(pair[0], WebUtility.UrlDecode(pair[1]));
            }
        }
        parameter.Add("oauth_signature", CreateSignature(request.Method.Method, request.RequestUri!, parameter));
        //HTTPリクエストのヘッダに追加
        StringBuilder sb = new();
        foreach (KeyValuePair<string, string> item in parameter)
        {
            //各種情報のうち、oauth_で始まる情報のみ、ヘッダに追加する。各情報はカンマ区切り、データはダブルクォーテーションで括る
            if (item.Key.StartsWith("oauth_"))
            {
                sb.AppendFormat("{0}=\"{1}\",", item.Key, WebUtility.UrlEncode(item.Value));
            }
        }
        request.Headers.Authorization = new("OAuth", sb.ToString());

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    private string CreateSignature(string method, Uri uri, Dictionary<string, string> parameter)
    {
        //パラメタをソート済みディクショナリに詰替（OAuthの仕様）
        SortedDictionary<string, string> sorted = new(parameter);
        //URLエンコード済みのクエリ形式文字列に変換
        string paramString = CreateQueryString(sorted);
        //アクセス先URLの整形
        string url = string.Format("{0}://{1}{2}", uri.Scheme, uri.Host, uri.AbsolutePath);
        //署名のベース文字列生成（&区切り）。クエリ形式文字列は再エンコードする
        string signatureBase = string.Format("{0}&{1}&{2}", method, WebUtility.UrlEncode(url), WebUtility.UrlEncode(paramString));
        //署名鍵の文字列をコンシューマー秘密鍵とアクセストークン秘密鍵から生成（&区切り。アクセストークン秘密鍵なくても&残すこと）
        string key = WebUtility.UrlEncode(consumerSecret) + "&";
        //鍵生成＆署名生成
        System.Security.Cryptography.HMACSHA1 hmac = new(Encoding.ASCII.GetBytes(key));
        byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(signatureBase));
        return Convert.ToBase64String(hash);
    }

    private static string CreateQueryString(IDictionary<string, string> param)
    {
        if (param == null || param.Count == 0)
            return string.Empty;

        StringBuilder query = new();
        foreach (string key in param.Keys)
        {
            query.AppendFormat("{0}={1}&", WebUtility.UrlEncode(key), WebUtility.UrlEncode(param[key]));
        }
        return query.ToString(0, query.Length - 1);
    }
}
