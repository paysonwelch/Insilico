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

namespace DemoGraph {

    public partial class MainWindow: Window {

        Engine insilico = new Insilico.Engine();
        Random rand = new Random();

        public MainWindow() {
            InitializeComponent();
            this.Title = "DemoGraph";

            #region Engine setup
            insilico.bShowAnimations = true;        // Enable node-physics and smooth display transitions
            insilico.Start();                       // Start the background thread (for animations and physics)

            insilico.bEnableSimulatedData = true;   // <------------------ Enable/Disable simulated data here
            #endregion

            Graph graph = new Graph();
            graph.TargetCanvas = MyCanvas;
            graph.DefaultVertexStyleTemplate = Styles.Green_VertexStyle;
            graph.DefaultEdgeStyleTemplate = Styles.GreenGlass_EdgeStyle;
            graph.DefaultVertexStyleTemplate.vertexColor = Cached.BrushLimeGreen;
            graph.DefaultVertexStyleTemplate.vertexOpacity = 0.7;
            insilico.displays.Add(graph);
            graph.Activate();
        }
    }
}
