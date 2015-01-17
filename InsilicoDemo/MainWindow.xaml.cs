using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Insilico;

namespace InsilicoDemo {

    /*
     * TODO
     *  - fix no-graph click on empty canvas (event handlers trigger a null exception)
     *  - mouse-over control for multiple displays (get rid of Cached.graph)
     * 
     * 
     * */


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window {

        Engine insilico = new Insilico.Engine();
        Random rand = new Random();

        public MainWindow() {
            InitializeComponent();
            this.Title = "InsilicoDemo";
            canvas.Background = Cached.DarkestBrown;
            #region Engine setup
            int tier = RenderCapability.Tier >> 16;
            // RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly; 

            // Example graph
            Graph myGraph = new Graph();
            GraphLayout myLayout = new GraphLayout();
            myGraph.DefaultVertexStyleTemplate = Styles.Green_VertexStyle;
            myGraph.DefaultEdgeStyleTemplate = Styles.GreenGlass_EdgeStyle;
            insilico.displays.Add(myGraph);
            myGraph.drawEdgeArrows = false;
            Cached.graph = insilico.graph;

            insilico.graph = myGraph;
            Cached.graph = myGraph;

            insilico.canvas = canvas;
            insilico.bShowAnimations = true;
            insilico.layout = myLayout;
            insilico.bPrintEdgeNode = false;
            insilico.bSizeVertexToText = true;
            insilico.bShowObjectCoordinates = false;
            insilico.bShowAnimations = true;
            insilico.bCompressLeaves = false;
            insilico.bShowEdges = true;

            insilico.Start();
            #endregion

            #region Displays
            VitalIndicator vital0 = new VitalIndicator();
            insilico.displays.Add(vital0);
            vital0.Height = 400;
            vital0.Width = 20;
            vital0.yo = 300;
            vital0.xo = 50;
            //vital0.elementColor = Cached.BrushRed;
            vital0.Activate();
            
            VitalIndicator vital1 = new VitalIndicator();
            insilico.displays.Add(vital1);
            vital1.Height = 400;
            vital1.Width = 20;
            vital1.yo = 300;
            vital1.xo = 110;
            //vital1.elementColor = Cached.BrushLimeGreen;
            vital1.Activate();
            
            VitalIndicator vital2 = new VitalIndicator();
            insilico.displays.Add(vital2);
            vital2.Height = 400;
            vital2.Width = 20;
            vital2.yo = 300;
            vital2.xo = 170;
            //vital2.elementColor = Cached.BrushDodgerBlue;
            vital2.Activate();
            
            LinePlot lp1 = new LinePlot(10);
            //lp1.Layout.valueColorScheme = Scheme.SimpleRYG;
            insilico.displays.Add(lp1);
            lp1.Height = 200;
            lp1.Width = 300;
            lp1.yo = 50;
            lp1.xo = 50;
            lp1.Activate();

            LinePlot lp2 = new LinePlot(10);
            //lp2.Layout = Layouts.LinePlotBlue;
            //lp2.Layout.valueColorScheme = Scheme.SimpleRYG;
            insilico.displays.Add(lp2);
            lp2.Height = 200;
            lp2.Width = 600;
            lp2.yo = 50;
            lp2.xo = 750;
            lp2.Activate();

            Histogram h3 = new Histogram(25);
            //h3.Layout.valueColorScheme = Scheme.SimpleRYG;
            h3.Height = 200;
            h3.Width = 300;
            h3.xo = 400;
            h3.yo = 50;
            insilico.displays.Add(h3);
            h3.Activate();

            EEG e0 = new EEG(15);
            e0.Height = 400;
            e0.Width = 350;
            e0.xo = 250;
            e0.yo = 300;
            insilico.displays.Add(e0);
            e0.stepCount = 15;
            e0.Layout.bShowPoints = false;
            e0.min = -1;
            e0.Activate();

            EEG e1 = new EEG(100);
            e1.Height = 400;
            e1.Width = 700;
            e1.xo = 650;
            e1.yo = 300;
            insilico.displays.Add(e1);
            e1.Layout.bShowPoints = false;
            e1.stepCount = 400;
            e1.min = -1;
            e1.handle = "test";
            e1.Activate();

            EEG e2 = new EEG(100);
            e2.Height = 150;
            e2.Width = 1300;
            e2.xo = 50;
            e2.yo = 750;
            insilico.displays.Add(e2);
            e2.Layout.bShowPoints = false;
            e2.stepCount = 400;
            e2.min = -1;
            e2.Activate();

            #endregion

            #region Graph Test
            /*
            Vertex v0 = new Vertex();
            v0.label = "Neuron_49586F";
            Vertex v1 = new Vertex();
            v1.label = "Subsystem-9";
            Vertex v2 = new Vertex();
            v2.label = "Output_Signal";

            v0.type = 5;
            v1.type = 6;
            v2.type = 7;

            v0.attachedDisplay = h0;
            //v1.attachedDisplay = lp1;
            //v2.attachedDisplay = lp0;

            myGraph.Add(v0);
            myGraph.Add(v1);
            myGraph.Add(v2);
            myGraph.Root = v0;
            myGraph.CreateUnidirectionalEdge(v0, v1, insilico.DefaultEdgeStyleTemplate);
            myGraph.CreateUnidirectionalEdge(v1, v2, insilico.DefaultEdgeStyleTemplate);

            //insilico.GenerateLabels();
            //insilico.Compute_RadialExpansionLayout(insilico.graph, 0, 0, (float)canvas.ActualWidth, (float)canvas.ActualHeight);
            //insilico.RequestCompute();

            v0.coordinates = Engine.ToWPFCoords(new Point(rand.Next(-50, 50), rand.Next(-50, 50)), 1100, 800);
            v1.coordinates = Engine.ToWPFCoords(new Point(rand.Next(-50, 50), rand.Next(-50, 50)), 1100, 800);
            v2.coordinates = Engine.ToWPFCoords(new Point(rand.Next(-50, 50), rand.Next(-50, 50)), 1100, 800);
            */
            #endregion

            // Simulated data source to feed the various display objects in the rendering engine
            SimulatedData myDataSource = new SimulatedData();
            myDataSource.canvasWidth = 1100.0f;
            myDataSource.canvasHeight = 800.0f;
            myDataSource.graph = myGraph;
            myDataSource.Start();
        }

        #region Events
        private void LeftMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnLeftMouseDown(sender, e); }
        private void RightMouseDown(object sender, MouseButtonEventArgs e) { insilico.OnRightMouseDown(sender, e); }
        private void MouseMove(object sender, MouseEventArgs e) { insilico.OnMouseMove(sender, e); }
        private void ClickUp(object sender, RoutedEventArgs e) { insilico.OnClickUp(sender, e); }
        private void OnSizeChanged(object sender, RoutedEventArgs e) { insilico.OnSizeChanged(sender, e); }
        private void OnMouseWheel(object sender, MouseWheelEventArgs e) { insilico.OnMouseWheel(sender, e); }
        #endregion
    }
}
