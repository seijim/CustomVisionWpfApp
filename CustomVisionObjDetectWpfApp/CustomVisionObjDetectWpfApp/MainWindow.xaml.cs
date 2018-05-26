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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace CustomVisionObjDetectWpfApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly string predictionKeyName = "Prediction-Key";
        static string predictionKeyValue;
        static string predictionEndpoint;
        static string iterationId;
        static string predictionUri;

        SettingWindow settingWindow;
        double resizeFactor;
        JArray visionPredictions;
        string[] visionDescriptions;

        class VisionRectangle
        {
            public double Left { set; get; }
            public double Top { set; get; }
            public double Width { set; get; }
            public double Height { set; get; }
        }
        VisionRectangle[] visionRectangles;

        static readonly Brush[] brushArray = {
            Brushes.Yellow,
            Brushes.Pink,
            Brushes.Orange,
            Brushes.LightGreen,
            Brushes.LightCyan,
            Brushes.LightGoldenrodYellow,
            Brushes.LightPink,
            Brushes.LightSeaGreen,
            Brushes.LightCoral,
            Brushes.LightBlue
        };
        int indexBrushes;
        Dictionary<string, Brush> dicTagBrush;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)Properties.Settings.Default["ApiEndpoint"] == string.Empty)
            {
                settingWindow = new SettingWindow();
                settingWindow.ShowDialog();
            }
            predictionEndpoint = (string)Properties.Settings.Default["ApiEndpoint"];
            iterationId = (string)Properties.Settings.Default["ApiIterationId"];
            predictionUri = predictionEndpoint + $"?iterationId={iterationId}";
            predictionKeyValue = (string)Properties.Settings.Default["ApiPredictionKey"];
            TextBoxProbability.Text = (string)Properties.Settings.Default["Probability"];

            indexBrushes = 0;
            dicTagBrush = new Dictionary<string, Brush>();

            // Get the image file to scan from the user.
            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Filter = "JPEG Image(*.jpg,*.png,*.gif)|*.jpg;*.png;*.gif";
            bool? result = openDlg.ShowDialog(this);

            // Return if canceled.
            if (!(bool)result)
            {
                return;
            }

            // Display the image file.
            string filePath = openDlg.FileName;

            Uri fileUri = new Uri(filePath);
            BitmapImage bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            VisionPhoto.Source = bitmapSource;

            // Detect any faces in the image.
            Title = "オブジェクトの検出中.....";
            visionPredictions = await UploadAndDetectObjects(filePath);
            if (visionPredictions == null)
                return;

            Title = String.Format("** オブジェクトが検出されました ** {0} object(s) detected", visionPredictions.Count);

            if (visionPredictions.Count > 0)
            {
                // Prepare to draw rectangles around the objects.
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bitmapSource,
                    new Rect(0, 0, bitmapSource.Width, bitmapSource.Height));
                double dpi = bitmapSource.DpiX;
                resizeFactor = 96 / dpi;

                visionRectangles = new VisionRectangle[visionPredictions.Count];
                visionDescriptions = new String[visionPredictions.Count];

                for (int i = 0; i < visionPredictions.Count; ++i)
                {
                    var prediction = (JObject)visionPredictions[i];

                    string tagName = (string)prediction["tagName"];
                    double probability = (double)prediction["probability"];

                    var boundingBox = (JObject)prediction["boundingBox"];
                    var rectangle = new VisionRectangle();
                    rectangle.Left = (double)boundingBox["left"] * bitmapSource.PixelWidth;
                    rectangle.Top = (double)boundingBox["top"] * bitmapSource.PixelHeight;
                    rectangle.Width = (double)boundingBox["width"] * bitmapSource.PixelWidth;
                    rectangle.Height = (double)boundingBox["height"] * bitmapSource.PixelHeight;
                    // Store the object rectangle.
                    visionRectangles[i] = rectangle;
                    // Store the object description.
                    visionDescriptions[i] = "Tag : " + tagName + " / Probability : " + probability.ToString();

                    // Draw a rectangle on the object.
                    double threshold = 0;
                    if (!double.TryParse(TextBoxProbability.Text, out threshold))
                    {
                        threshold = 0.5;
                        TextBoxProbability.Text = threshold.ToString();
                    }

                    if (probability >= threshold)
                    {
                        try
                        {
                            Brush brush = dicTagBrush[tagName];
                        }
                        catch
                        {
                            // Set diffrent color to each tag's rectangle and text.
                            dicTagBrush[tagName] = brushArray[indexBrushes];
                            ++indexBrushes;
                            if (indexBrushes == brushArray.Length)
                                indexBrushes = 0;
                        }
                        drawingContext.DrawRectangle(
                            Brushes.Transparent,
                            new Pen(dicTagBrush[tagName], 2),
                            new Rect(
                                rectangle.Left * resizeFactor,
                                rectangle.Top * resizeFactor,
                                rectangle.Width * resizeFactor,
                                rectangle.Height * resizeFactor
                            )
                        );
                        drawingContext.DrawText(
                            new FormattedText(
                                tagName + $" ({(probability * 100).ToString().Substring(0, 5)}%)",
                                CultureInfo.CurrentCulture,
                                FlowDirection.LeftToRight, 
                                new Typeface("Meiryo UI"),
                                11,
                                dicTagBrush[tagName]
                            ),
                            new Point(
                                rectangle.Left * resizeFactor + 4,
                                rectangle.Top * resizeFactor + 2
                            )
                        );
                    }
                    else
                    {
                        // Store the object rectangle.
                        visionRectangles[i] = null;
                        // Store the object description.
                        visionDescriptions[i] = string.Empty;
                    }
                }

                drawingContext.Close();

                // Display the image with the rectangle around the face.
                RenderTargetBitmap visionWithRectBitmap = new RenderTargetBitmap(
                    (int)(bitmapSource.PixelWidth * resizeFactor),
                    (int)(bitmapSource.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                visionWithRectBitmap.Render(visual);
                VisionPhoto.Source = visionWithRectBitmap;

                // Set the status bar text.
                VisionDescriptionStatusBar.Text = " ** マウスポインターをオブジェクト上にポイントすると、詳細が表示されます";
            }
        }

        private async Task<JArray> UploadAndDetectObjects(string filePath)
        {
            TimeSpan ts;
            string stringTs = string.Empty;

            // Set http client
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(predictionKeyName, predictionKeyValue);

            try
            {
                // Prepare target file data
                byte[] buffer = File.ReadAllBytes(filePath);
                var content = new ByteArrayContent(buffer);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                DateTime dt1 = DateTime.Now;

                // Predict by using object detection model.
                var visionPredictResult = await client.PostAsync(predictionUri, content);

                DateTime dt2 = DateTime.Now;
                ts = dt2 - dt1;
                stringTs = $"** TAT => {ts.Hours}:{ts.Minutes}:{ts.Seconds}.{ts.Milliseconds}";
                Console.WriteLine(stringTs);

                if (visionPredictResult.IsSuccessStatusCode)
                {
                    var responseBody = visionPredictResult.Content.ReadAsStringAsync();
                    var resultBody = JObject.Parse(responseBody.Result.ToString());
                    var predictions = (JArray)resultBody["predictions"];

                    return predictions;
                }
                else
                {
                    string errorMessage = "** Web Call Error **  Status Code=" + visionPredictResult.StatusCode 
                                        + " Reason=" + visionPredictResult.ReasonPhrase;
                    MessageBox.Show(errorMessage);
                }
            }
            // catch (ApplicationException ex)
            catch (Exception ex)
            {
                string errorMessage = "** Exception occured ** " + ex.ToString();
                MessageBox.Show(errorMessage);
            }

            return null;
        }

        // Displays the face description when the mouse is over a face rectangle.
        private void VisionPhoto_MouseMove(object sender, MouseEventArgs e)
        {
            // If the REST call has not completed, return from this method.
            if (visionPredictions == null)
                return;

            // Find the mouse position relative to the image.
            Point mouseXY = e.GetPosition(VisionPhoto);

            ImageSource imageSource = VisionPhoto.Source;
            BitmapSource bitmapSource = (BitmapSource)imageSource;

            // Scale adjustment between the actual size and displayed size.
            var scale = VisionPhoto.ActualWidth / (bitmapSource.PixelWidth / resizeFactor);

            // Check if this mouse position is over a face rectangle.
            bool mouseOverFace = false;

            for (int i = 0; i < visionPredictions.Count; ++i)
            {
                VisionRectangle rectangle = visionRectangles[i];
                if (rectangle == null)
                    continue;

                double left = rectangle.Left * scale;
                double top = rectangle.Top * scale;
                double width = rectangle.Width * scale;
                double height = rectangle.Height * scale;

                // Display the face description for this face if the mouse is over this face rectangle.
                if (mouseXY.X >= left && mouseXY.X <= left + width && mouseXY.Y >= top && mouseXY.Y <= top + height)
                {
                    VisionDescriptionStatusBar.Text = visionDescriptions[i];
                    mouseOverFace = true;
                    break;
                }
            }

            // If the mouse is not over a face rectangle.
            if (!mouseOverFace)
                VisionDescriptionStatusBar.Text = " ** マウスポインターをオブジェクト上にポイントすると、詳細が表示されます";

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
        }
    }

}

