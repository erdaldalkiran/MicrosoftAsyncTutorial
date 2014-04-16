using System.Windows;

namespace AsyncExampleWPF
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Documents;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            ResultsTextBox.Clear();
            await SumPageSizesAsync();
            ResultsTextBox.Text += "\r\nControl returned to StartButton_Click";
            StartButton.IsEnabled = true;
        }

        private async Task SumPageSizesAsync()
        {
            var downloadTasks = SetUpURLList().Select(ProcessURLAsync).ToList();
            var lengths = await Task.WhenAll(downloadTasks);

            ResultsTextBox.Text += string.Format("\r\n\r\nTotal bytes returned: {0}\r\n", lengths.Sum());
        }

        private async Task<int> ProcessURLAsync(string url)
        {
            var client = new HttpClient { MaxResponseContentBufferSize = 1000000 };
            var content = await client.GetByteArrayAsync(url);
            DisplayResults(url, content);
            return content.Length;
        }

        private void DisplayResults(string url, byte[] content)
        {
            ResultsTextBox.Text += string.Format("\n{0,-58} {1,8}",
                                                    url.Replace("http://", ""),
                                                    content.Length
                                                );
        }

        private async Task<byte[]> GetUrlContentsAsync(string url)
        {
            var content = new MemoryStream();
            var request = (HttpWebRequest)WebRequest.Create(url);


            using (var response = await request.GetResponseAsync())
            {
                using (var responseStream = response.GetResponseStream())
                {
                    await responseStream.CopyToAsync(content);
                }
            }

            return content.ToArray();
        }

        private List<string> SetUpURLList()
        {
            return new List<string> 
    { 
        "http://msdn.microsoft.com/library/windows/apps/br211380.aspx",
        "http://msdn.microsoft.com",
        "http://msdn.microsoft.com/en-us/library/hh290136.aspx",
        "http://msdn.microsoft.com/en-us/library/ee256749.aspx",
        "http://msdn.microsoft.com/en-us/library/hh290138.aspx",
        "http://msdn.microsoft.com/en-us/library/hh290140.aspx",
        "http://msdn.microsoft.com/en-us/library/dd470362.aspx",
        "http://msdn.microsoft.com/en-us/library/aa578028.aspx",
        "http://msdn.microsoft.com/en-us/library/ms404677.aspx",
        "http://msdn.microsoft.com/en-us/library/ff730837.aspx"
    };
        }
    }
}
