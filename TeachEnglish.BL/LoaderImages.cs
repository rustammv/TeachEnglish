//-----------------------------------------------------------------------
// <copyright file="LoaderImages.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.BL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;
    using Ruma.ExtensionEventArgs;
    using Google.API.Search;

    /// <summary>
    /// 
    /// </summary>
    public interface ILoaderImages
    {
        #region events

        event EventHandler LoadingStart;

        event EventHandler<LoadEventArgs> LoadingProcessChanged;

        event EventHandler<BitmapImage> LoadingFileComplite;

        event EventHandler<int> LoadingComplite;

        event EventHandler<MessageEventArgs> SendMessage;

        #endregion

        #region property

        /// <summary>
        /// Gets list check image
        /// </summary>
        List<bool> GetListImageCheck { get; }

        /// <summary>
        /// Gets list loaded image
        /// </summary>
        List<byte[]> GetListLoadedImage { get; }

        int SearchResultCount { get; set; }

        #endregion

        void StartLoad(string searchWord, int maximumImageFiles);

        List<byte[]> GetListImageChecked();

        void UnCheckedAllImage();

        void CheckImage(int id, bool state);
    }

    public class LoaderImages : ILoaderImages
    {
        #region field

        private List<string> listUrl = new List<string>();

        private Task task;

        private string wordKey = string.Empty;

        private int maximumImageFiles;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private int searchResultCount;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderImages"/> class
        /// default constructor
        /// </summary>
        public LoaderImages()
        {
            this.GetListLoadedImage = new List<byte[]>();
            this.GetListImageCheck = new List<bool>();
            this.IsLoading = false;
        }

        #region event

        public event EventHandler LoadingStart;

        public event EventHandler<LoadEventArgs> LoadingProcessChanged;

        public event EventHandler<BitmapImage> LoadingFileComplite;

        public event EventHandler<int> LoadingComplite;

        public event EventHandler<MessageEventArgs> SendMessage;

        #endregion

        #region properties

        public List<byte[]> GetListLoadedImage { get; private set; }

        /// <summary>
        /// return list checked image
        /// </summary>
        public List<bool> GetListImageCheck { get; private set; }

        public bool IsLoading { get; private set; }

        public int SearchResultCount
        {
            get
            {
                return this.searchResultCount;
            }

            set
            {
                this.searchResultCount = value;
                if (value > 100)
                {
                    this.searchResultCount = 100;
                }

                if (value < 1)
                {
                    this.searchResultCount = 1;
                }
            }
        }

        #endregion

        public void UnCheckedAllImage()
        {
            for (int i = 0; i < this.GetListImageCheck.Count; i++)
            {
                this.GetListImageCheck[i] = false;
            }
        }

        public byte[] ReSizeImageToByte(Stream stream, int maxSize = 300)
        {
            stream.Position = 0;
            Bitmap bb = new Bitmap(stream);
            float scale;
            Size size;
            byte[] result;
            if (bb.Height >= bb.Width)
            {
                scale = bb.Height / (float)maxSize;
                size = new Size((int)(bb.Width / scale), maxSize);
            }
            else
            {
                scale = bb.Width / (float)maxSize;
                size = new Size(maxSize, (int)(bb.Height / scale));
            }

            MemoryStream str = new MemoryStream();
            using (Bitmap newBitmap = new Bitmap(bb, size))
            {
                newBitmap.Save(str, System.Drawing.Imaging.ImageFormat.Jpeg);
                result = str.ToArray();
            }

            str.Dispose();
            bb.Dispose();
            return result;
        }

        /// <summary>
        /// method getting a list of url to image with google search 
        /// use Google.Search.Api
        /// </summary>
        /// <param name="searchKeyWord">the key word for image search</param>
        /// <returns></returns>
        public List<string> GetListUrlImages(string searchKeyWord)
        {
            this.OnLoadingProcessChanged(new LoadEventArgs("Getting link ..."));
            //// the specified search site
            GimageSearchClient client = new GimageSearchClient("http://www.google.com");
            //// configuring search
            IList<IImageResult> result = client.Search(searchKeyWord, 100, "large", "all", "all", ImageFileType.Jpg.ToString());
            this.ClearLists();
            this.listUrl = result.Select(x => x.Url).ToList();
            return this.listUrl;
        }

        public List<byte[]> GetListImageChecked()
        {
            List<byte[]> result = new List<byte[]>();
            for (int i = 0; i < this.GetListImageCheck.Count; i++)
            {
                if (this.GetListImageCheck[i])
                {
                    result.Add(this.GetListLoadedImage[i]);
                }
            }

            return result;
        }

        public void CheckImage(int id, bool state)
        {
            this.GetListImageCheck[id] = state;
        }

        public void StartLoad(string searchKeyWord, int maxImageFiles)
        {
            this.wordKey = searchKeyWord;
            this.maximumImageFiles = maxImageFiles;
            try
            {
                if (this.task != null && this.task.Status == TaskStatus.Running)
                {
                    this.tokenSource.Cancel();
                }
            }
            catch (AggregateException)
            {
            }
            this.tokenSource = new CancellationTokenSource();
            this.OnLoadingStart();
            this.task = Task.Factory.StartNew(this.LoadImages, this.tokenSource.Token);
        }

        /// <summary>
        /// loading the image file from Url to BitmapImage
        /// </summary>
        /// <param name="url"></param>
        /// <param name="current"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        protected virtual BitmapImage DownloadingImageFile(string url, int current, int count)
        {
            BitmapImage src = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 5000;
                request.AllowAutoRedirect = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream outputStream = null;
                    using (Stream inputStream = response.GetResponseStream())
                    {
                        long totalBytesToReceive = response.ContentLength;
                        if (inputStream != null)
                        {
                            outputStream = new MemoryStream();
                            byte[] buffer = new byte[4096];
                            long bytesReceived = 0;
                            int bytesRead;
                            do
                            {
                                bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                                outputStream.Write(buffer, 0, bytesRead);
                                bytesReceived += bytesRead;
                                this.OnLoadingProcessChanged(
                                    new LoadEventArgs(
                                        "Downloading file ...", 
                                        current + 1, 
                                        this.maximumImageFiles, 
                                        url, 
                                        bytesReceived, 
                                        totalBytesToReceive));
                            } 
                            while (bytesRead != 0);
                        }
                    }

                    src = this.CreateBitmapImageFromStream(outputStream);
                }
            }
            catch (Exception e)
            {
                this.OnSendMessage(new MessageEventArgs(string.Format("Error downloading file :/n{0}", e.Message), TypeMessage.Error));
                return null;
            }

            return src;
        }

        protected virtual BitmapImage CreateBitmapImageFromStream(Stream stream)
        {
            BitmapImage tempImage;
            try
            {
                if (stream != null)
                {
                    tempImage = new BitmapImage();
                    tempImage.BeginInit();
                    //// to solve the error The image cannot be decoded. The header image might be corrupted   ----
                    tempImage.CacheOption = BitmapCacheOption.OnLoad;
                    stream.Position = 0; // <------ 
                    tempImage.StreamSource = stream;
                    tempImage.EndInit();
                    tempImage.Freeze();
                    return tempImage;
                }
            }
            catch (Exception)
            {
                tempImage = null;
            }

            return null;
        }

        /// <summary>
        /// method loading all files from ListUrl
        /// </summary>
        /// <param name="ct"></param>
        protected virtual void LoadImages(object ct)
        {
            CancellationToken cancelTok = (CancellationToken)ct;
            this.GetListUrlImages(this.wordKey);
            int loadedFilesCount = 0;
            for (int i = 0; i < this.listUrl.Count; i++)
            {
                if (loadedFilesCount >= this.maximumImageFiles)
                {
                    break;
                }

                if (!cancelTok.IsCancellationRequested)
                {
                    BitmapImage myBitmapImage = this.DownloadingImageFile(this.listUrl[i], i, this.listUrl.Count);
                    if (myBitmapImage != null)
                    {
                        if (!cancelTok.IsCancellationRequested)
                        {
                            this.GetListLoadedImage.Add(this.ReSizeImageToByte(myBitmapImage.StreamSource));
                            this.GetListImageCheck.Add(false);
                            this.OnLoadingFileComplite(myBitmapImage);
                            loadedFilesCount++;
                        }
                    }
                }
                else
                {
                    break;
                }
            }

            if (!cancelTok.IsCancellationRequested)
            {
                this.OnLoadingComplite(loadedFilesCount);
            }
        }

        protected virtual void ClearLists()
        {
            this.listUrl.Clear();
            this.GetListLoadedImage.Clear();
            this.GetListImageCheck.Clear();
        }

        #region invocator event

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

        protected virtual void OnLoadingFileComplite(BitmapImage bm)
        {
            var handler = this.LoadingFileComplite;
            if (handler != null)
            {
                handler(this, bm);
            }
        }

        protected virtual void OnLoadingProcessChanged(LoadEventArgs e)
        {
            var handler = this.LoadingProcessChanged;
            if (handler != null)
            {
                handler(this, e);
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
