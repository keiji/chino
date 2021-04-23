# Cappuccino
Cappuccino is an yet another Exposure Notification Library for COCOA.

## これはなに？
「接触確認（Exposure Notification: EN） API」を実際に触っていて、AndroidとiOSの差異を確認しながら作っていたXamarin用のENライブラリです。

Xamarin.ExposureNotificationではAndroid/iOSのバックグラウンド処理もライブラリ側でハンドリングしています。
Cappuccinoは、バックグラウンド処理は行わず、Android/iOSの薄いラッパーとして動作することを目指しています（バックグラウンド処理はアプリ側で開発する必要があります）。

## 誰が作っているの？
ここまでは @keiji が開発してきました。
COPYRIGHT HOLDERは Cappuccino Authors とする予定です。

必要に応じてリポジトリを移管する可能性があります。

## COCOAに採用されるの？
あくまで個人的に作っているものなので、COCOAに採用されるかはわかりません
（そもそも現在のCappuccinoはプロダクトに使える品質に達していません）。

その上で、ENv2などの検討にあたって、COCOAよりももっと小さなコードで各機能をテストできる環境の必要性は常々感じています。
たとえCOCOAに採用されなくても、本プロジェクトの意義はあると考えています。

## License

Cappuccino is distributed under the terms of the MIT License.