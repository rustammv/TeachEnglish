//-----------------------------------------------------------------------
// <copyright file="MessageService.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------
namespace TeachEnglish.BL
{
    using System.Windows;

    public interface IMessageService
    {
        void MessageError(string message);
    }

    public class MessageService : IMessageService
    {
        public void MessageError(string message)
        {
            MessageBox.Show(message);
        }
    }
}