using System.Windows.Controls;

namespace TeachEnglish.View.Controls
{
    /// <summary>
    /// Interaction logic for LoadingProcess.xaml
    /// </summary>
    public partial class LoadingProcess : UserControl
    {
        /// <summary>
        /// sets the initial values
        /// </summary>
        public void ResetProgressBar()
        {
            this.ProgressBarCountFile.Maximum = 1;
            this.ProgressBarCountFile.Value = 0;
            this.LabelProgressBarCountFile.Content = string.Format("Loaded {0} is {1} files", 0, 0);
            this.ProgressBarDownloadFile.Maximum = 1;
            this.ProgressBarDownloadFile.Value = 0;
            this.LabelProgressBarDownloadFile.Content = string.Format("Loaded {0} is {1} bytes", 0, 0);
        }

        public LoadingProcess()
        {
            InitializeComponent();
        }
    }
}
