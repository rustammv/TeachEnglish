//-----------------------------------------------------------------------
// <copyright file="AboutBox.xaml.cs" company="Ruma">
//     Copyright (c) Ruma. All rights reserved.
// </copyright>
// <author>Rustam Muratov</author>
//-----------------------------------------------------------------------

using TeachEnglish.View.AboutDataProviders;

namespace TeachEnglish
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Windows;

    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox"/> class.
        /// </summary>
        public AboutBox()
        {
            this.InitializeComponent();
            ////Assembly app = Assembly.GetExecutingAssembly();
            ////AssemblyTitleAttribute title = (AssemblyTitleAttribute)app.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
            ////AssemblyProductAttribute product = (AssemblyProductAttribute)app.GetCustomAttributes(typeof(AssemblyProductAttribute), false)[0];
            ////AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)app.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
            ////AssemblyCompanyAttribute company = (AssemblyCompanyAttribute)app.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false)[0];
            ////AssemblyDescriptionAttribute description = (AssemblyDescriptionAttribute)app.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false)[0];
            ////Version version = app.GetName().Version;
            AboutAssemblyDataProvider ap = new AboutAssemblyDataProvider();
            this.Title = ap.Title;
            this.productName.Content = ap.Product;
            this.version.Content = ap.Version;
            this.copyright.Content = ap.Copyright;
            this.company.Content = ap.Company;
            this.description.Text = ap.Description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AboutBox"/> class.
        /// </summary>
        /// <param name="parent"></param>
        public AboutBox(Window parent)
            : this()
        {
            this.Owner = parent;
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && string.IsNullOrEmpty(e.Uri.OriginalString) == false)
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }
    }
}
