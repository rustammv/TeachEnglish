//-----------------------------------------------------------------------
// <copyright file="LoaderAudio.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.BL
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Ruma.ExtensionEventArgs;
    using HtmlAgilityPack;

    public interface ILoaderAudio
    {
        event EventHandler LoadingStart;

        event EventHandler<LoadEventArgs> LoadingProcessChanged;

        event EventHandler<byte[]> LoadingFileComplite;

        event EventHandler<int> LoadingComplite;

        event EventHandler<MessageEventArgs> SendMessage;

        List<bool> ListAudioCheck { get; }

        List<byte[]> ListLoadedAudio { get; }

        bool IsLoading { get; }

        void StartLoad(string searchWord, int maximumAudioFiles);

        void CheckAudio(int id, bool state);

        void UnCheckedAllAudio();

        List<byte[]> GetListAudioChecked();
    }

    public class LoaderAudio : ILoaderAudio
    {
        #region consts

        private const string UrlTargetPage = "http://ru.forvo.com//word/{0}/#en";

        private const string UrlAutorizationPage = "http://ru.forvo.com/login/";

        private const string LoginString = "rustammv";

        private const string PasswordString = "pindos2001";

        private const string HostTargetPage = "http://ru.forvo.com";

        #endregion

        #region fields

        private Task task;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private string keyWord = string.Empty;

        private int maximumAudioFiles = 1;

        #endregion

        public LoaderAudio()
        {
            this.ListAudioCheck = new List<bool>();
            this.ListLoadedAudio = new List<byte[]>();
            this.IsLoading = false;
        }

        #region Event

        public event EventHandler LoadingStart;

        public event EventHandler<LoadEventArgs> LoadingProcessChanged;

        public event EventHandler<byte[]> LoadingFileComplite;

        public event EventHandler<int> LoadingComplite;

        public event EventHandler<MessageEventArgs> SendMessage;

        #endregion

        #region properties

        public bool IsLoading { get; private set; }

        public List<bool> ListAudioCheck
        {
            get;
            private set;
        }

        public List<byte[]> ListLoadedAudio
        {
            get;
            private set;
        }

        #endregion

        public void CheckAudio(int id, bool state)
        {
            this.ListAudioCheck[id] = state;
        }

        public void UnCheckedAllAudio()
        {
            for (int i = 0; i < this.ListAudioCheck.Count; i++)
            {
                this.ListAudioCheck[i] = false;
            }
        }

        public List<byte[]> GetListAudioChecked()
        {
            List<byte[]> result = new List<byte[]>();
            for (int i = 0; i < this.ListAudioCheck.Count; i++)
            {
                if (this.ListAudioCheck[i])
                {
                    result.Add(this.ListLoadedAudio[i]);
                }
            }

            return result;
        }

        public void StartLoad(string searchWord, int maximumAudioFiles)
        {
            this.keyWord = searchWord;
            this.maximumAudioFiles = maximumAudioFiles;
            try
            {
                if (this.task != null && this.task.Status == TaskStatus.Running)
                {
                    this.tokenSource.Cancel();
                }
            }
            catch (AggregateException e)
            {
                this.OnSendMessage(new MessageEventArgs(string.Format("Error :/n{0}", e.Message), TypeMessage.Error));
            }

            this.tokenSource = new CancellationTokenSource();
            this.OnLoadingStart();
            this.task = Task.Factory.StartNew(this.LoadAudio, this.tokenSource.Token, this.tokenSource.Token);
        }

        #region full cycle of loading audio files

        protected virtual bool Autorization(string url, string login, string password, out string cookies)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.KeepAlive = true;
                request.AllowAutoRedirect = false;
                string formParams = string.Format("login={0}&password={1}", login, password);
                byte[] someBytes = Encoding.UTF8.GetBytes(formParams);
                request.ContentLength = someBytes.Length;
                Stream newStream = request.GetRequestStream();
                newStream.Write(someBytes, 0, someBytes.Length);
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                cookies = string.IsNullOrEmpty(response.Headers["Set-Cookie"])
                    ? string.Empty
                    : response.Headers["Set-Cookie"];
                response.Close();
                return true;
            }
            catch (WebException e)
            {
                this.OnSendMessage(new MessageEventArgs(
                    string.Format("Error during execution authorization:/n{0}", e.Message),
                    TypeMessage.Error));
                cookies = string.Empty;
                return false;
            }
        }

        protected virtual string GettingTargetPage(string searchWord, string cookies)
        {
            try
            {
                string url = string.Format(UrlTargetPage, searchWord);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/4.0+(compatible;+MSIE+5.01;+Windows+NT+5.0)";
                request.Method = "GET";
                if (!string.IsNullOrEmpty(cookies))
                {
                    request.Headers.Add(HttpRequestHeader.Cookie, cookies);
                }

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string html = string.Empty;
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    using (StreamReader myStreamReader = new StreamReader(stream))
                    {
                        html = myStreamReader.ReadToEnd();
                    }
                }

                response.Close();
                return html;
            }
            catch (WebException e)
            {
                this.OnSendMessage(new MessageEventArgs(
                    string.Format("Error getting the target page:/n{0}", e.Message),
                    TypeMessage.Error));
                return string.Empty;
            }
        }

        protected virtual byte[] DownloadAudioFile(string url, string filePath, string cookies, int count = 1, int currentFile = 1)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                if (!string.IsNullOrEmpty(cookies))
                {
                    request.Headers.Add(HttpRequestHeader.Cookie, cookies);
                }

                byte[] resultByteArray = null;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    long totalBytesToReceive = response.ContentLength;
                    using (Stream inputStream = response.GetResponseStream())
                    {
                        if (inputStream != null)
                        {
                            using (MemoryStream outputStream = new MemoryStream())
                            {
                                byte[] buffer = new byte[4024];
                                int bytesRead;
                                long bytesReceived = 0;
                                do
                                {
                                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                                    outputStream.Write(buffer, 0, bytesRead);
                                    bytesReceived += bytesRead;
                                    this.OnLoadingProcessChanged(new LoadEventArgs(
                                        "Downloading audio file...",
                                        currentFile,
                                        count,
                                        filePath,
                                        bytesReceived,
                                        totalBytesToReceive));
                                }
                                while (bytesRead > 0);
                                resultByteArray = outputStream.ToArray();
                            }
                        }
                    }
                }

                return resultByteArray;
            }
            catch (WebException e)
            {
                this.OnSendMessage(new MessageEventArgs(
                        string.Format("Failed to load file {0}:/n{1}", url, e.Message),
                        TypeMessage.Error));
                return null;
            }
        }

        protected virtual List<string> GettingListLinkToAudioFile(string html)
        {
            try
            {
                List<string> resultList = new List<string>();
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                foreach (HtmlNode node in doc.DocumentNode.SelectNodes("//div[@class='word']//div[@class='download']//a"))
                {
                    if (node.Attributes["href"] != null)
                    {
                        if (Uri.IsWellFormedUriString(HostTargetPage + node.Attributes["href"].Value, UriKind.Absolute))
                        {
                            resultList.Add(HostTargetPage + node.Attributes["href"].Value);
                        }
                    }
                }

                return resultList;
            }
            catch (Exception e)
            {
                this.OnSendMessage(new MessageEventArgs(string.Format("Error parser :/n{0}", e.Message), TypeMessage.Error));
                return new List<string>();
            }
        }

        protected virtual void CleatLists()
        {
            this.ListAudioCheck.Clear();
            this.ListLoadedAudio.Clear();
        }

        protected virtual void LoadAudio(object ct)
        {
            CancellationToken cancelTok = (CancellationToken)ct;
            this.CleatLists();
            if (!cancelTok.IsCancellationRequested)
            {
                this.OnLoadingProcessChanged(new LoadEventArgs("Autorization..."));
            }
            else
            {
                cancelTok.ThrowIfCancellationRequested();
            }

            string sCookies;
            bool autorizationSuccessfully = this.Autorization(UrlAutorizationPage, LoginString, PasswordString, out sCookies);
            if (!autorizationSuccessfully)
            {
                return;
            }

            if (!cancelTok.IsCancellationRequested)
            {
                this.OnLoadingProcessChanged(new LoadEventArgs("Download target page..."));
            }
            else
            {
                cancelTok.ThrowIfCancellationRequested();
            }

            string html = this.GettingTargetPage(this.keyWord, sCookies);
            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            if (!cancelTok.IsCancellationRequested)
            {
                this.OnLoadingProcessChanged(new LoadEventArgs("Getting links from the target page..."));
            }
            else
            {
                cancelTok.ThrowIfCancellationRequested();
            }

            List<string> list = this.GettingListLinkToAudioFile(html);

            int loadedFilesCount = 0;

            for (int i = 0; i < list.Count; i++)
            {
                if (loadedFilesCount >= this.maximumAudioFiles)
                {
                    break;
                }

                string filePath = string.Format("{0}.mp3", i);
                if (!cancelTok.IsCancellationRequested)
                {
                    byte[] downloadAudio = this.DownloadAudioFile(list[i], filePath, sCookies, list.Count, i);
                    if (downloadAudio != null)
                    {
                        this.OnLoadingAudioFileComplite(downloadAudio);
                        this.ListLoadedAudio.Add(downloadAudio);
                        this.ListAudioCheck.Add(false);
                        loadedFilesCount++;
                    }
                }
                else
                {
                    cancelTok.ThrowIfCancellationRequested();
                }
            }

            if (!cancelTok.IsCancellationRequested)
            {
                this.OnLoadingComplite(loadedFilesCount);
            }
        }

        #endregion

        #region invocator event

        protected virtual void OnLoadingProcessChanged(LoadEventArgs e)
        {
            var handler = this.LoadingProcessChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnLoadingStart()
        {
            this.IsLoading = true;
            var handler = this.LoadingStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnLoadingComplite(int e)
        {
            var handler = this.LoadingComplite;
            if (handler != null)
            {
                handler(this, e);
            }

            this.IsLoading = false;
        }

        protected virtual void OnLoadingAudioFileComplite(byte[] source)
        {
            var handler = this.LoadingFileComplite;
            if (handler != null)
            {
                handler(this, source);
            }
        }

        protected virtual void OnSendMessage(MessageEventArgs e)
        {
            var handler = this.SendMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion
    }
}
