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

using Insilico;

namespace SuperSimpleDemo {
    
    public partial class MainWindow: Window {

        Engine insilico = new Insilico.Engine();
        Random rand = new Random();

        public MainWindow() {
            InitializeComponent();
            this.Title = "SuperSimpleDemo";

            #region Engine setup
            insilico.canvas = MyCanvas;
            insilico.bShowAnimations = true;
            insilico.Start();
            #endregion

            #region Display Objects (LinePlots, Histograms, VitalIndicators, EEGs, Networks, etc)
            LinePlot lp1 = new LinePlot(10);
            insilico.displays.Add(lp1);
            lp1.Height = 400;
            lp1.Width = 400;
            lp1.yo = 30;
            lp1.xo = 30;
            lp1.Activate();
            #endregion
        }

        #region Mouse Events (if you need them)
        private void LeftMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnLeftMouseDown(sender, e); }
        private void RightMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnRightMouseDown(sender, e); }
        private void MoveMouse(object sender, MouseEventArgs e) { insilico.OnMouseMove(sender, e); }
        private void ClickUp(object sender, RoutedEventArgs e) { insilico.OnClickUp(sender, e); }
        private void OnSizeChanged(object sender, RoutedEventArgs e) { insilico.OnSizeChanged(sender, e); }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e) { insilico.OnMouseWheel(sender, e); }
        #endregion
    }
}
