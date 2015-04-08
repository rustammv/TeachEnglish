using System.Windows.Controls;

namespace Ruma.WpfUserControls
{
    /// <summary>
    /// Interaction logic for ListBAC.xaml
    /// </summary>
    public partial class ListBac : System.Windows.Controls.UserControl
    {
        public StackPanel SP;
        public ListBac()
        {
            InitializeComponent();
            this.SP = SpListButton;
        }
    }
}

