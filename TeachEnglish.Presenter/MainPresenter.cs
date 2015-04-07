//-----------------------------------------------------------------------
// <copyright file="MainPresentor.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.Presenter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using Ruma.ExtensionEventArgs;
    using TeachEnglish.BL;
    using TeachEnglish.SQLite.DAL;
    using TeachEnglish.View;
    using YandexApi;

    /// <summary>
    /// Class presenter for MVP pattern
    /// </summary>
    public class MainPresenter
    {
        private IView view;
        private BusinessModel business;
        private IMessageService messageService;
        private SqLiteService dataBaseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPresenter"/>
        /// </summary>
        /// <param name="view">view</param>
        /// <param name="business">business logic</param>
        /// <param name="service">service message</param>

        public MainPresenter(IView view, BusinessModel business, IMessageService service)
        {
            this.view = view;
            this.business = business;
            this.messageService = service;
            this.dataBaseService = new SqLiteService();

            this.business.LoaderImages.SearchResultCount = 5;
            this.view.StartSearchAudio += this.View_StartSearch;
            this.view.StartSearchImage += this.View_StartSearchImage;
            this.view.StartSearchTranslation += this.View_StartSearchTranslation;

            this.view.SelectImage += this.View_SelectImage;
            this.view.SelectAudio += this.View_SelectAudio;
            this.view.GetCheckedData += this.View_GetCheckedData;
            this.view.UnCheckedAllImage += this.View_UnCheckedAllImage;
            this.view.GetListWordFromDataBase += this.View_GetListWordFromDataBase;
            this.view.ShotdownApplication += view_ShotdownApplication;

            this.business.LoaderAudio.LoadingStart += this.LoaderAudio_LoadingStart;
            this.business.LoaderAudio.LoadingProcessChanged += this.LoaderAudio_LoadingProcessChanged;
            this.business.LoaderAudio.LoadingFileComplite += this.LoaderAudio_LoadingFileComplite;
            this.business.LoaderAudio.LoadingComplite += this.LoaderAudio_LoadingComplite;

            this.business.LoaderImages.LoadingStart += this.LoaderImages_LoadingStart;
            this.business.LoaderImages.LoadingProcessChanged += this.LoaderImages_LoadingProcessChanged;
            this.business.LoaderImages.LoadingFileComplite += this.LoaderImages_LoadingFileComplite;
            this.business.LoaderImages.LoadingComplite += this.LoaderImages_LoadingComplite;

            this.business.LoaderTranslation.GettingTranslationStart += this.LoaderTranslationGettingTranslationStart;
            this.business.LoaderTranslation.GettingOneTranslationComplete += this.LoaderTranslationGettingOneTranslationComplite;
            this.business.LoaderTranslation.GettingTranslationComplete += this.LoaderTranslationGettingTranslationComplite;
            //// Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<TeachEnglishDbModel>());
        }

        void view_ShotdownApplication(object sender, EventArgs e)
        {
            StartUp.App.Current.Shutdown();
        }

        #region view

        private void View_GetListWordFromDataBase(object sender, string e)
        {
            List<string> list = this.dataBaseService.GetListWord(e, 50);
            if (list == null)
            {
                list = new List<string>();
            }

            this.view.CreateListIntellisense(list);
        }

        private void View_UnCheckedAllImage(object sender, EventArgs e)
        {
            this.business.LoaderImages.UnCheckedAllImage();
        }

        private void View_StartSearchTranslation(object sender, StartSearchTranslationEventArgs e)
        {
            this.business.LoaderTranslation.GettingTranslation(e.WordKey, e.ApiKeyDict);
        }

        /// <summary>
        /// create log checked image and audio, passed that string to view 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void View_GetCheckedData(object sender, int idTranslarion)
        {
            string text = string.Empty;
            List<bool> checkAudio = this.business.LoaderAudio.ListAudioCheck;
            int countCheckedAudio = this.business.LoaderAudio.ListAudioCheck.Count(x => x == true);
            List<bool> checkImage = this.business.LoaderImages.GetListImageCheck;
            int countCheckedImage = this.business.LoaderImages.GetListImageCheck.Count(x => x == true);
            text += string.Format("Amount checked audio {0}\n", countCheckedAudio);
            for (int j = 0; j < checkAudio.Count; j++)
            {
                if (checkAudio[j])
                {
                    text += string.Format("\tchecked audio № {0}\n", j);
                }
            }

            text += string.Format("Amount checked image {0}\n", countCheckedImage);
            for (int j = 0; j < checkImage.Count; j++)
            {
                if (checkImage[j])
                {
                    text += string.Format("\tchecked image № {0}\n", j);
                }
            }

            this.view.FillTExtLog(text);
            List<byte[]> listAudio = this.business.LoaderAudio.GetListAudioChecked();
            List<byte[]> listImages = this.business.LoaderImages.GetListImageChecked();

            var dict = this.business.LoaderTranslation.DictResult;
            int tempId = 0;
            string posEn = string.Empty;
            string posRu = string.Empty;
            string word = string.Empty;
            string transcript = string.Empty;
            string translation = string.Empty;

            foreach (var d in dict)
            {
                foreach (YTranslation t in d.Translations)
                {
                    if (tempId == idTranslarion)
                    {
                        posEn = d.AttrPartOfSpeech;
                        posRu = t.AttrPartOfSpeech;
                        word = d.AttrText;
                        transcript = d.AttrTranscription;
                        translation = t.AttrTranslation;
                    }

                    tempId++;
                }
            }

            if (translation != string.Empty)
            {
                this.dataBaseService.UpdateWord(word, translation, posEn, posRu, listImages, listAudio, transcript);
            }

            // _dataBaseService.ClearDataBase();
        }

        private void View_SelectImage(object sender, CheckedEventArgs e)
        {
            this.business.LoaderImages.CheckImage(e.ID, e.State);
        }

        private void View_SelectAudio(object sender, CheckedEventArgs e)
        {
            this.business.LoaderAudio.CheckAudio(e.ID, e.State);
        }

        private void View_StartSearchImage(object sender, StartSearchImageEventArgs e)
        {
            this.business.LoaderImages.StartLoad(e.WordKey, e.MaxCountFiles);
        }

        private void View_StartSearch(object sender, StartSearchAudioEventArgs e)
        {
            this.business.LoaderAudio.StartLoad(e.WordKey, e.MaxCountFiles);
        }

        #endregion

        #region BL translation loader mothod

        private void LoaderTranslationGettingTranslationComplite(object sender, int e)
        {
            this.view.ListTranslationLoadingComplite(e);
        }

        private void LoaderTranslationGettingOneTranslationComplite(object sender, TranslationEventArgs e)
        {
            this.view.ListTransAddTranslation(e.Translation, e.PartOfSpeach, e.Transcription);
        }

        private void LoaderTranslationGettingTranslationStart(object sender, EventArgs e)
        {
            this.view.ListTranslationClear();
        }

        #endregion

        #region BL images loader method

        private void LoaderImages_LoadingStart(object sender, EventArgs e)
        {
            this.view.ListImageClear();
            this.view.ListImageShowProgress();
        }

        private void LoaderImages_LoadingProcessChanged(object sender, LoadEventArgs e)
        {
            this.view.ListImageProgressChanged(e.TextOperation, e.CurrentFile, e.AmountFiles, e.BytesReceived,
                e.TotalBytesToReceive);
        }

        private void LoaderImages_LoadingFileComplite(object sender, BitmapImage e)
        {
            this.view.ListImageAddImage(e);
        }

        private void LoaderImages_LoadingComplite(object sender, int e)
        {
            this.view.ListImageHideProgress(e);
        }

        #endregion

        #region BL audio loader method

        private void LoaderAudio_LoadingStart(object sender, EventArgs e)
        {
            this.view.ListAudioClear();
            this.view.ListAudioShowProgress();
        }

        private void LoaderAudio_LoadingProcessChanged(object sender, LoadEventArgs e)
        {
            this.view.ListAudioProgressChanged(e.TextOperation, e.AmountFiles, e.CurrentFile, e.BytesReceived,
                e.TotalBytesToReceive);
        }

        private void LoaderAudio_LoadingFileComplite(object sender, byte[] source)
        {
            this.view.ListAudioAddAudio(source);
        }

        private void LoaderAudio_LoadingComplite(object sender, int e)
        {
            this.view.ListAudioHideProgress(e);
        }

        #endregion
    }
}