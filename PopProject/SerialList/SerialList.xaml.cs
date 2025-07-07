using PopProject.Util;
using System;
using System.Windows;

namespace PopProject.SerialList
{
    /// <summary>
    /// SerialList.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SerialList : Window
    {
        public SerialList(string[] portName)
        {
            InitializeComponent();
            cb.ItemsSource = portName;
            cb.SelectedItem = SettingJsonManager.SerialName;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (cb.SelectedItem != null)
            {

                SettingJsonManager.SerialName = cb.SelectedItem.ToString();
                SettingJsonManager.SaveJsonFile();

            }
        }
    }
}
