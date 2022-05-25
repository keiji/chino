# Cappuccino
Cappuccino is an yet another Exposure Notification Library for COCOA.

[![CI](https://github.com/keiji/chino/actions/workflows/main.yml/badge.svg)](https://github.com/keiji/chino/actions/workflows/main.yml)

## これはなに？
「接触確認（Exposure Notification: EN） API」を実際に触っていて、AndroidとiOSの差異を確認しながら作っていたXamarin用のENライブラリです。

Xamarin.ExposureNotificationではAndroid/iOSのバックグラウンド処理もライブラリ側でハンドリングしています。
Cappuccinoは、バックグラウンド処理は行わず、Android/iOSの薄いラッパーとして動作することを目指しています（バックグラウンド処理はアプリ側で開発する必要があります）。

## 誰が作っているの？
ここまでは @keiji が開発してきました。
COPYRIGHT HOLDERは Cappuccino Authors としています。

必要に応じてリポジトリを移管する可能性があります。

## COCOAに採用されるの？
~~あくまで個人的に作っているものなので、COCOAに採用されるかはわかりません
（そもそも現在のCappuccinoはプロダクトに使える品質に達していません）~~ COCOAに採用されました。

その上で、ENv2などの検討にあたって、COCOAよりももっと小さなコードで各機能をテストできる環境の必要性は常々感じています。
たとえCOCOAに採用されなくても、本プロジェクトの意義はあると考えています。

[Prism(Xamarin.Forms)から利用するサンプル実装](https://github.com/keiji/chino.prism)

## サンプルアプリ（Smaple.*）について
リポジトリにはCappuccinoを利用したサンプルアプリが含まれています。

サンプルアプリは、Sample.Android, Sample.iOS, Sample.Commonのプロジェクトで構成されます。

### Edit `Sample.Common/ServerConfiguration.cs`
サンプルアプリにはサーバーと連携して診断キーのアップロード、ダウンロードをする機能があります。

連携するサーバーをカスタムするには、上記の実装のサーバーを用意した上で`ApiEndpoint` を書き換えてください。
（デフォルト値 `https://en.keiji.dev/diagnosis_keys` は、動作確認用に用意したサーバーです）

`ClusterId` はサーバーを仮想的に区切るための値で、6桁の数字を指定します。

```
using Newtonsoft.Json;

namespace Sample.Common
{
    [JsonObject]
    public class ServerConfiguration
    {
        [JsonProperty("api_endpoint")]
        public string ApiEndpoint = "https://en.keiji.dev/diagnosis_keys";

        [JsonProperty("cluster_id")]
        public string ClusterId = "212458"; // 6 digits
    }
}
```

また、一度サンプルアプリを起動すると端末内に作成される`config/server_configuration.json`を書き換えると、設定値をオーバーライドできます。

サーバーの実装の詳細は次のURLを参照してください。

 * https://github.com/keiji/en-calibration-server


## License

Cappuccino is distributed under the terms of the MIT License.
