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
using System.Windows.Shapes;

namespace MouseGrabber
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        public PopUpWindow()
        {
            InitializeComponent();
        }

        public bool? ForceAudioValue { get; set; }
        public string WaitTimerValue { get; set; }
        public string TickTimerValue { get; set; }
        public string MouseTimerValue { get; set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ForceAudioValue = ForceAudio.IsChecked;
            WaitTimerValue = WaitTimer.Text;
            TickTimerValue = TickTimer.Text;
            MouseTimerValue = MouseTimer.Text;

            DialogResult = true;

            this.Close();
        }
    }
}
