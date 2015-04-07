//-----------------------------------------------------------------------
// <copyright file="PlayMp3.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------


using System;
using System.IO;
using NAudio.Wave;
using Ruma.ExtensionEventArgs;

namespace Ruma.PlayMp3
{
    public class PlayMp3 : IDisposable
    {
        private AbortableBackgroundWorker.AbortableBackgroundWorker backgroundWorker;

        private IWavePlayer waveOutDevice;

        private byte[] sourceMp3 = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayMp3"/> class
        /// default constructor
        /// </summary>
        public PlayMp3()
        {
            this.waveOutDevice = new WaveOut();
        }

        public event EventHandler<MessageEventArgs> SendMessage;

        public void PlayStart(byte[] source)
        {
            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.Abort();
            }

            this.backgroundWorker = new AbortableBackgroundWorker.AbortableBackgroundWorker();
            this.backgroundWorker.DoWork += this.BackgroundWorker_DoWork;
            this.backgroundWorker.RunWorkerCompleted += (sender1, args) =>
            {
                if (args.Cancelled)
                {
                    if (args.Error != null)
                    {
                        //MessageBox.Show(string.Format("{0}: Long running task failed. Error: {1}", DateTime.Now.TimeOfDay, args.Error));
                    }
                }
                
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.Dispose();
                    this.backgroundWorker = null;
                }
            };

            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.sourceMp3 = source;
            this.backgroundWorker.RunWorkerAsync();
        }

        public void PlayStop()
        {
            if (this.backgroundWorker != null)
            {
                if (this.waveOutDevice != null)
                {
                    this.waveOutDevice.Stop();
                }
            }
        }

        public void Dispose()
        {
            if (this.waveOutDevice != null)
            {
                this.waveOutDevice.Stop();
            }

            if (this.waveOutDevice != null)
            {
                this.waveOutDevice.Dispose();
                this.waveOutDevice = null;
            }

            if (this.backgroundWorker != null)
            {
                this.backgroundWorker.Abort();
                if (this.backgroundWorker != null)
                {
                    this.backgroundWorker.Dispose();
                }

                this.backgroundWorker = null;
            }
        }

        protected virtual void OnSendMessage(MessageEventArgs e)
        {
            var handler = this.SendMessage;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void PlayMp3FromUrlOld(string url)
        {
            using (Stream ms = new MemoryStream())
            {
                using (FileStream file = new FileStream(url, FileMode.Open, FileAccess.Read))
                {
                    byte[] bytes = new byte[file.Length];
                    file.Read(bytes, 0, (int)file.Length);
                    ms.Write(bytes, 0, (int)file.Length);
                }

                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                    {
                        waveOut.Init(blockAlignedStream);
                        waveOut.Play();
                        while (waveOut.PlaybackState == PlaybackState.Playing)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
        }

        private void PlayMp3FromByteArrayOld(byte[] source)
        {
            using (Stream ms = new MemoryStream(source))
            {
                ms.Position = 0;
                using (WaveStream blockAlignedStream =
                    new BlockAlignReductionStream(
                        WaveFormatConversionStream.CreatePcmStream(
                            new Mp3FileReader(ms))))
                {
                    try
                    {
                        using (WaveOut waveOut = new WaveOut(WaveCallbackInfo.FunctionCallback()))
                        {
                            waveOut.Init(blockAlignedStream);
                            waveOut.Play();
                            while (waveOut.PlaybackState == PlaybackState.Playing)
                            {
                                System.Threading.Thread.Sleep(100);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        this.OnSendMessage(new MessageEventArgs(string.Format("Error play :/n{0}", e.Message), TypeMessage.Error));
                    }
                }
            }
        }

        private void PlayMp3FromByteArray(byte[] source)
        {
            using (Stream ms = new MemoryStream(source))
            {
                using (WaveStream ws = new Mp3FileReader(ms))
                {
                    this.waveOutDevice.Init(ws);
                    this.waveOutDevice.Play();
                    while (this.waveOutDevice.PlaybackState == PlaybackState.Playing)
                    {
                        System.Threading.Thread.Sleep(100);
                    }

                    this.waveOutDevice.Stop();
                }
            }
        }

        private void BackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (this.waveOutDevice != null && this.waveOutDevice.PlaybackState == PlaybackState.Playing)
            {
                this.waveOutDevice.Stop();
            }

            this.PlayMp3FromByteArray(this.sourceMp3);
        }
    }
}
