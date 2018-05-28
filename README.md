# CustomVisionWpfApp
Azure Cognitive Services - Custom Vison Service (https://customvision.ai/) の Object Detection 機能 (2018.05.25 時点で Limited Preview) を使った以下のような WPF アプリケーションです。バイナリのインストーラーは、<a href="./Installer.zip">こちら</a>からダウンロードしてください。Custom Vision Service - Object Detection の使い方が分からない方は、<a href="./CustomVisionObjectDetection.pdf">こちら</a>をご確認ください。
<br><br>
<img src="./images/ap01.png" />
<br><br><br>
CustomVisionWpfApp を初めて起動すると、以下の設定画面が表示されます。Custom Vision Services で、オブジェクトを学習後、「PERFORMANCE」タブの 「Prediction URL」をクリックして、そこに表示される情報を使い、設定画面の以下項目について入力し、「設定の保存」をクリックしてください。Custom Vision Services で、再学習を実行した場合、IterationId が新しく生成される為、「ファイル」メニューの「設定画面の表示」を選択し、再設定してください。
<br><br>
<img src="./images/ap03.png" />
<br><br><br>
※オブジェクトが異なれば、矩形とテキストの色が変わります
<br>
※オブジェクトをポイントすると、詳細が表示されます
