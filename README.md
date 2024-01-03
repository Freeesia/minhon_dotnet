# minhon_dotnet

[![Package](https://github.com/Freeesia/minhon_dotnet/actions/workflows/dotnet-package.yml/badge.svg)](https://github.com/Freeesia/minhon_dotnet/actions/workflows/dotnet-package.yml)
[![NuGet version](https://badge.fury.io/nu/minhon.svg)](https://badge.fury.io/nu/minhon)

「みんなの自動翻訳」のWeb APIを.NETから利用するためのライブラリです。

## インストール

NuGetからインストールできます。

```
dotnet add package minhon
```

## 使い方

### APIキーの取得

[「みんなの自動翻訳」](https://mt-auto-minhon-mlt.ucri.jgn-x.jp/)にアクセスし、アカウントを作成します。     
アカウントを作成したら、[設定](https://mt-auto-minhon-mlt.ucri.jgn-x.jp/content/setting/user/edit/)からAPIキーおよびAPIシークレットを取得します。

### アクセストークンの取得

「みんなの自動翻訳」のWeb APIはOAuth2を利用しています。
以下では[IdentityModel.OidcClient](https://github.com/IdentityModel/IdentityModel.OidcClient)を利用してアクセストークンを取得します。

```cs
var httpClient = new HttpClient();
var response = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
    Address = "https://mt-auto-minhon-mlt.ucri.jgn-x.jp/oauth2/token.php",
    ClientId = apiKey, // 設定ページで取得したAPIキー
    ClientSecret = apiSecret, // 設定ページで取得したAPIシークレット
});
```

### 翻訳

```cs
// クライアントの作成
var client = RestService.For<IMachineTranslationClient>("https://mt-auto-minhon-mlt.ucri.jgn-x.jp");

// 翻訳
var res = await client.Translate(TranslateMode.TransLM, LanguageCode.Ja, LanguageCode.En, new(
    apiKey, // 設定ページで取得したAPIキー
    name, // ログインID
    response.AccessToken, // アクセストークン
    """
    「みんなの自動翻訳＠TexTra®」は、自動翻訳をみんなで育てるサイトです。
    あらかじめ登録された自動翻訳を試したり、自動でファイルを翻訳できたり、サイト上で翻訳エディタを使用して自分で翻訳することができます。
    ※「みんなの自動翻訳」は、国立研究開発法人情報通信研究機構の登録商標です（第6120510号）。
    ※「TexTra」は、国立研究開発法人情報通信研究機構の登録商標です（第5398294号）。
    ※国立研究開発法人情報通信研究機構 ユニバーサルコミュニケーション研究所 先進的音声翻訳研究開発推進センターは、情報セキュリティマネジメントシステムの国際規格である「ISO/IEC27001」の認証を取得しています。認証範囲は、「先進的音声翻訳研究開発推進センターの音声処理技術及び機械翻訳技術の研究開発、音声コーパス、対訳コーパス、並びに関連する言語コーパスの構築」です。
    """));
if (res is { ResultSet: { Result: { Information: { Sentences: { } sentences } } } })
{
    foreach (var sentence in sentences)
    {
        Console.WriteLine(sentence);
    }
}
else
{
    Console.WriteLine(res);
}
```
