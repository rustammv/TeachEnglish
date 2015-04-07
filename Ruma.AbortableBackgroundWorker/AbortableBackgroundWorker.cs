namespace Ruma.AbortableBackgroundWorker
{
    using System.ComponentModel;
    using System.Threading;

    /// <summary>
    /// class to extend BackgroundWorker, added method Abort to stop thread
    /// </summary>
    public class AbortableBackgroundWorker : BackgroundWorker
    {
        /// <summary>
        /// private field thread
        /// </summary>
        private Thread workerThread;

        /// <summary>
        /// method to abort thread
        /// </summary>
        public void Abort()
        {
            if (this.workerThread != null)
            {
                this.workerThread.Abort();
                this.workerThread = null;
            }
        }

        /// <summary>
        /// Override event
        /// </summary>
        /// <param name="e">the object which is passed in the created thread</param>
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            this.workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; ////We must set Cancel property to true!
                Thread.ResetAbort(); ////Prevents ThreadAbortException propagation
            }
        }
    }}
