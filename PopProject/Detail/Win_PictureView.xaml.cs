using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PopProject.Detail
{
    /// <summary>
    /// Win_PictureView.xaml에 대한 상호 작용 논리
    /// </summary>

    public partial class Win_PictureView : Window
    {
        private Point scrollMousePoint = new Point();
        private double hOff = 1;
        private double vOff = 1;

        public Win_PictureView(ImageSource imageSource)
        {
            InitializeComponent();
            pic.Source = imageSource;
        }

        //이미지 확대 
        private void Image_enlargement(object sender, MouseButtonEventArgs e)
        {
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //'pic.Stretch = Stretch.None
            myScaleTransform.ScaleX *= 1.1;
            myScaleTransform.ScaleY *= 1.1;
        }

        // 이미지 축소
        private void Image_reduction(object sender, MouseButtonEventArgs e)
        {
            sv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //' pic.Stretch = Stretch.None
            myScaleTransform.ScaleX /= 1.1;
            myScaleTransform.ScaleY /= 1.1;
        }

        // 이미지 로테이션
        private void Image_rotation(object sender, MouseButtonEventArgs e)
        {
            myRotateTransform.Angle += 90;
        }

        // 팝업 종료
        private void Image_exit(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ScrollViewer_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            scrollMousePoint = e.GetPosition(sv);
            hOff = sv.HorizontalOffset;
            vOff = sv.VerticalOffset;
            _ = sv.CaptureMouse();
        }
        private void ScrollViewer_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            sv.ReleaseMouseCapture();
        }

        private void ScrollViewer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (sv.IsMouseCaptured)
            {
                sv.ScrollToHorizontalOffset(hOff + (scrollMousePoint.X - e.GetPosition(sv).X));
                sv.ScrollToVerticalOffset(vOff + (scrollMousePoint.Y - e.GetPosition(sv).Y));
            }
        }
    }
}
