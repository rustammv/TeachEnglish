//-----------------------------------------------------------------------
// <copyright file="StartUp.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------

using TeachEnglish.Views;

namespace TeachEnglish.Presenter
{
    using TeachEnglish.BL;
    using TeachEnglish.Views;
    using System.Windows;

    public class StartUp
    {
        public partial class App : Application
        {
            [System.STAThreadAttribute()]
            [System.Diagnostics.DebuggerNonUserCodeAttribute()]
            public static void Main()
            {
                RumaMainWindow window = new RumaMainWindow();
                MessageService service = new MessageService();
                MainPresenter presenter = new MainPresenter(window, new BusinessModel(), service);
                App app = new App();
                app.Run(window);
            }
        }
    }
}
