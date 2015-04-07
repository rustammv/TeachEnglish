//-----------------------------------------------------------------------
// <copyright file="LoaderTranslation.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.BL
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Ruma.ExtensionEventArgs;
    using YandexApi;

    /// <summary>
    /// interface for translation of the words
    /// </summary>
    public interface ILoaderTranslation
    {
        /// <summary>
        /// event start working method GettingTranslation
        /// </summary>
        event EventHandler GettingTranslationStart;

        /// <summary>
        /// event the next version of the translation
        /// occurs when you receive a new version of the translation in the method GettingTranslation
        /// </summary>
        event EventHandler<TranslationEventArgs> GettingOneTranslationComplete;

        /// <summary>
        /// event complete working method GettingTranslation
        /// </summary>
        event EventHandler<int> GettingTranslationComplete;

        /// <summary>
        /// event error messages
        /// </summary>
        event EventHandler<MessageEventArgs> SendMessage;

        /// <summary>
        /// Gets field stores the results of processing method GettingTranslation
        /// </summary>
        List<YDictionary> DictResult { get; }

        /// <summary>
        /// Gets a value indicating whether indicating the execution of the async method GettingTranslation 
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// asynchronous a method of obtaining translations in the public field Dict
        /// uses YandexApi Dictionary
        /// </summary>
        /// <param name="searchKeyWord">the word for translation</param>
        /// <param name="apiKey">the key to access the "Yandex Api Dictionary"
        /// which can be obtained by reference https://tech.yandex.ru/dictionary/
        /// </param>
        void GettingTranslation(string searchKeyWord, string apiKey);
    }

    /// <summary>
    /// the class implements the interface ILoaderTranslation
    /// </summary>
    public class LoaderTranslation : ILoaderTranslation
    {
        #region field

        private Task task;

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        private string apiKeyDict = string.Empty;

        private string wordKey = string.Empty;

        #endregion

        #region constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderTranslation"/> class
        /// default constructor
        /// </summary>
        public LoaderTranslation()
        {
            this.DictResult = new List<YDictionary>();
        }
        #endregion

        #region event

        /// <summary>
        /// event start working method GettingTranslation
        /// </summary>
        public event EventHandler GettingTranslationStart;

        /// <summary>
        /// event the next version of the translation
        /// occurs when you receive a new version of the translation in the method GettingTranslation
        /// </summary>
        public event EventHandler<TranslationEventArgs> GettingOneTranslationComplete;

        /// <summary>
        /// event complete working method GettingTranslation
        /// </summary>
        public event EventHandler<int> GettingTranslationComplete;

        /// <summary>
        /// event error messages
        /// </summary>
        public event EventHandler<MessageEventArgs> SendMessage;

        #endregion

        #region property

        /// <summary>
        /// Gets field stores the results of processing method GettingTranslation
        /// </summary>
        public List<YDictionary> DictResult { get; private set; }

        /// <summary>
        /// Gets a value indicating whether indicating the execution of the async method GettingTranslation 
        /// </summary>
        public bool IsLoading { get; private set; }

        #endregion

        /// <summary>
        /// asynchronous a method of obtaining translations in the public field Dict
        /// uses YandexApi Dictionary
        /// </summary>
        /// <param name="searchKeyWord">the word for translation</param>
        /// <param name="apiKey">the key to access the "Yandex Api Dictionary"
        /// which can be obtained by reference https://tech.yandex.ru/dictionary/
        /// </param>
        public virtual void GettingTranslation(string searchKeyWord, string apiKey)
        {
            this.wordKey = searchKeyWord;
            this.apiKeyDict = apiKey;
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
            this.OnGettingTranslationStart();
            this.task = Task.Factory.StartNew(this.LoadTranslation, this.tokenSource.Token, this.tokenSource.Token);
        }

        protected virtual void LoadTranslation(object ct)
        {
            CancellationToken cancelTok = (CancellationToken)ct;
            if (this.DictResult != null)
            {
                this.DictResult.Clear();
            }

            ApiDictionary apiDict = new ApiDictionary(this.apiKeyDict);
            this.DictResult = apiDict.GetTranslation(Lang.EnRu, this.wordKey);

            int loadedFilesCount = 0;

            for (int i = 0; i < this.DictResult.Count; i++)
            {
                if (!cancelTok.IsCancellationRequested)
                {
                    foreach (YTranslation translation in this.DictResult[i].Translations)
                    {
                        TranslationEventArgs e = new TranslationEventArgs
                        {
                            PartOfSpeach = this.DictResult[i].AttrPartOfSpeech,
                            Transcription = this.DictResult[i].AttrTranscription
                        };
                        e.Translation = translation.AttrTranslation;
                        this.OnGettingOneTranslationComplete(e);
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
                this.OnGettingTranslationComplete(loadedFilesCount);
            }
        }

        #region invoсator events

        /// <summary>
        /// invocator event GettingTranslationStart
        /// </summary>
        protected virtual void OnGettingTranslationStart()
        {
            this.IsLoading = true;
            var handler = this.GettingTranslationStart;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// invocator event GettingTranslationComplete
        /// </summary>
        /// <param name="e">amount translation</param>
        protected virtual void OnGettingTranslationComplete(int e)
        {
            var handler = this.GettingTranslationComplete;
            if (handler != null)
            {
                handler(this, e);
            }

            this.IsLoading = false;
        }

        /// <summary>
        /// invocator event GettingOneTranslationComplete
        /// </summary>
        /// <param name="e">result of translator</param>
        protected virtual void OnGettingOneTranslationComplete(TranslationEventArgs e)
        {
            var handler = this.GettingOneTranslationComplete;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// invocator event SendMessage
        /// </summary>
        /// <param name="e">type and text message</param>
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