using System;

namespace Ruma.ExtensionEventArgs
{
    public class CheckedEventArgs : EventArgs
    {
        public int ID;
        public bool State;

        public CheckedEventArgs(int id, bool? state)
        {
            if (state == null) this.State = false;
            if (state != null) this.State = (bool)state;
            this.ID = id;
        }
    }
}