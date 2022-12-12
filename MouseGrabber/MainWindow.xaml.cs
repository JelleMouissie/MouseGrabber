using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using WpfAnimatedGif;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.IO;
using System.Media;
using System.Management;
using MouseGrabber.Properties;
using System.Drawing;
using System.Windows.Interop;

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

        public SoundPlayer clip1SoundPlayer;
        public SoundPlayer clip3SoundPlayer;

        public int state = 0;

        public bool ForceAudioValue;
        public int TickTimerValue;
        public int WaitTimerValue;
        public int MouseTimerValue;

        public MainWindow()
        {
            InitializeComponent();

            ShowOnMonitor(this);

            InitializeGifs();
            InitializeSounds();

            GetGlobalInformation();

            this.WindowState = WindowState.Maximized;
            this.ResizeMode = ResizeMode.NoResize;
            this.ShowInTaskbar = false;

            waitForMouseMove();

            //ForceAudio();

            StartClip1();
        }

        public void GetGlobalInformation()
        {
            PopUpWindow popup = new PopUpWindow();
            if (popup.ShowDialog().Value)
            {
                ForceAudioValue = popup.ForceAudioValue ?? false;
                bool waitParseSuccess = Int32.TryParse(popup.WaitTimerValue, out WaitTimerValue);
                if (!waitParseSuccess)
                {
                    WaitTimerValue = 60;
                }
                bool timeParseSuccess = Int32.TryParse(popup.TickTimerValue, out TickTimerValue);
                if (!timeParseSuccess)
                {
                    TickTimerValue = 47;
                }
                bool mouseParseSuccess = Int32.TryParse(popup.MouseTimerValue, out MouseTimerValue);
                if (!mouseParseSuccess)
                {
                    MouseTimerValue = 3;
                }
            }
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

        public void InitializeSounds()
        {
            string dir = Directory.GetCurrentDirectory();

            clip1SoundPlayer = new SoundPlayer();
            clip1SoundPlayer.SoundLocation = dir + "\\..\\..\\Sounds\\clip1Sound.wav";
            clip1SoundPlayer.Load();

            clip3SoundPlayer = new SoundPlayer();
            clip3SoundPlayer.SoundLocation = dir + "\\..\\..\\Sounds\\clip3Sound.wav";
            clip3SoundPlayer.Load();
        }

        #region https://social.msdn.microsoft.com/Forums/vstudio/en-US/2ca2fab6-b349-4c08-915f-373c71bd636a/show-and-maximize-wpf-window-on-a-specific-screen?forum=wpf

        private void ShowOnMonitor(Window window)
        {
            Screen screen = Screen.PrimaryScreen;

            window.WindowStyle = WindowStyle.None;
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            window.Left = screen.Bounds.Left;
            window.Top = screen.Bounds.Top;
            window.Show();
            window.WindowState = WindowState.Maximized;
        }

        #endregion

        public void ForceAudio()
        { 
            //USB\VID_0951&PID_16A4&MI_00\7&3321D09B&0&0000
        }

        public void FindAudioDevices()
        {
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(
           "SELECT * FROM Win32_SoundDevice");

            ManagementObjectCollection objCollection = objSearcher.Get();

            foreach (ManagementObject obj in objCollection)
            {
                Debug.WriteLine("-----------");
                foreach (PropertyData property in obj.Properties)
                {
                    Debug.WriteLine(String.Format("{0}:{1}", property.Name, property.Value));
                }
            }
        }

        public async void StartClip1()
        {
            state = 1;

            clip1SoundPlayer.Play();

            await Task.Delay(500);

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
            clip3SoundPlayer.Play();

            ImageBehavior.SetAnimatedSource(gifPlayer, clip3);
            ImageBehavior.SetRepeatBehavior(gifPlayer, new RepeatBehavior(1));

            state = 3;

            //ImageBehavior.SetAnimationSpeedRatio(gifPlayer, 0.25);

            //StartLogMouseloc();
            StartMouseManipulation();

            await Task.Delay(500);

            RedButton.Visibility = Visibility.Collapsed;
            InvisibleButton.Visibility = Visibility.Collapsed;

            //StartMouseManipulation();
        }

        public void ShutDown()
        {
            Environment.Exit(0);
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

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out System.Drawing.Point point);

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void waitForMouseMove()
        {
            System.Threading.Thread.Sleep(WaitTimerValue * 1000);

            int movedmouse = 0;

            System.Drawing.Point lastPoint = new System.Drawing.Point(0, 0); 

            while(movedmouse < 3)
            {
                System.Threading.Thread.Sleep(MouseTimerValue * 1000);

                System.Drawing.Point newPoint;
                GetCursorPos(out newPoint);

                if (lastPoint.X != newPoint.X || lastPoint.Y != newPoint.Y)
                {
                    movedmouse++;
                }
                else
                {
                    movedmouse = 0;
                }
                lastPoint = newPoint;
            }
        }


        private async Task StartMouseManipulation()
        {
            positionindex = 0;

            Timer timer = new Timer();
            timer.Tick += new EventHandler(SetMouseLocationEvent);
            timer.Interval = TickTimerValue;
            timer.Start();
        }

        int positionindex;

        private void SetMouseLocationEvent(object sender, object param)
        {
            if (positionindex >= recordedMousePositions.Count)
            {
                positionindex -= 1;
            }
            (int, int) pos = recordedMousePositions[positionindex];
            positionindex += 1;

            SetCursorPos(pos.Item1, pos.Item2);
        }

        private async Task StartLogMouseloc()
        {
            mousepositions = new List<(int, int)>();

            Timer timer = new Timer();
            timer.Tick += new EventHandler(logmouseLocationEvent);
            timer.Interval = 200;
            timer.Start();
        }

        private void logmouseLocationEvent(object sender, object param)
        {
            System.Drawing.Point point;
            GetCursorPos(out point);

            mousepositions.Add((point.X, point.Y));
        }


        public List<(int, int)> mousepositions;

        public List<(int, int)> recordedMousePositions = new List<(int, int)>()
        {
            (1490, 722),
            (1490, 722),
            (1490, 722),
            (1490, 722),
            (1490, 727),
            (1490, 736),
            (1490, 736),
            (1490, 736),
            (1490, 736),
            (1490, 736),
            (1490, 736),
            (1490, 736),
            (1491, 737),
            (1492, 738),
            (1492, 738),
            (1492, 738),
            (1492, 740),
            (1492, 740),
            (1491, 741),
            (1490, 742),
            (1490, 742),
            (1490, 742),
            (1490, 742),
            (1490, 742),
            (1489, 743),
            (1487, 744),
            (1483, 744),
            (1480, 741),
            (1475, 738),
            (1473, 738),
            (1471, 737),
            (1465, 737),
            (1462, 738),
            (1459, 739),
            (1456, 741),
            (1453, 742),
            (1450, 744),
            (1445, 751),
            (1445, 757),
            (1450, 772),
            (1454, 793),
            (1456, 812),
            (1459, 826),
            (1464, 842),
            (1474, 863),
            (1483, 887),
            (1502, 921),
            (1551, 967),
            (1568, 989),
            (1592, 1003),
            (1616, 1006),
            (1625, 1002),
            (1634, 996),
            (1639, 979),
            (1638, 962),
            (1635, 946),
            (1626, 921),
            (1610, 896),
            (1602, 885),
            (1601, 883),
            (1599, 891),
            (1604, 922),
            (1608, 942),
            (1608, 955),
            (1607, 966),
            (1605, 972),
            (1604, 975),
            (1602, 969),
            (1601, 965),
            (1601, 965),
            (1606, 965),
            (1607, 965),
            (1608, 965),
            (1608, 965),
            (1612, 965),
            (1616, 962),
            (1616, 962),
            (1621, 962),
            (1626, 963),
            (1628, 964),
            (1630, 966),
            (1631, 967),
            (1631, 967),
            (1631, 967),
            (1631, 967),
            (1631, 967),
            (1632, 968),
            (1632, 972),
            (1630, 974),
            (1629, 974),
            (1628, 972),
            (1645, 974),
            (1647, 975),
            (1647, 976),
            (1649, 976),
            (1649, 976),
            (1649, 976),
            (1649, 976),
            (1644, 977),
            (1640, 977),
            (1638, 977),
            (1636, 976),
            (1636, 975),
            (1635, 976),
            (1634, 977),
            (1633, 978),
            (1633, 978),
            (1627, 979),
            (1626, 980),
            (1624, 980),
            (1619, 981),
            (1619, 981),
            (1616, 982),
            (1616, 982),
            (1616, 982),
            (1620, 985),
            (1621, 986),
            (1624, 988),
            (1629, 991),
            (1631, 995), // dupe 6 dingen
            (1631, 995),
            (1638, 1002),
            (1638, 1002),//2
            (1651, 1008),
            (1651, 1008),//3
            (1671, 1015),
            (1671, 1015),//4
            (1679, 1018),
            (1682, 1020),
            (1684, 1024),
            (1690, 1028),
            (1692, 1031),
            (1686, 1036),
            (1700, 1044),
            (1711, 1048),
            (1725, 1049),
            (1757, 1051),
            (1785, 1053),
            (1826, 1056),
            (1860, 1054),
            (1880, 1042),
            (1919, 1021),
            (1919, 1004),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
            (1919, 1002),
        };
        
    }
}
