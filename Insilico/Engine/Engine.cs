using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Diagnostics;

namespace Insilico {
    public partial class Engine : BaseThread {

        #region Vars/Configuration
        public Canvas canvas;

        public List<BaseDisplay> displays = new List<BaseDisplay>();

        public bool bPrintEdgeNode = true;              // Print the quasi-vertex in the midpoint of an edge
        public bool bSizeVertexToText = true;           // Automatically resize vertices so text will fit
        public bool bShowObjectCoordinates = true;      // Print coordinates for all objects on screen
        public bool bCompressLeaves = true;             // Keep leaf-vertices closer to their parents 
        bool bRequestRender = false;                    // Tells the background worker that we want to re-draw all screen elements
        bool bRequestCompute = false;                   // Tells the background worker that we want to re-compute locations for all screen elements
        bool bRequestComputeNoDensity = false;          // Tells the background worker that we want to re-compute locations for all screen elements WITHOUT performing a density survey
        public bool bShowAnimations = false;            // Whether or not we want the background worker to process VertexAnimationStyles
        public bool bUseStandardTypeface = true;
        public Typeface standardTypeface;               // Prevents us from having to re-render a new typeface for each label and for each render
        public bool bRequestSnapBack;
        public Vertex snapBackVertex;
        Point snapBackOrigin;
        public bool bShowDensityRegions = false;
        public bool bShowEdges = true;
        Random rand = new Random(); 

        public float vSpace = 35;                       // The closest we want any two sibling-nodes
        public float clusterPadding = 100;              // A little extra space for each cluster
        public float overlapProbability = 1.0f;
        public float expansion = 1.1f;
        public int minLeftPadding = 100;

        public List<TextBlock> labels = new List<TextBlock>();
        #endregion

        #region Threading
        public delegate void UpdateTextCallback(BaseDisplay currDisplay);
        public Delegate threadBridge;
        public object[] dummy = new object[] { "" };
        public Engine() { threadBridge = new UpdateTextCallback(this.SendToGUI); }

        // Test data
        public double increment = 0.05;
        public double totalMultiplier = 0;
        public double theta = Math.PI*2;
        public bool bEnableSimulatedData = false;

        /// <summary>
        /// Main engine render cycle. This is called from the overridden Engine.RunThread() method.
        /// </summary>
        /// <param name="currDisplay"> The current display object we're rendering</param>
        public void SendToGUI(BaseDisplay currDisplay) {
            #region Process render/computation requests
            if (bRequestCompute) {
                //GenerateLabels();
                //RenderGraph(graph, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);
                bRequestCompute = false;
                bRequestComputeNoDensity = false;
            }
            if (bRequestRender) {
                //GenerateLabels();
                //RenderGraph(graph, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);
                bRequestRender = false;
            }
            #endregion

            #region Render animations
            if (bShowAnimations) {
                if (displays.Any()) {

                    TimeSpan ts = DateTime.Now - Cached.startTime;
                    double ticker = ts.TotalMilliseconds;

                    #region EEG
                    // EEG
                    if (currDisplay is EEG) {
                        EEG eeg = (EEG)currDisplay;
                        if (bEnableSimulatedData) {
                            if (totalMultiplier >= 2) {
                                totalMultiplier = 0;
                            }
                            else {
                                totalMultiplier += increment;
                            }
                            float divisor = 1;
                            if (eeg.handle == "test") {
                                divisor = (float)rand.Next(1, 5);
                            }
                            eeg.PushDatum((float)Math.Cos(Math.PI * totalMultiplier) / divisor);
                        }
                        eeg.Compute();
                        eeg.Render(eeg.TargetCanvas);
                    }
                    #endregion

                    #region VitalIndicator
                    // VITAL INDICATOR
                    if (currDisplay is VitalIndicator) {
                        VitalIndicator vital = (VitalIndicator)currDisplay;
                        if (bEnableSimulatedData) {
                            if (Math.Round(ticker, 0) % 9 == 0) {
                                vital.SetData(rand.NextDouble());
                            }
                        }
                        if (vital.stepsRemaining > 0) {
                            vital.Step();
                            vital.Compute();
                            vital.Render(vital.TargetCanvas);
                        }
                    }
                    #endregion

                    #region Graph
                    if (currDisplay is Graph) {
                        Graph gr = (Graph)currDisplay;
                        gr.Compute();
                        gr.Render(gr.TargetCanvas);
                        if (Math.Round(ticker, 0) % 9 == 0) {
                            /*Vertex newVertex = new Vertex();
                            newVertex.style = Styles.Green_VertexStyle;
                            newVertex.label = "" + Math.Round(ticker, 0);
                            newVertex.coordinates = ToWPFCoords();
                            newVertex.box = Primitives.CreateEllipse(newVertex.coordinates.X, newVertex.coordinates.Y, 20, 20, newVertex.style.vertexColor);
                            newVertex.box.Opacity = newVertex.style.vertexOpacity;
                            //gr.elements.Add(newVertex.box);
                            gr.CreateUnidirectionalEdge(newVertex, gr.GetRandomVertex());
                            gr.Add(newVertex);
                             * */

                            Point p = ToWPFCoords(new Point(rand.Next(-50, 50), rand.Next(-50, 50)), (float)gr.TargetCanvas.ActualWidth, (float)gr.TargetCanvas.ActualHeight);
                            Vertex newVertex = gr.CreateNewVertex((int)p.X, (int)p.Y, 30, "" + Math.Round(ticker, 0));
                            gr.CreateUnidirectionalEdge(newVertex, gr.GetRandomVertex());
                        }
                        

                        // Vertex animations
                        /*
                        foreach (Vertex v in gr.vertices.Values) {
                            if (v.animation != null) {
                                if (Math.Abs(ticker - v.animation.lastPulse) >= v.animation.pulseDelayLength || v.animation.lastPulse == 0) {
                                    v.animation.bPulseOn = !v.animation.bPulseOn;
                                    v.animation.lastPulse = ticker;
                                }
                            }
                        }
                        * */
                        // Arrows
                        // Cached.arrowPercentage += 0.01f;
                        // if (Cached.arrowPercentage >= 0.90f) { Cached.arrowPercentage = 0.10f; }
                        // RenderGraph(gr, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);

                    }
                    #endregion

                    #region Histogram
                    if (currDisplay is Histogram) {
                        Histogram hist = (Histogram)currDisplay;
                        if (hist.stepsRemaining > 0) {
                            hist.Step();
                            hist.Compute();
                            hist.Render(hist.TargetCanvas);
                        }
                        if (hist.stepsRemaining == 0) {
                            if (bEnableSimulatedData) {
                                List<float> newSeries = new List<float>();
                                for (int i = 0; i < hist.pointCount; i++) { newSeries.Add(rand.Next(0, 100)); }
                                hist.SetData(newSeries.ToArray());
                            }
                        }
                    }
                    #endregion

                    #region LinePlot
                    if (currDisplay is LinePlot) {
                        LinePlot hist = (LinePlot)currDisplay;
                        if (hist.stepsRemaining > 0) {
                            hist.Step();
                            hist.Compute();
                            hist.Render(hist.TargetCanvas);
                        }
                        if (hist.stepsRemaining == 0) {
                            if (bEnableSimulatedData) {
                                List<float> newSeries = new List<float>();
                                for (int i = 0; i < hist.pointCount; i++) { newSeries.Add(rand.Next(0, 100)); }
                                hist.SetData(newSeries.ToArray());
                            }
                        }
                    }
                    #endregion
                }
            }
            #endregion
        }

        public override void RunThread() {
            while (!_shouldStop) {
                if (!_shouldPause) {
                    if (_shouldSleep) {
                        Thread.Sleep(_sleepTime);
                        _sleepTime = 0;
                        _shouldSleep = false;
                    }
                    #region Code to Execute
                    //canvas.Dispatcher.Invoke(threadBridge, dummy);
                    for (int i = 0; i < displays.Count(); i++) { 
                        if (displays[i].IsActive) {
                            // FIXME: Possibly a more efficient way to do this?
                            displays[i].TargetCanvas.Dispatcher.Invoke(threadBridge, displays[i]); 
                        }
                    }
                    #endregion
                }
                else { Thread.Sleep(400); } // Extra long slumber while we're paused.
                Thread.Sleep(30);
            }
        }

        #region Operation Requests
        public void RequestComputeNoDensity() {
            bRequestCompute = true;
            bRequestComputeNoDensity = true;
        }
        public void RequestCompute() { bRequestCompute = true; }
        public void RequestRender() { bRequestRender = true; }
        public void RequestSnapBack(Vertex parent, Point origin) {
            bRequestSnapBack = true;
            snapBackVertex = parent;
            snapBackOrigin = origin;
        }
        #endregion

        #endregion

        #region Coordinate conversions
        // Convert our coordinates from a system centered at (0,0) to the strange fucked up world of WPF coordinates
        public static Point ToWPFCoords(Point c, float canvasWidth, float canvasHeight) { return new Point((float)((canvasWidth / 2.0) + c.X), (float)((canvasHeight / 2.0) - c.Y)); }
        public static Point ToWPFCoords(int x, int y, float canvasWidth, float canvasHeight) { return new Point((float)((canvasWidth / 2.0) + x), (float)((canvasHeight / 2.0) - y)); }
        public static Point ToCenteredCoords(Point c, float canvasWidth, float canvasHeight) { return new Point((float)(c.X - (canvasWidth / 2.0)), (float)((canvasHeight / 2.0) - c.Y)); }
        #endregion
    }
}
