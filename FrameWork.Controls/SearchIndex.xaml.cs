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

namespace FrameWork.Controls
{
    /// <summary>
    /// Interaction logic for Array.xaml
    /// </summary>
    public partial class SearchIndex : UserControl
    {
        public SearchIndex()
        {
            this.InitializeComponent();
        }

        public event EventHandler<RoutedEventArgs> OnClick;

        public string ReturnContent()
        {
            string ReturnContent = string.Empty;

            for (int i = 0; i < ArrayRadio.Children.Count; i++)
            {
                if (((RadioButton)ArrayRadio.Children[i]).IsChecked == true)
                {
                    ReturnContent = ((RadioButton)ArrayRadio.Children[i]).Content.ToString();
                }
            }

            return ReturnContent;
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            if (OnClick != null)
                OnClick(this, e);
        }

    }
}