# CustomVisionWpfApp - 誰でもできるディープ ラーニング
Azure Cognitive Services - Custom Vison Service (https://customvision.ai/) の Object Detection 機能 (2018.07.12 時点で Preview) を使うと、従来、TensorFlow や CNTK などのディープラーニング ツールキットを使ってモデル構築をしないとできなかったことが、GUI だけで誰でもモデル構築を実現できるようになりました。
<br><br>
Custom Vision Service - Object Detection 機能の使い方と CustomVisionWpfApp の初期設定方法は、<a href="./CustomVisionObjectDetection.pdf">こちらの PDF ファイル</a>をご確認ください。
<br><br>
Custom Vison Service で作成した Web API を使って実現できる以下のような WPF アプリケーションを公開しています。
<br>
インストーラーは、<a href="./Installer.zip">こちら</a>からダウンロードしてください。
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
