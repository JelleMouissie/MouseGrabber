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

namespace MouseGrabber
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
        }

        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
            this.Activate();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartMouseManipulation();
        }


        private List<Movement> positions = new List<Movement>()
        {
            new Movement((75, 75), (200, 200), 3),
        };



        private void waitForMouseMove()
        {

        }

        private int roundDoubleToInt(double d)
        {
            return Convert.ToInt32(Math.Round(d));
        }

        private double CalculateCurrentValue(double start, double end, double tick, double totalTicks)
        {
            return start + (tick / totalTicks) * (end - start);
        }

        private void StartMouseManipulation()
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

                    System.Threading.Thread.Sleep(10);
                }

                DateTime newdate = DateTime.Now;

                TimeSpan ts = newdate - currentdate;
                double f = ts.TotalMilliseconds;

            }
        }




    }
}
