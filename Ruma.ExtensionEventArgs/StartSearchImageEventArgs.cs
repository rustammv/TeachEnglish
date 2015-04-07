using System;

namespace Ruma.ExtensionEventArgs
{
    public class StartSearchImageEventArgs:EventArgs
    {
        public string WordKey;
        public int MaxCountFiles;

        public StartSearchImageEventArgs(string wordkey, int maxCountFiles)
        {
            this.WordKey = wordkey;
            this.MaxCountFiles = maxCountFiles;
        }
    }
}