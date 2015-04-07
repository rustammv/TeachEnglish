//-----------------------------------------------------------------------
// <copyright file="BusinessModel.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.BL
{
    public class BusinessModel
    {
        public ILoaderAudio LoaderAudio = new LoaderAudio();

        public ILoaderImages LoaderImages = new LoaderImages();

        public ILoaderTranslation LoaderTranslation = new LoaderTranslation();
    }
}