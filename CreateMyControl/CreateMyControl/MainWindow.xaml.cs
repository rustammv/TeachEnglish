using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Ruma.Controls;

namespace CreateMyControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<string> list = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            list.Add("mother");
            list.Add("mot");
            list.Add("moth" );
            list.Add("mom" );
            list.Add("other" );
            list.Add("dother" );
            list.Add("oth" );
            list.Add("ther" );
            list.Add("er" );
            list.Add("mo" );
            list.Add("mer" );
            list.Add("her" );
            txt.TextChanged += txt_TextChanged;
            txt.ListData = list;

        }

        void txt_TextChanged(object sender, TextChangedEventArgs e)
        {
            list.Add("dotherasdasdasdasdasdasdasdasdasdasdasdasd");

            TextBoxIntellisense tt = sender as TextBoxIntellisense;
            if (tt != null)
            {
                txtFilterText.Text = tt.Text;
            }
        }


        void btn_ClickButton(object sender, EventArgs e)
        {
            SystemSounds.Hand.Play();
        }

        void Part_CheckBox_Click(object sender, EventArgs e)
        {
            SystemSounds.Beep.Play();
        }



        private void OnMethodsSelectionKeyDown
                    (object sender, System.Windows.Input.KeyEventArgs e)
        {
            //switch (e.Key)
            //{
            //    case System.Windows.Input.Key.Enter:
            //        // Hide the Popup
            //        popupLinqMethods.IsOpen = false;

            //        ListBox lb = sender as ListBox;
            //        if (lb == null)
            //            return;

            //        // Get the selected item value
            //        string methodName = lb.SelectedItem.ToString();

            //        // Save the Caret position
            //        int i = txtFilterText.CaretIndex;

            //        // Add text to the text
            //        txtFilterText.Text = txtFilterText.Text.Insert(i, methodName);

            //        // Move the caret to the end of the added text
            //        txtFilterText.CaretIndex = i + methodName.Length;

            //        // Move focus back to the text box. 
            //        // This will auto-hide the PopUp due to StaysOpen="false"
            //        txtFilterText.Focus();
            //        break;

            //    case System.Windows.Input.Key.Escape:
            //        // Hide the Popup
            //        popupLinqMethods.IsOpen = false;
            //        break;
            //}
        }

        private void OnFilterTextKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            TextBox txtBox = sender as TextBox;
            if ((txtBox == null) || (txtBox.CaretIndex == 0))
                return;

            // Check for a predefined hot-key
            if (e.Key != System.Windows.Input.Key.OemPeriod)
                return;

            // Get the last word in the text (preceding the ".")
            string txt = txtBox.Text;
            int wordStart = txt.LastIndexOf(' ', txtBox.CaretIndex - 1);
            if (wordStart == -1)
                wordStart = 0;

            string lastWord = txt.Substring(wordStart, txtBox.CaretIndex - wordStart);

            // Check if the last word equal to the one we're waiting
            if (lastWord.Trim().ToLower() != "item.")
                return;

            ShowMethodsPopup(txtBox.GetRectFromCharacterIndex(txtBox.CaretIndex, true));
        }

        private void ShowMethodsPopup(Rect placementRect)
        {
            //lstMethodsSelection.SelectedIndex = 0;
            //lstMethodsSelection.Focus();
        }

        private void txt1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

    }


}
