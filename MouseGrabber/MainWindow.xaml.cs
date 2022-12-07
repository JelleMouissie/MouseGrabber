using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WpfAnimatedGif;
using System.Windows.Media.Animation;
using System.Threading;

namespace MouseGrabber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public BitmapImage clip1;
        public BitmapImage clip2;
        public BitmapImage clip3;

        public int state = 0;

        private struct Movement
        {
            public (int, int) StartPos;
            public (int, int) EndPos;
            public int Time;

            public Movement((int, int) startPos, (int, int) endPos, int time)
            {
                StartPos = startPos;
                EndPos = endPos;
                Time = time;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;

            InitializeGifs();

            StartClip1();
        }

        public void InitializeGifs()
        {
            clip1 = new BitmapImage();
            clip1.BeginInit();
            clip1.UriSource = new Uri("gifs/clip1edited.gif", UriKind.Relative);
            clip1.EndInit();

            clip2 = new BitmapImage();
            clip2.BeginInit();
            clip2.UriSource = new Uri("gifs/clip2edited.gif", UriKind.Relative);
            clip2.EndInit();

            clip3 = new BitmapImage();
            clip3.BeginInit();
            clip3.UriSource = new Uri("gifs/clip3edited.gif", UriKind.Relative);
            clip3.EndInit();
        }

        public async void StartClip1()
        {
            state = 1;

            ImageBehavior.SetAnimatedSource(gifPlayer, clip1);
            ImageBehavior.SetAutoStart(gifPlayer, true);
            ImageBehavior.SetRepeatBehavior(gifPlayer, new RepeatBehavior(1));

            await Task.Delay(11500);
            RedButton.Visibility = Visibility.Visible;
            InvisibleButton.Visibility = Visibility.Visible;
        }

        public void Startclip2()
        {
            state = 2;

            ImageBehavior.SetAnimatedSource(gifPlayer, clip2);
            ImageBehavior.SetRepeatBehavior(gifPlayer, RepeatBehavior.Forever);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Startclip3();
        }

        public async Task Startclip3()
        {
            state = 3;

            ImageBehavior.SetAnimatedSource(gifPlayer, clip3);
            ImageBehavior.SetRepeatBehavior(gifPlayer, new RepeatBehavior(1));

            await Task.Delay(500);

            RedButton.Visibility = Visibility.Collapsed;
            InvisibleButton.Visibility = Visibility.Collapsed;

            StartMouseManipulation();
        }

        public void ShutDown()
        {
            this.Close();
        }

        private void gifPlayer_AnimationCompleted(object sender, RoutedEventArgs e)
        {
            if (state == 1)
            {
                Startclip2();
            }

            if (state == 3)
            {
                ShutDown();
            }
        }


        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void waitForMouseMove()
        {

        }


        private List<Movement> positions = new List<Movement>()
        {
            new Movement((335, 335), (335, 335), 3),
        };

        private int roundDoubleToInt(double d)
        {
            return Convert.ToInt32(Math.Round(d));
        }

        private double CalculateCurrentValue(double start, double end, double tick, double totalTicks)
        {
            return start + (tick / totalTicks) * (end - start);
        }

        private async Task StartMouseManipulation()
        {
            foreach (Movement movement in positions)
            {
                int ticks = movement.Time * 100;

                DateTime currentdate = DateTime.Now;

                for (int i = 0; i < ticks; i++)
                {
                    int x = roundDoubleToInt(CalculateCurrentValue(movement.StartPos.Item1, movement.EndPos.Item1, i, ticks));
                    int y = roundDoubleToInt(CalculateCurrentValue(movement.StartPos.Item2, movement.EndPos.Item2, i, ticks));

                    SetCursorPos(roundDoubleToInt(x), roundDoubleToInt(y));

                    await Task.Delay(10);
                }

                DateTime newdate = DateTime.Now;

                TimeSpan ts = newdate - currentdate;
                double f = ts.TotalMilliseconds;

            }
        }
    }
}
