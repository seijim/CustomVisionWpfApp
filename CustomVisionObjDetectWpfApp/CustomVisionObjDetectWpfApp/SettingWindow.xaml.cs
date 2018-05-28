using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CustomVisionObjDetectWpfApp
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window
    {
        public SettingWindow()
        {
            InitializeComponent();

            if ((string)Properties.Settings.Default["ApiEndpoint"] != string.Empty)
            {
                TextBoxApiEndpoint.Text = (string)Properties.Settings.Default["ApiEndpoint"];
                TextBoxApiIterationId.Text = (string)Properties.Settings.Default["ApiIterationId"];
                TextBoxApiPredictionKey.Text = (string)Properties.Settings.Default["ApiPredictionKey"];
                TextBoxProbability.Text = (string)Properties.Settings.Default["Probability"];
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default["ApiEndpoint"] = TextBoxApiEndpoint.Text.Trim();
            Properties.Settings.Default["ApiIterationId"] = TextBoxApiIterationId.Text.Trim();
            Properties.Settings.Default["ApiPredictionKey"] = TextBoxApiPredictionKey.Text.Trim();
            Properties.Settings.Default["Probability"] = TextBoxProbability.Text.Trim();

            double d;
            if (double.TryParse(TextBoxProbability.Text, out d))
            {
                Properties.Settings.Default.Save();

                MainWindow.predictionEndpoint = (string)Properties.Settings.Default["ApiEndpoint"];
                MainWindow.iterationId = (string)Properties.Settings.Default["ApiIterationId"];
                MainWindow.predictionKeyValue = (string)Properties.Settings.Default["ApiPredictionKey"];
            }
            else
            {
                MessageBox.Show("信頼確率の値が不正です", "** Error **", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            this.Close();
        }
    }
}
