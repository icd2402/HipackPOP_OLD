using System.Windows;
using System.Windows.Input;
using PopProject.Util;

namespace PopProject.Time_pick
{
    /// <summary>
    /// Time_pick.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Time_pick : Window
    {
        private int hour = SettingJsonManager.hour;
        private int minuite = SettingJsonManager.minute;

        public Time_pick()
        {
            InitializeComponent();
            tbHour.Text = hour + "";
            tbMinute.Text = minuite + "";
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            SettingJsonManager.hour = hour;
            SettingJsonManager.minute = minuite;
            SettingJsonManager.SaveJsonFile();
            Close();
        }

        private void Hour_up(object sender, RoutedEventArgs e)
        {

            if (hour + 1 > 23)
            {
                hour = 0;
            }
            else
            {
                hour += 1;
            }
            tbHour.Text = hour + "";
        }

        private void Hour_down(object sender, RoutedEventArgs e)
        {

            if (hour - 1 < 0)
            {
                hour = 23;
            }

            else
            {
                hour -= 1;
            }

            tbHour.Text = hour + "";

        }

        private void Minuite_up(object sender, RoutedEventArgs e)
        {

            if (minuite + 1 > 59)
            {
                minuite = 0;
            }

            else
            {
                minuite += 1;
            }

            tbMinute.Text = minuite + "";

        }

        private void Minuite_down(object sender, RoutedEventArgs e)
        {
            if (minuite - 1 < 0)
            {
                minuite = 59;
            }

            else
            {
                minuite -= 1;
            }

            tbMinute.Text = minuite + "";

        }


        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
