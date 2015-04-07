using System;

namespace Ruma.ExtensionEventArgs
{
    public class StartSearchTranslationEventArgs : EventArgs
    {
        public string WordKey;
        public string ApiKeyDict;

        public StartSearchTranslationEventArgs(string wordkey, string apiKeyDict)
        {
            this.WordKey = wordkey;
            this.ApiKeyDict = apiKeyDict;
        }         
    }
}