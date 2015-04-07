namespace Ruma.Controls
{
    using System.Windows;
    
    public delegate void CheckStateEventHandler(object sender, CheckStateRoutedEventArgs e);

    public class CheckStateRoutedEventArgs : RoutedEventArgs
    {
        public CheckStateRoutedEventArgs(RoutedEvent routedEvent, object source, bool isChecked)
            :base(routedEvent, source)
        {
            IsChecked = isChecked;
        }

        public bool IsChecked { get; protected set; }
    }
}