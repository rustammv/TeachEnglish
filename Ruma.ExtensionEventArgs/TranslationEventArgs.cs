//-----------------------------------------------------------------------
// <copyright file="TranslationEventArgs.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------

using System;

namespace Ruma.ExtensionEventArgs
{
    public class TranslationEventArgs : EventArgs
    {
        public string Translation;
        public string PartOfSpeach;
        public string Transcription;

        public TranslationEventArgs()
        {
        }

        public TranslationEventArgs(string traslation, string pos, string transcription)
        {
            this.Translation = traslation;
            this.PartOfSpeach = pos;
            this.Transcription = transcription;
        }

    }
}