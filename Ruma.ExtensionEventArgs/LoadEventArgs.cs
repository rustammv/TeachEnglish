//-----------------------------------------------------------------------
// <copyright file="LoadEventArgs.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------

using System;

namespace Ruma.ExtensionEventArgs
{
    /// <summary>
    /// class for argument's load event
    /// </summary>
    public class LoadEventArgs : EventArgs
    {
        #region fields

        /// <summary>
        /// Field: the operation name of the download process
        /// </summary>
        public string TextOperation;

        /// <summary>
        /// Field: the path of the loaded file
        /// </summary>
        public string FilePath;

        /// <summary>
        /// Field: the amount files of the download process
        /// </summary>
        public int AmountFiles;

        /// <summary>
        /// Field: the current downloadable file
        /// </summary>
        public int CurrentFile;

        /// <summary>
        /// Field: downloaded bytes of current file
        /// </summary>
        public long BytesReceived;

        /// <summary>
        /// Field: the total number of bytes of the current file
        /// </summary>
        public long TotalBytesToReceive;

        #endregion

        #region constructors

        /// <summary>
        /// Constructior: the default constructor
        /// </summary>
        public LoadEventArgs()
        {
        }

        /// <summary>
        /// Constructior: the constructor with parameters
        /// </summary>
        /// <param name="textOperation"> the operation name of the download process</param>
        /// <param name="amountFiles">the amount files of the download process
        /// default value = 0</param>
        /// <param name="currentFile">the current downloadable file
        /// default value = 0</param>
        /// <param name="filePath">the path of the loaded file, 
        /// default value = empty</param>
        /// <param name="bytesReceived">downloaded bytes of current file
        /// default value = 0</param>
        /// <param name="totalBytesToReceive">the total number of bytes of the current file
        /// default value = 0</param>
        public LoadEventArgs(string textOperation, int currentFile = 0, int amountFiles = 0, string filePath = "",
            long bytesReceived = 0, long totalBytesToReceive = 0)
        {
            this.TextOperation = textOperation;
            this.AmountFiles = amountFiles;
            this.CurrentFile = currentFile;
            this.FilePath = filePath;
            this.BytesReceived = bytesReceived;
            this.TotalBytesToReceive = totalBytesToReceive;
        }

        #endregion
    }
}