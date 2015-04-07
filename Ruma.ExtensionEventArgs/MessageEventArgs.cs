//-----------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------

using System;

namespace Ruma.ExtensionEventArgs
{
    public enum TypeMessage
    {
        Error = -1,
        Exclamation = 0,
        Normal = 1
    }

    public class MessageEventArgs : EventArgs
    {
        public string TextMessage;
        public TypeMessage TypeMessage;

        public MessageEventArgs()
        {
        }

        public MessageEventArgs(string textMessage, TypeMessage typeMessage)
        {
            this.TextMessage = textMessage;
            this.TypeMessage = typeMessage;
        }
    }
}