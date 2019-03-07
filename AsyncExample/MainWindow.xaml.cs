using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace AsyncExample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void executeSync_Click(object sender, RoutedEventArgs e)
        {
            resultsWindow.Text = "";
            stopwatch.Restart();

            Progress<ProgressModel> progress = new Progress<ProgressModel>();
            progress.ProgressChanged += Progress_ProgressChanged;
            DemoModel.RunDownloadSync(progress);
        }

        private async void executeAsync_Click(object sender, RoutedEventArgs e)
        {
            executeAsync.IsEnabled = false;
            resultsWindow.Text = "";
            stopwatch.Restart();


            Progress<ProgressModel> progress = new Progress<ProgressModel>();
            progress.ProgressChanged += Progress_ProgressChanged;
            await DemoModel.RunDownloadParallel(progress);


            executeAsync.IsEnabled = true;

        }

        private void Progress_ProgressChanged(object sender, ProgressModel e)
        {
            progrBar.Value = e.Progess;
            resultsWindow.Text += e.WebSiteStatus;

            if (progrBar.Value == 100)
            {
                stopwatch.Stop();
                var elapsedMs = stopwatch.ElapsedMilliseconds;
                resultsWindow.Text += $"Total execution time: { elapsedMs }";
            }

        }

        private void ReportWebsiteInfo(WebsiteDataModel data)
        {
            resultsWindow.Text += $"{ data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long in {data.time_ms} ms .{ Environment.NewLine }";
        }
    }
}
