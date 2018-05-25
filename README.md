# CustomVisionWpfApp
Azure Cognitive Services - Custom Vison Service (https://customvision.ai/) の Object Detection 機能 (2018.05.25 時点で Limited Preview) を使った以下のような WPF アプリケーションです。
<img src="images/ap01.png" />

Custom Vision Services で、オブジェクトを学習後、PERFORMANCE タブの Prediction URL をクリックして、そこに表示される情報を使い、MainWindow.xaml.cs の以下変数の値「xxxxx～xxxxx」を修正してください。

        static readonly string predictionKeyValue = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        static readonly string iterationId = "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx";
        static readonly string predictionUri =
             "https://southcentralus.api.cognitive.microsoft.com/customvision/v2.0/Prediction/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx/image?"
            + $"iterationId={iterationId}";

それ以外に修正の必要はありません。
オブジェクトが異なれば、矩形とテキストの色が変わります。
