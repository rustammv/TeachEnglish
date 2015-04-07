namespace Ruma.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    public class TextBoxIntellisense : TextBox
    {
        private Popup part_Popup;
        private ListBox part_ListBox;
        public List<string> ListData { get; set; }

        
        static TextBoxIntellisense()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TextBoxIntellisense), new FrameworkPropertyMetadata(typeof(TextBoxIntellisense)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.TextChanged += part_TextBox_TextChanged;
            this.PreviewKeyDown += part_TextBox_PreviewKeyDown;

            const string popupName = "PART_Popup";
            this.part_Popup = GetTemplateChild(popupName) as Popup;
            Debug.Assert(this.part_Popup != null, "Не найден элемент с именем " + popupName);

            const string listboxName = "PART_ListBox";
            this.part_ListBox = GetTemplateChild(listboxName) as ListBox;
            Debug.Assert(this.part_ListBox != null, "Не найден элемент с именем " + listboxName);
            this.part_ListBox.PreviewKeyDown += part_ListBox_PreviewKeyDown;
            this.part_ListBox.MouseDoubleClick += part_ListBox_MouseDoubleClick;
        }

        public TextBoxIntellisense()
        {
            ListData = new List<string>();
            ListData.Add("mOther");
            ListData.Add("otHer");
            ListData.Add("mama");
            ListData.Add("thIs");
            ListData.Add("ther");
            ListData.Add("foregro");
            ListData.Add("GRid");
            ListData.Add("God");
            ListData.Add("rid");
        }

        #region helper method

        private TextBlock CreateTextBlockWithBackLight(string text, string textSearch)
        {
            SolidColorBrush foregroundMain = (SolidColorBrush) (new BrushConverter().ConvertFrom("#FFaaaaaa"));
            SolidColorBrush foregroundLight = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF007ACC"));
            SolidColorBrush backgroundMain = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF28282B"));
            string s = text;
            TextBlock textBlock = new TextBlock();
            if (string.IsNullOrEmpty(textSearch))
            {
                textBlock.Inlines.Add(new Run(s) { Foreground = foregroundMain, Background = backgroundMain});
                return textBlock;
            }

            while (s.Length > 0)
            {
                int i = s.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase);
                if (i > 0)
                {
                    string sub = s.Substring(0, i);
                    textBlock.Inlines.Add(new Run(sub) { Foreground = foregroundMain, Background = backgroundMain });
                    s = s.Remove(0, i);
                }

                if (i == 0)
                {
                    string sub = s.Substring(0, textSearch.Length);
                    textBlock.Inlines.Add(new Run(sub) { Foreground = foregroundLight, Background = backgroundMain});
                    s = s.Remove(0, textSearch.Length);
                }

                if (i == -1)
                {
                    textBlock.Inlines.Add(new Run(s) { Foreground = foregroundMain, Background = backgroundMain });
                    s = "";
                }

            }
            return textBlock;
        }

        private string InLinesToString(InlineCollection collection)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (Inline c in collection)
            {
                if (c is LineBreak)
                    stringBuilder.AppendLine();
                else if (c is Run)
                    stringBuilder.Append(((Run)c).Text);

            }
            return stringBuilder.ToString();
        }

        private void CreateListData()
        {
            List<string> tempList = string.IsNullOrEmpty(this.Text)
                ? ListData
                : ListData.Where(x => x.ToLower().Contains(this.Text.ToLower())).ToList();

            this.part_ListBox.Items.Clear();
            foreach (string s in tempList)
            {
                this.part_ListBox.Items.Add(new ListBoxItem { Content = CreateTextBlockWithBackLight(s, this.Text) });
            }

            if (this.part_ListBox.Items.Count < 1)
            {
                this.part_Popup.IsOpen = false;
            }
            else
            {
                this.part_ListBox.SelectedIndex = 0;
            }
        }

        private void SetTextFromPopup()
        {
            if (this.part_ListBox.SelectedIndex > -1)
            {
                // get listboxitem 
                TextBlock textBlock = ((ListBoxItem)this.part_ListBox.SelectedValue).Content as TextBlock;
                if (textBlock != null)
                {
                    this.Text = InLinesToString(textBlock.Inlines);
                    this.part_Popup.IsOpen = false;
                    this.Focus();
                }
            }
        }

        #endregion

        private void part_ListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SetTextFromPopup();
            e.Handled = true;
        }

        void part_ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                SetTextFromPopup();
                e.Handled = true;
            }
        }

        private void part_TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.part_Popup.IsOpen = false;
                return;
            }

            if (e.Key == Key.Down)
            {
                if (this.part_Popup.IsOpen == false)
                {
                    this.part_Popup.IsOpen = true;
                    CreateListData();
                }
            }

            if (e.Key == Key.Tab || e.Key == Key.Enter)
            {
                SetTextFromPopup();                        
                e.Handled = true;
            }

            if (this.part_ListBox.Items.Count > 0)
            {
                if (e.Key == Key.Down)
                {
                    if (this.part_ListBox.Items.Count - 1 > this.part_ListBox.SelectedIndex)
                    {
                        this.part_ListBox.SelectedIndex++;
                    }
                }
                if (e.Key == Key.Up)
                {
                    if (this.part_ListBox.SelectedIndex > 0)
                    {
                        this.part_ListBox.SelectedIndex--;
                    }
                }
                this.part_ListBox.ScrollIntoView(this.part_ListBox.Items[this.part_ListBox.SelectedIndex]);
            }

        }

        void part_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            this.part_Popup.IsOpen = true;
            CreateListData();
        }


    }
}
