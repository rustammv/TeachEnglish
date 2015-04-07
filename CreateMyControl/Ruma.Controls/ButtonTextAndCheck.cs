namespace Ruma.Controls
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;

    
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ruma.Controls"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:Ruma.Controls;assembly=Ruma.Controls"
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
    public class ButtonTextAndCheck : Control
    {
        public static readonly RoutedEvent CheckBoxCheckStateChangedEvent = EventManager.RegisterRoutedEvent(
            "CheckBoxCheckStateChanged", RoutingStrategy.Bubble, typeof(CheckStateEventHandler), typeof(ButtonTextAndCheck));

        public static readonly RoutedEvent ButtonClickEvent = EventManager.RegisterRoutedEvent(
            "ButtonClick", RoutingStrategy.Bubble, typeof(EventHandler), typeof(ButtonTextAndCheck));


        /// <summary>
        /// При переопределении в производном классе вызывается каждый раз, когда код приложения 
        /// или внутренние процессы вызывают <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            const string checkBoxName = "PART_CheckBox";
            var chBox = GetTemplateChild(checkBoxName) as CheckBox;
            Debug.Assert(chBox != null, "Не найден элемент с именем " + checkBoxName);
            chBox.Checked += CheckBox_Checked;
            chBox.Unchecked += CheckBox_Unchecked;

            const string buttonName = "PART_Button";
            var btn = GetTemplateChild(buttonName) as Button;
            Debug.Assert(btn != null, "Не найден элемент с именем " + buttonName);
            btn.Click += Button_Click;
        }

        /// <summary>
        /// Вызывается при изменении шаблона элемента управления.
        /// </summary>
        /// <param name="oldTemplate">Прежний шаблон.</param><param name="newTemplate">Новый шаблон.</param>
        protected override void OnTemplateChanged(ControlTemplate oldTemplate, ControlTemplate newTemplate)
        {
            const string checkBoxName = "PART_CheckBox";
            var chBox = GetTemplateChild(checkBoxName) as CheckBox;
            if (chBox != null)
            {
                chBox.Checked -= CheckBox_Checked;
                chBox.Unchecked -= CheckBox_Unchecked;
            }

            const string buttonName = "PART_Button";
            var btn = GetTemplateChild(buttonName) as Button;
            if (btn != null)
            {
                btn.Click -= Button_Click;
            }

            base.OnTemplateChanged(oldTemplate, newTemplate);
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnButtonClick(e);
        }

        protected void OnButtonClick(RoutedEventArgs e)
        {
            var newEventArgs = new RoutedEventArgs(ButtonClickEvent, e.Source);
            RaiseEvent(newEventArgs);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            IsCheckBoxChecked = false;
            OnCheckBoxClicked(e, false);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            IsCheckBoxChecked = true;
            OnCheckBoxClicked(e, true);
        }

        protected void OnCheckBoxClicked(RoutedEventArgs e, bool isChecked)
        {
            var newEventArgs = new CheckStateRoutedEventArgs(CheckBoxCheckStateChangedEvent, e.Source, isChecked);
            RaiseEvent(newEventArgs);
        }

        /// <summary>
        ///     Происходит, когда элемент <see cref="CheckBox"/> изменяет своё состояние выбран или не выбран
        /// </summary>
        public event CheckStateEventHandler CheckBoxCheckStateChanged
        {
            add { AddHandler(CheckBoxCheckStateChangedEvent, value); }
            remove { RemoveHandler(CheckBoxCheckStateChangedEvent, value); }
        }

        public event EventHandler ButtonClick
        {
            add { AddHandler(ButtonClickEvent, value); }
            remove { RemoveHandler(ButtonClickEvent, value); }
        }

        public static readonly DependencyProperty IsCheckBoxCheckedProperty;
        public static readonly DependencyProperty TextButtonProperty;
        public static readonly DependencyProperty IDProperty;

        public bool IsCheckBoxChecked
        {
            get { return (bool)GetValue(IsCheckBoxCheckedProperty); }
            set
            {
                SetValue(IsCheckBoxCheckedProperty, value);
                const string checkBoxName = "PART_CheckBox";
                var chBox = GetTemplateChild(checkBoxName) as CheckBox;
                if (chBox != null)
                {
                    chBox.IsChecked = value;
                }
            }
        }

        public int ID
        {
            get { return (int)GetValue(IDProperty); }
            set { SetValue(IDProperty, value); }
        }


        public string TextButton
        {
            get { return (string) GetValue(TextButtonProperty); }
            set { SetValue(TextButtonProperty, value); }
        }

        static ButtonTextAndCheck()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonTextAndCheck), new FrameworkPropertyMetadata(typeof(ButtonTextAndCheck)));
            IsCheckBoxCheckedProperty = DependencyProperty.Register("IsCheckBoxChecked", typeof(bool), typeof(ButtonTextAndCheck), new PropertyMetadata(default(bool)));
            TextButtonProperty = DependencyProperty.Register("TextButton", typeof (string), typeof (ButtonTextAndCheck), new PropertyMetadata(default(string)));
            IDProperty = DependencyProperty.Register("ID", typeof(int), typeof(ButtonTextAndCheck), new PropertyMetadata(default(int)));
        }

        public ButtonTextAndCheck():this(0, "")
        {
        }

        public ButtonTextAndCheck(int id = 0, string text = "")
        {
            this.ID = id;
            this.TextButton = text;
        }

    }
}
