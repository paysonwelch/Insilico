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

namespace MultiCanvasDemo {
    public partial class MainWindow: Window {

        Engine insilico = new Insilico.Engine();
        Random rand = new Random();

        public MainWindow() {
            InitializeComponent();
            this.Title = "MultiCanvasDemo";

            #region Engine setup
            insilico.bShowAnimations = true;        // Enable node-physics and smooth display transitions
            insilico.Start();                       // Start the background thread (for animations and physics)

            insilico.bEnableSimulatedData = true;  // <------------------ Enable/Disable simulated data here
            #endregion

            #region Display Objects 

            EEG eeg1 = new EEG(100);
            eeg1.TargetCanvas = RedCanvas;
            eeg1.Layout = Layouts.Spartan;
            insilico.displays.Add(eeg1);
            eeg1.max = 7;
            eeg1.min = 0;
            eeg1.Activate();
            
            
            EEG eeg0 = new EEG(30);
            eeg0.TargetCanvas = BlueCanvas;
            eeg0.Layout = Layouts.Spartan;
            insilico.displays.Add(eeg0);
            eeg0.max = 10;
            eeg0.min = 0;
            eeg0.Activate();


            Graph graph = new Graph();
            graph.TargetCanvas = BlackCanvas;
            graph.DefaultVertexStyleTemplate = Styles.Green_VertexStyle;
            graph.DefaultEdgeStyleTemplate = Styles.GreenGlass_EdgeStyle;
            graph.DefaultVertexStyleTemplate.vertexColor = Cached.BrushLimeGreen;
            graph.DefaultVertexStyleTemplate.vertexOpacity = 0.7;
            insilico.displays.Add(graph);
            graph.Activate();


            EEG eeg3 = new EEG(30);
            eeg3.TargetCanvas = GreenCanvas;
            eeg3.Layout = Layouts.Spartan;
            insilico.displays.Add(eeg3);
            eeg3.max = 10;
            eeg3.min = 0;
            eeg3.Activate();


            EEG eeg4 = new EEG(30);
            eeg4.TargetCanvas = YellowCanvas;
            eeg4.Layout = Layouts.Spartan;
            insilico.displays.Add(eeg4);
            eeg4.max = 10;
            eeg4.min = 0;
            eeg4.Activate();
             

            #endregion
        }

        /*
        double FirstXPos;
        double FirstYPos;

        double FirstArrowXPos;
        double FirstArrowYPos;

        Canvas MovingObject;

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T: DependencyObject {
            if (depObj != null) {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++) {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T) {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child)) {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            //In this event, we get current mouse position on the control to use it in the MouseMove event.
            FirstXPos = e.GetPosition(sender as Control).X;
            FirstYPos = e.GetPosition(sender as Control).Y;

            foreach (Canvas tb in FindVisualChildren<Canvas>(this)) {
                if (tb.IsMouseOver) {
                    MovingObject = tb;

                    FirstArrowXPos = e.GetPosition(tb).X - FirstXPos;
                    FirstArrowYPos = e.GetPosition(tb).Y - FirstYPos;
                }
            }

            
            //MovingObject = (Canvas)sender;
        }

        void PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            MovingObject = null;
        }

        private void MouseMove(object sender, MouseEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                MovingObject.SetValue(Canvas.LeftProperty, FirstArrowXPos);
                MovingObject.SetValue(Canvas.TopProperty, FirstArrowYPos);

                
                (MovingObject as FrameworkElement).SetValue(Canvas.LeftProperty,
                     e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).X - FirstArrowXPos);

                (MovingObject as FrameworkElement).SetValue(Canvas.TopProperty,
                     e.GetPosition((MovingObject as FrameworkElement).Parent as FrameworkElement).Y - FirstArrowYPos);
            }
        }
*/
    }


}
