using System;

namespace Ruma.ExtensionEventArgs
{
    public class StartSearchAudioEventArgs : EventArgs
    {
        public string WordKey;
        public int MaxCountFiles;

        public StartSearchAudioEventArgs(string wordkey, int maxCountFiles)
        {
            this.WordKey = wordkey;
            this.MaxCountFiles = maxCountFiles;
        }
    }
}