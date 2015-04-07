using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ruma.Controls
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ruma.CustomControls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ruma.CustomControls;assembly=Ruma.CustomControls"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class Radio3LinesText : RadioButton
    {
        public static readonly DependencyProperty Text1LineProperty;
        public static readonly DependencyProperty Text2LineProperty;
        public static readonly DependencyProperty Text3LineProperty;
        //public static readonly DependencyProperty GroupNameProperty;
        public static readonly DependencyProperty IDProperty;

        public string Text1Line
        {
            get { return (string)GetValue(Text1LineProperty); }
            set { SetValue(Text1LineProperty, value); }
        }
        public string Text2Line
        {
            get { return (string)GetValue(Text2LineProperty); }
            set { SetValue(Text2LineProperty, value); }
        }
        public string Text3Line
        {
            get { return (string)GetValue(Text3LineProperty); }
            set { SetValue(Text3LineProperty, value); }
        }

        //public string GroupName
        //{
        //    get { return (string)GetValue(GroupNameProperty); }
        //    set { SetValue(GroupNameProperty, value); }
        //}

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }



        static Radio3LinesText()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Radio3LinesText), new FrameworkPropertyMetadata(typeof(Radio3LinesText)));

            Text1LineProperty = DependencyProperty.Register("Text1Line", typeof(string), typeof(Radio3LinesText), new PropertyMetadata(default(string)));
            Text2LineProperty = DependencyProperty.Register("Text2Line", typeof(string), typeof(Radio3LinesText), new PropertyMetadata(default(string)));
            Text3LineProperty = DependencyProperty.Register("Text3Line", typeof(string), typeof(Radio3LinesText), new PropertyMetadata(default(string)));
            //GroupNameProperty = DependencyProperty.Register("GroupName", typeof(string), typeof(Radio3LinesText), new PropertyMetadata(default(string)));
            IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(Radio3LinesText), new PropertyMetadata(default(int)));
        }

        public Radio3LinesText()
            : this(0, "Empty", "Empty", "Empty", "TranslationDefault")
        {

        }
        public Radio3LinesText(int id = 0, string text1Line = "Empty", string text2Line = "Empty", string text3Line = "Empty", string group = "TranslationDefault")
        {
            this.Text1Line = text1Line;
            this.Text2Line = text2Line;
            this.Text3Line = text3Line;
            this.ID = id;
            this.GroupName = group;
        }
    }
}
