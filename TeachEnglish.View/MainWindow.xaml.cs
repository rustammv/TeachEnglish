
using Ruma.PlayMp3;

namespace TeachEnglish.View
{
    using System;
    using System.IO;
    using System.Configuration;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Collections.Generic;
    using Ruma.ExtensionEventArgs;
    using Ruma.Controls;

    public interface IView
    {
        event EventHandler<StartSearchAudioEventArgs> StartSearchAudio;

        event EventHandler<StartSearchImageEventArgs> StartSearchImage;

        event EventHandler<StartSearchTranslationEventArgs> StartSearchTranslation;

        event EventHandler<CheckedEventArgs> SelectImage;

        event EventHandler<CheckedEventArgs> SelectAudio;

        event EventHandler UnCheckedAllAudio;

        event EventHandler UnCheckedAllImage;

        event EventHandler<int> GetCheckedData;

        event EventHandler<string> GetListWordFromDataBase;

        event EventHandler ShotdownApplication;

        void CreateListIntellisense(List<string> list);
        void ListAudioAddAudio(byte[] source);

        void ListAudioClear();

        void ListAudioShowProgress();

        void ListAudioHideProgress(int e);

        void ListAudioProgressChanged(string text, int current, int amount, long bytesReceived, long totalBytesOfReceive);

        void ListImageAddImage(BitmapImage bitmap);

        void ListImageClear();

        void ListImageShowProgress();

        void ListImageHideProgress(int e);

        void ListImageProgressChanged(string status, int current, int amount, long bytesReceived, long totalBytesOfReceive);

        void FillTExtLog(string text);

        void ListTransAddTranslation(string translation, string partOfSpeach, string trancription);

        void ListTranslationClear();

        void ListTranslationLoadingComplite(int e);

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class RumaMainWindow : Window, IView
    {
        #region field

        private readonly PlayMp3 playMp3 = new PlayMp3();

        private string wordKey = string.Empty;

        private int maxImageFiles = 1;

        private int maxAudioFiles = 1;

        private string apiKeyDict;

        private int selectedTranslationID = -1;

        #endregion

        public RumaMainWindow()
        {
            this.InitializeComponent();
            this.TextBoxApiKeyDict.Text = ConfigurationManager.AppSettings["ApiKeyDict"];
            this.MiTrans.Click += this.OnStartSearchTranslation;
            this.MiImage.Click += this.OnStartSearchImages;
            this.MiAudio.Click += this.OnStartSearchAudio;
            this.MiAll.Click += this.OnStartSearchAll;
            this.ButtonStartSearchAudio.Click += this.OnStartSearchAudio;
            this.ButtonStartSearchImages.Click += this.OnStartSearchImages;
            this.ButtonStartSearchAll.Click += this.OnStartSearchAll;
            this.ButtonStartSearchTranslation.Click += this.OnStartSearchTranslation;
            this.ButtonGetCheckedData.Click += this.OnGetCheckedData;

            this.MiAbout.Click += this.OnMiAboutClick;
            this.MiExit.Click += this.OnMiExitClick;

            this.RowProgressAudio.Height = new GridLength(0);
            this.RowProgressImage.Height = new GridLength(0);

            this.TextBoxSearchWord.TextChanged += TextBoxSearchWord_TextChanged;

            //tt.Text = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

        void TextBoxSearchWord_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBoxIntellisense tb = sender as TextBoxIntellisense;
            if (tb != null)
            {
                OnGetListWordFromDataBase(tb.Text);
            }
        }

        public void CreateListIntellisense(List<string> list)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.TextBoxSearchWord.ListData = list;
            }));
        }

        #region event

        public event EventHandler<StartSearchAudioEventArgs> StartSearchAudio;

        public event EventHandler<StartSearchImageEventArgs> StartSearchImage;

        public event EventHandler<StartSearchTranslationEventArgs> StartSearchTranslation;

        public event EventHandler<CheckedEventArgs> SelectImage;

        public event EventHandler<CheckedEventArgs> SelectAudio;

        public event EventHandler UnCheckedAllAudio;

        public event EventHandler UnCheckedAllImage;

        public event EventHandler<int> GetCheckedData;

        public event EventHandler<string> GetListWordFromDataBase;

        public event EventHandler ShotdownApplication;

        #endregion

        #region audio

        public void ListAudioAddAudio(byte[] source)
        {
            this.DispatcherEx((Action)(() =>
            {
                int id = this.ListAudio.SpListButton.Children.Count;
                ButtonTextAndCheck btn = new ButtonTextAndCheck(id, string.Format("Play {0} var. of the word \"{1}\"", id + 1, this.wordKey));
                btn.Margin = new Thickness(3);
                btn.Height = 25;
                btn.ButtonClick += (btnsender, btnargs) =>
                {
                    this.playMp3.PlayStart(source);
                };

                btn.CheckBoxCheckStateChanged += (sender, args) =>
                {
                    this.OnSelectAudio(new CheckedEventArgs(id, args.IsChecked));
                };

                this.ListAudio.SpListButton.Children.Add(btn);
            }));
        }

        public void ListAudioClear()
        {
            this.DispatcherEx((Action)(() =>
            {
                if (this.ListAudio.SpListButton.Children.Count > 0)
                {
                    this.ListAudio.SpListButton.Children.Clear();
                }

                this.playMp3.PlayStop();
            }));
        }

        public void ListAudioShowProgress()
        {
            this.DispatcherEx((Action)(() =>
            {
                this.RowProgressAudio.Height = new GridLength(69);
                this.LoadingProcessAudio.ResetProgressBar();
            }));
        }

        public void ListAudioHideProgress(int e)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.RowProgressAudio.Height = new GridLength(0);
                this.LoadingProcessAudio.ResetProgressBar();
                this.LblAudio.Content = "In the list of " + e + " audio";
            }));
        }

        public void ListAudioProgressChanged(string text, int count, int current, long bytesReceived, long totalBytesOfReceive)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.LoadingProcessAudio.LabelStatusLoading.Content = text;
            }));

            if (count > 0)
            {
                this.DispatcherEx((Action)(() =>
                {
                    this.LoadingProcessAudio.ProgressBarCountFile.Maximum = count;
                    this.LoadingProcessAudio.ProgressBarCountFile.Value = current;
                    this.LoadingProcessAudio.LabelProgressBarCountFile.Content = string.Format(
                        "Loaded {0} is {1} files",
                        current + 1,
                        count);
                    this.LoadingProcessAudio.ProgressBarDownloadFile.Maximum = totalBytesOfReceive;
                    this.LoadingProcessAudio.ProgressBarDownloadFile.Value = bytesReceived;
                    this.LoadingProcessAudio.LabelProgressBarDownloadFile.Content = string.Format(
                        "Loaded {0} is {1} bytes",
                        bytesReceived,
                        totalBytesOfReceive);
                }));
            }
        }

        #endregion

        #region images

        public void ListImageAddImage(BitmapImage bitmap)
        {
            this.DispatcherEx((Action)(() =>
            {
                int id = ListImage.SpListButton.Children.Count;

                Image temp = new Image();
                temp.Height = 200;
                temp.Source = bitmap;
                ButtonImageAndCheck btn = new ButtonImageAndCheck(id, bitmap);
                btn.Margin = new Thickness(3);
                btn.CheckBoxCheckStateChanged += (sender, args) =>
                {
                    this.OnSelectImage(new CheckedEventArgs(id, args.IsChecked));
                };

                this.ListImage.SpListButton.Children.Add(btn);
            }));
        }

        public void ListImageClear()
        {
            this.DispatcherEx((Action)(() =>
            {
                if (this.ListImage.SpListButton.Children.Count > 0)
                {
                    this.ListImage.SpListButton.Children.Clear();
                }
            }));
        }

        public void ListImageShowProgress()
        {
            this.DispatcherEx((Action)(() =>
            {
                this.RowProgressImage.Height = new GridLength(69);
                this.LoadingProcessImage.ResetProgressBar();
            }));
        }

        public void ListImageHideProgress(int e)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.RowProgressImage.Height = new GridLength(0);
                this.LoadingProcessImage.ResetProgressBar();
                this.LblImage.Content = "In the list of " + e + " images";
            }));
        }

        public void ListImageProgressChanged(string status, int current, int amount, long bytesReceived, long totalBytesOfReceive)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.LoadingProcessImage.LabelStatusLoading.Content = status;
            }));

            if (amount > 0)
            {
                this.DispatcherEx((Action)(() =>
                {
                    this.LoadingProcessImage.ProgressBarCountFile.Maximum = amount;
                    this.LoadingProcessImage.ProgressBarCountFile.Value = current;
                    this.LoadingProcessImage.LabelProgressBarCountFile.Content = string.Format("Loaded {0} of {1} files", current, amount);
                    this.LoadingProcessImage.ProgressBarDownloadFile.Maximum = totalBytesOfReceive;
                    this.LoadingProcessImage.ProgressBarDownloadFile.Value = bytesReceived;
                    this.LoadingProcessImage.LabelProgressBarDownloadFile.Content = string.Format("Loaded {0} of {1} bytes", bytesReceived, totalBytesOfReceive);
                }));
            }
        }

        public void FillTExtLog(string text)
        {
            this.TextLog.Clear();
            this.TextLog.Text = text;
        }

        #endregion

        #region Translation

        public void ListTransAddTranslation(string translation, string partOfSpeach, string trancription)
        {
            this.DispatcherEx((Action)(() =>
            {
                int id = this.ListTranslation.SpListButton.Children.Count;
                Radio3LinesText btn = new Radio3LinesText(id, translation, partOfSpeach, trancription);
                btn.Margin = new Thickness(3);
                if (id == 0)
                {
                    btn.IsChecked = true;
                    this.selectedTranslationID = id;
                }

                btn.Checked += (sender, args) =>
                {
                    Radio3LinesText temp = sender as Radio3LinesText;
                    if (temp != null)
                    {
                        if (this.selectedTranslationID != temp.ID)
                        {
                            OnUnCheckedAllImage();
                            this.selectedTranslationID = temp.ID;
                            foreach (ButtonImageAndCheck child in ListImage.SpListButton.Children)
                            {
                                child.IsCheckBoxChecked = false;
                            }
                        }
                    }
                };

                this.ListTranslation.SpListButton.Children.Add(btn);
            }));
        }

        public void ListTranslationClear()
        {
            this.DispatcherEx((Action)(() =>
            {
                if (this.ListTranslation.SpListButton.Children.Count > 0)
                {
                    this.ListTranslation.SpListButton.Children.Clear();
                    this.selectedTranslationID = -1;
                }
            }));
        }

        public void ListTranslationLoadingComplite(int e)
        {
            this.DispatcherEx((Action)(() =>
            {
                this.LblTranslation.Content = "In the list of " + e + " translation";
            }));
        }

        #endregion

        #region invocator event

        private void OnMiExitClick(object sender, RoutedEventArgs e)
        {
            OnShotdownApplication();
            //StartUp.App.Current.Shutdown();
        }

        private void OnMiAboutClick(object sender, RoutedEventArgs e)
        {
            AboutBox aboutBox = new AboutBox(this);
            aboutBox.ShowDialog();
        }

        protected virtual void OnGetCheckedData(object sender, RoutedEventArgs e)
        {
            if (this.selectedTranslationID >= 0)
            {
                var handler = this.GetCheckedData;
                if (handler != null)
                {
                    handler(this, this.selectedTranslationID);
                }
            }
        }

        protected virtual void OnUnCheckedAllAudio()
        {
            var handler = this.UnCheckedAllAudio;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnUnCheckedAllImage()
        {
            var handler = this.UnCheckedAllImage;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnSelectImage(CheckedEventArgs e)
        {
            var handler = this.SelectImage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnSelectAudio(CheckedEventArgs e)
        {
            var handler = this.SelectAudio;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// invocator event StartSearchAudio
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartSearchAudio(object sender, RoutedEventArgs e)
        {
            this.InitSetting();
            if (this.StartSearchAudio != null)
            {
                this.StartSearchAudio(this, new StartSearchAudioEventArgs(this.wordKey, this.maxAudioFiles));
            }
        }

        /// <summary>
        /// invocator event StartSearchImages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartSearchImages(object sender, RoutedEventArgs e)
        {
            this.InitSetting();
            if (this.StartSearchImage != null)
            {
                this.StartSearchImage(this, new StartSearchImageEventArgs(this.wordKey, this.maxImageFiles));
            }
        }

        private void OnStartSearchTranslation(object sender, RoutedEventArgs e)
        {
            this.InitSetting();
            if (this.StartSearchTranslation != null)
            {
                this.StartSearchTranslation(this, new StartSearchTranslationEventArgs(this.wordKey, this.apiKeyDict));
            }
        }

        /// <summary>
        /// invoker loader Audio and Images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnStartSearchAll(object sender, RoutedEventArgs e)
        {
            this.InitSetting();
            if (this.StartSearchAudio != null)
            {
                this.StartSearchAudio(this, new StartSearchAudioEventArgs(this.wordKey, this.maxAudioFiles));
            }

            if (this.StartSearchImage != null)
            {
                this.StartSearchImage(this, new StartSearchImageEventArgs(this.wordKey, this.maxImageFiles));
            }

            if (this.StartSearchTranslation != null)
            {
                this.StartSearchTranslation(this, new StartSearchTranslationEventArgs(this.wordKey, this.apiKeyDict));
            }
        }

        protected virtual void OnGetListWordFromDataBase(string e)
        {
            var handler = GetListWordFromDataBase;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion

        #region helper method

        private void DispatcherEx(Delegate method)
        {
            if (this.CheckAccess())
            {
                method.DynamicInvoke();
            }
            else
            {
                this.Dispatcher.Invoke(DispatcherPriority.Normal, method);
            }
        }

        private void InitSetting()
        {
            this.wordKey = TextBoxSearchWord.Text.ToLower();
            if (NumMaxImage.Value != null)
            {
                this.maxImageFiles = (int)NumMaxImage.Value;
            }

            if (NumMaxAudio.Value != null)
            {
                this.maxAudioFiles = (int)NumMaxAudio.Value;
            }

            if (TextBoxApiKeyDict.Text != null)
            {
                this.apiKeyDict = TextBoxApiKeyDict.Text;
            }
        }
        #endregion

        protected virtual void OnShotdownApplication()
        {
            var handler = this.ShotdownApplication;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
