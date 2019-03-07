using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncExample
{
    public static class DemoModel
    {
        static int _doneSiteCont = 0;
        static IProgress<ProgressModel> _progress;
        static int _totalSiteCount = PrepData().Count;

        public static List<string> PrepData()
        {
            List<string> output = new List<string>();

            output.Add("https://www.yahoo.com");
            output.Add("https://www.google.com");
            output.Add("https://www.microsoft.com");
            output.Add("https://www.cnn.com");
            output.Add("https://www.codeproject.com");
            output.Add("https://www.stackoverflow.com");

            return output;
        }

        public static async Task RunDownloadParallel(IProgress<ProgressModel> progress)
        {
            _progress = progress;

            _doneSiteCont = 0;
            progress = new Progress<ProgressModel>();

            List<string> websites = PrepData();
            List<Task<WebsiteDataModel>> tasks = new List<Task<WebsiteDataModel>>();

            foreach (string site in websites)
            {
                Task task = Task.Factory.StartNew(() => DownloadWebsite(site, progress));
                
                tasks.Add(DownloadWebsiteAsync(site));
            }

            Task t = Task.WhenAll(tasks.ToArray());

            await t;

            // This is a workaround to help the main thread
            // It seems the progress thread sometimes lags behin which causes the main thread to 
            // update before we are done reporting on all tasks
            while (_doneSiteCont != _totalSiteCount)
                Thread.Sleep(5);

        }

        public static void DownloadWebsite(string websiteURL, IProgress<ProgressModel> progress)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            output.WebsiteData = client.DownloadString(websiteURL);

            watch.Stop();
            output.time_ms = watch.ElapsedMilliseconds;

            _doneSiteCont++;
            ProgressModel progressModel = new ProgressModel();
            progressModel.Progess = _doneSiteCont * 100 / _totalSiteCount;
            progressModel.WebSiteStatus = $"{ websiteURL } downloaded: { output.WebsiteData.Length } characters long in {output.time_ms} ms .{ Environment.NewLine }";
            _progress.Report(progressModel);


        }


        private static void TaskDone(Task<WebsiteDataModel> arg)
        {
            _doneSiteCont++;

            if (_progress != null)
            {
                ProgressModel progressModel = new ProgressModel();
                progressModel.Progess = _doneSiteCont * 100 / _totalSiteCount;

                WebsiteDataModel data = arg.Result;

                progressModel.WebSiteStatus = $"{  data.WebsiteUrl } downloaded: { data.WebsiteData.Length } characters long in {data.time_ms} ms .{ Environment.NewLine }";

                _progress.Report(progressModel);
            }
        }

        public static void RunDownloadSync(IProgress<ProgressModel> progress)
        {
            List<string> websites = PrepData();

            foreach (string site in websites)
            {
                WebsiteDataModel results = DownloadWebsite(site);
            }
        }

        public static WebsiteDataModel DownloadWebsite(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();


            var watch = System.Diagnostics.Stopwatch.StartNew();

            output.WebsiteUrl = websiteURL;
            output.WebsiteData = client.DownloadString(output.WebsiteUrl);

            watch.Stop();
            output.time_ms = watch.ElapsedMilliseconds;

            return output;
        }

        public static async Task DownloadWebsiteAddToListAsync(string websiteURL)
        {
            WebsiteDataModel websiteDataModel = await DownloadWebsiteAsync(websiteURL);

        }

        public static async Task<WebsiteDataModel> DownloadWebsiteAsync(string websiteURL)
        {
            WebsiteDataModel output = new WebsiteDataModel();
            WebClient client = new WebClient();

            output.WebsiteUrl = websiteURL;

            var watch = System.Diagnostics.Stopwatch.StartNew();
            output.WebsiteData = await client.DownloadStringTaskAsync(websiteURL);

            watch.Stop();
            output.time_ms = watch.ElapsedMilliseconds;

            return output;
        }

    }
}
