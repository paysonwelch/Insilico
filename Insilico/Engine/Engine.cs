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
        public VertexStyleTemplate DefaultVertexStyleTemplate;
        public EdgeStyleTemplate DefaultEdgeStyleTemplate;
        public GraphLayout layout;
        public Graph graph;
        public Histogram histogram;
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
        public delegate void UpdateTextCallback(string message);
        public Delegate threadBridge;
        public object[] dummy = new object[] { "" };
        public Engine() { threadBridge = new UpdateTextCallback(this.SendToGUI); }

        // Test data
        public double increment = 0.05;
        public double totalMultiplier = 0;
        public double theta = Math.PI*2;


        public void SendToGUI(string text) {
            #region Process render/computation requests
            if (bRequestCompute) {
                GenerateLabels();
                ComputeTypedRadialGraphLayout(graph, 0.0f, 0.0f, (float)canvas.ActualWidth, (float)canvas.ActualHeight, !bRequestComputeNoDensity);
                RenderGraph(graph, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);
                bRequestCompute = false;
                bRequestComputeNoDensity = false;
            }
            if (bRequestRender) {
                GenerateLabels();
                RenderGraph(graph, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);
                bRequestRender = false;
            }
            #endregion

            #region Render animations
            if (bShowAnimations) {

                if (displays.Any()) {
                    foreach (BaseDisplay disp in displays) {
                        if (disp.IsActive) {
                            TimeSpan ts = DateTime.Now - Cached.startTime;
                            double ticker = ts.TotalMilliseconds;

                            // EEG
                            if (disp is EEG) {
                                EEG eeg = (EEG)disp;
                                //if (Math.Round(ticker, 0) % 2 == 0) {
                                    //eeg.PushDatum((float)rand.NextDouble());

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
                                //}
                                eeg.Compute();
                                eeg.Render(canvas);
                            }

                            // VITAL INDICATOR
                            if (disp is VitalIndicator) {
                                VitalIndicator vital = (VitalIndicator)disp;
                                if (Math.Round(ticker, 0) % 9 == 0) {
                                    vital.SetData(rand.NextDouble());
                                }
                                if (vital.stepsRemaining > 0) {
                                    vital.Step();
                                    vital.Compute();
                                    vital.Render(canvas);
                                }
                            }

                            if (disp is Graph) {
                                Graph gr = (Graph)disp;
                                //GenerateLabels();
                                gr.Compute();
                                GenerateLabels();

                                /*
                                if (Math.Round(ticker, 0) % 9 == 0) {
                                    //int toDelete = rand.Next(0, gr.vertices.Count - 1);
                                    //gr.Remove(gr.vertices.Values.ToList()[toDelete]); 
                                        
                                    Vertex newVertex = new Vertex();
                                    newVertex.style = Styles.Green_VertexStyle;
                                    newVertex.type = 5;
                                    newVertex.label = "" + Math.Round(ticker, 0);
                                    newVertex.coordinates = ToWPFCoords(new Point(rand.Next(-50, 50), rand.Next(-50, 50)), (float)canvas.ActualWidth, (float)canvas.ActualHeight);
                                    //newVertex.enabled = rand.Next(0, 100);
                                    gr.forceConstant += rand.Next(-3, 3);
                                    gr.Add(newVertex);
                                    gr.CreateUnidirectionalEdge(newVertex, gr.GetRandom(), DefaultEdgeStyleTemplate);

                                    GenerateLabel(newVertex);
                                }
                                * */

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
                                //Cached.arrowPercentage += 0.01f;
                                //if (Cached.arrowPercentage >= 0.90f) { Cached.arrowPercentage = 0.10f; }
                                RenderGraph(gr, (float)Cached.lastOffset.X, (float)Cached.lastOffset.Y);

                            }

                            if (disp is Histogram) {
                                Histogram hist = (Histogram)disp;
                                if (hist.stepsRemaining > 0) {
                                    hist.Step();
                                    hist.Compute();
                                    hist.Render(canvas);
                                }
                                if (hist.stepsRemaining == 0) {
                                    List<float> newSeries = new List<float>();
                                    for (int i = 0; i < hist.pointCount; i++) { newSeries.Add(rand.Next(0, 100)); }
                                    hist.SetData(newSeries.ToArray());
                                }
                            }


                            if (disp is LinePlot) {
                                LinePlot hist = (LinePlot)disp;
                                if (hist.stepsRemaining > 0) {
                                    hist.Step();
                                    hist.Compute();
                                    hist.Render(canvas);
                                }
                                if (hist.stepsRemaining == 0) {
                                    List<float> newSeries = new List<float>();
                                    for (int i = 0; i < hist.pointCount; i++) { newSeries.Add(rand.Next(0, 100)); }
                                    hist.SetData(newSeries.ToArray());
                                }
                            }
                        }
                    }
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
                    canvas.Dispatcher.Invoke(threadBridge, dummy);
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
        public static Point ToCenteredCoords(Point c, float canvasWidth, float canvasHeight) { return new Point((float)(c.X - (canvasWidth / 2.0)), (float)((canvasHeight / 2.0) - c.Y)); }
        #endregion

        #region Layout Computation
        public void ComputeTypedRadialGraphLayout(Graph n, float initialX, float initialY, float outputWidth, float outputHeight, bool bDensity = true) {
            GenerateLabels();
            Point location = new Point(0,0);
            if (n.Root != null) {
                if (bDensity) NewDensitySurvey(n, new Point(initialX, initialY), outputWidth, outputHeight);
                if (n.topLevelVertices.Any()) {
                    for (int i = 0; i < n.topLevelVertices.Count(); i++) {

                        if (i == 0) { // Make sure to draw the first TopLevel unit far enough from the left so that it doesn't go off the screen
                            initialX = Math.Abs(n.topLevelVertices[i].minrelX) + minLeftPadding;
                            initialY = (float)(outputHeight * (4.0 / 10.0));
                            location = ToCenteredCoords(new Point(initialX, initialY), (float)outputWidth, (float)outputHeight); // Unfortunate, redundant, but ultimately necessary coordinate conversion
                        }
                        ComputeGraphClusterSize(n, n.topLevelVertices[i], location, 0.0f, 0.0f, (float)outputWidth, (float)outputHeight); // Compute for current topLevel unit
                        // Compute position for next topLevel unit
                        if (n.topLevelVertices.Count() > i + 1) {
                            location.X = location.X + Math.Abs(n.topLevelVertices[i].maxrelX) + Math.Abs(n.topLevelVertices[i + 1].minrelX);
                        }
                    }
                }
            }
        }

        public void GenerateLabels() {
            //labels.Clear();
            #region Compute Label size (must be done before the density survey)
            foreach (Vertex v in graph.vertices.Values) {
                // Select style based on rules
                GenerateLabel(v);
            }
            #endregion
        }

        public void GenerateLabel(Vertex v) {
            VertexStyleTemplate style = null;
            if (layout.VertexStyleRules.ContainsKey(v.type)) { style = layout.VertexStyleRules[v.type]; }
            v.style = style == null ? v.style : style;
            // We pre-render this so the spacing algorithm knows how much space to allocate for this vertex
            int fSize = (int)Math.Ceiling((Cached.nodeRadiusFudge) * 14);
            if (v.labelBlock == null) {
                Point p = v.transCoords.X == 0.0 && v.transCoords.Y == 0.0 ? v.coordinates : v.transCoords;

                Typeface typeface = new Typeface(v.style.fontFamily, FontStyles.Normal, v.style.vertexFontWeight, FontStretches.Normal); // FIXME possibly cache various sized typefaces?
                v.labelBlock = Primitives.GenerateTextBlock(v.label, typeface, fSize, v.style.vertexTextColor, v.style.vertexColor, p.X, p.Y, true, v.tooltip);
                v.labelBlock.Background = Cached.BrushTransparent;
                labels.Add(v.labelBlock);

                if (bSizeVertexToText) {
                    if (bUseStandardTypeface && standardTypeface != null) {
                        v.labelBlockSize = Primitives.MeasureString(v.labelBlock, standardTypeface);
                    }
                    else {
                        typeface = new Typeface(v.labelBlock.FontFamily, v.labelBlock.FontStyle, v.labelBlock.FontWeight, v.labelBlock.FontStretch);
                        v.labelBlockSize = Primitives.MeasureString(v.labelBlock, typeface);
                    }
                }
            }
        }

        /// <summary>
        /// Pre-computes required space for clusters before the main layout computation is undertaken
        /// </summary>
        public void NewDensitySurvey(Graph n, Point coords, float outputWidth, float outputHeight) {
            foreach (KeyValuePair<string, Vertex> pair in n.vertices) {
                Vertex v = (Vertex)pair.Value;
                if (v.bTopLevel) { ComputeGraphClusterSize(n, v, coords, 0, 0, outputWidth, outputHeight); }
            }
        }

        /// <summary>
        /// Compute positions (layout) of elements on the screen
        /// </summary>
        public void ComputeGraphClusterSize(Graph n, Vertex v, Point coords, float parentTheta, float parentRadius, float outputWidth, float outputHeight, int depth = 0) {

            #region Assign initial coordinates
            v.box = null;
            v.coordinates = ToWPFCoords(coords, outputWidth, outputHeight);
            v.transCoords = new Point(0, 0);
            #endregion

            #region Assign default min/max based on coordinates and label size
            // Assign default min/max
            v.minx = (float)(v.coordinates.X);
            v.miny = (float)(v.coordinates.Y);
            v.maxx = (float)(v.coordinates.X);
            v.maxy = (float)(v.coordinates.Y);
            // Account for label sizes
            v.minx -= (float)(v.labelBlockSize.Width * (1.0 / 2.0));
            v.miny -= (float)(v.labelBlockSize.Height * (1.0 / 2.0));
            v.maxx += (float)(v.labelBlockSize.Width * (1.0 / 2.0));
            v.maxy += (float)(v.labelBlockSize.Height * (1.0 / 2.0));
            // Account for border thicknesses
            v.minx -= (float)Cached.borderThickness + 1;
            v.miny -= (float)Cached.borderThickness + 1;
            v.maxx += (float)Cached.borderThickness + 1;
            v.maxy += (float)Cached.borderThickness + 1;
            #endregion

            #region Compile angle constraints (AC) and constraint overrides (CO)
            var ChildWithRules = new Dictionary<Vertex, AngleConstraint>();
            foreach (Vertex child in v.children) {
                var overrideRules = new List<AngleConstraintOverride>();
                if (child.children.Count() == 1) {
                    overrideRules = layout.VertextAngleOverrideRules.Where(q => q.overridingType == child.children[0].type && q.overriddenType == child.type).ToList();
                    if (overrideRules.Any()) { // Constraint-override
                        ChildWithRules.Add(child, overrideRules[0].masterAngleConstraint);
                    }
                }
                if (!overrideRules.Any()) { // No constraint-override. Use regular constraint
                    List<AngleConstraint> angleRulesNonSpecific = layout.RadialExpansionRules.Where(q => q.toType == child.type && q.fromType == -1).ToList();
                    List<AngleConstraint> angleRulesSpecific = layout.RadialExpansionRules.Where(q => q.toType == child.type && q.fromType == v.type).ToList();
                    if (angleRulesSpecific.Any()) {
                        if (!ChildWithRules.ContainsKey(child)) ChildWithRules.Add(child, angleRulesSpecific[0]);
                    }
                    else if (angleRulesNonSpecific.Any()) {
                        if (!ChildWithRules.ContainsKey(child)) ChildWithRules.Add(child, angleRulesNonSpecific[0]);
                    }
                    else { // No constraints whatsoever
                        // Fabricate AC/Vertex pair for those vertices where no appropriate AC was found (use parent's theta)
                        if (!ChildWithRules.ContainsKey(child)) ChildWithRules.Add(child, new AngleConstraint("parent", v.type, child.type, parentTheta, parentTheta));
                    }
                }
            }
            #endregion

            #region Compute locations
            // Iterate through all constraints
            foreach (AngleConstraint ac in ChildWithRules.Values.ToList()) { // Cycle through ACs
                List<Vertex> children = ChildWithRules.Where(q => q.Value == ac).Select(s => s.Key).ToList(); // Get all children for this AC
                for (int i = 0; i < children.Count; i++) {
                    if (children[i].bIsolated) return; // Don't process any portion of the tree below an isolated node

                    #region Vars
                    // Angle offset calculation
                    float childCountOffset = parentTheta == 0.0 ? 0 : 1;
                    float limitedThetaInc = (float)(ac.maxAngle - ac.minAngle) / Math.Max(2, (children.Count + childCountOffset));
                    float angle = (float)Math.Abs(ac.minAngle + (limitedThetaInc * (i + 1)));
                    float childMult = children.Count == 1 ? 1 : children.Count + 1;
                    float fraction = (ac.maxAngle - ac.minAngle) != 0.0 ? (float)(ac.maxAngle - ac.minAngle) : 1.0f;
                    float Rn = 0.0f;
                    float r = 0.0f;
                    float pi = (float)Math.PI;
                    float rSpace = 10.0f;
                    float child_offset = 0.0f;
                    #endregion

                    #region Compute quantities and angles
                    // Get child space reqs
                    float maxChildHeight = (float)children.Select(q => q.labelBlockSize.Height).Max();
                    float maxChildWidth = (float)children.Select(q => q.labelBlockSize.Width).Max();
                    maxChildHeight = Math.Max(children[i].reqHeight, maxChildHeight);
                    maxChildWidth = Math.Max(children[i].reqWidth, maxChildWidth);

                    // DISPLACEMENT (based on child's required space)
                    if (children[i].reqHeight > 0.0) {
                        if ((ac.minAngle >= (1.0 / 4.0) * pi && ac.maxAngle <= (3.0 / 4.0) * pi)
                            || (ac.minAngle >= (5.0 / 4.0) * pi && ac.maxAngle <= (7.0 / 4.0) * pi)) {
                            child_offset = children[i].reqHeight;
                            child_offset *= layout.radiusReqFudge;
                        }
                    }
                    else {
                        child_offset = children[i].reqWidth;
                        child_offset *= layout.radiusReqFudge;
                    }

                    // If a leaf, minimize final radius
                    if (!children[i].children.Any() && v.children.Count == 1) {
                        if (bCompressLeaves) {
                            r = (float)(Math.Max(maxChildHeight, maxChildWidth) + v.children[i].style.haloThickness);
                        }
                    }
                    else { // Otherwise, perform normal calulation
                        // Compute radius
                        Rn = child_offset != 0.0 ? child_offset : (float)(Math.Sqrt(Math.Pow(maxChildHeight, 2) * 1.2 + Math.Pow(maxChildWidth, 2)) + (v.children[i].style.haloThickness * 2));
                        r = ((Rn + vSpace) * childMult) / fraction;
                        r += parentRadius;
                    }
                    r *= Cached.edgeLengthFudge;

                    // Compute (X,Y)
                    float px = (float)(coords.X - ((r + rSpace) * Math.Cos(angle + pi)));  // X-coordinate for child
                    float py = (float)(coords.Y - ((r + rSpace) * Math.Sin(angle + pi)));  // Y-coordinate for child
                    #endregion

                    // RECURSE
                    float thisRadius = (float)((v.labelBlockSize.Width + v.labelBlockSize.Height) / 2.0);
                    if (!layout.IgnoreVertexList.Contains(children[i].type)) { ComputeGraphClusterSize(n, children[i], new Point(px, py), angle, thisRadius, outputWidth, outputHeight, depth + 1); }

                    // Inherit min/max from child
                    if (!children[i].bTopLevel && children[i].type != 6) {
                        v.minx = (float)(v.minx < children[i].minx ? v.minx : children[i].minx);
                        v.miny = (float)(v.miny < children[i].miny ? v.miny : children[i].miny);
                        v.maxx = (float)(v.maxx > children[i].maxx ? v.maxx : children[i].maxx);
                        v.maxy = (float)(v.maxy > children[i].maxy ? v.maxy : children[i].maxy);
                    }
                }
            }
            #endregion

            #region Compute mins/maxes
            if ((v.type == 5 || v.type == 6) && v.children.Any() /*&& v.bTopLevel*/) {
                // Compute relative min/max
                v.minrelX = (float)(v.minx - v.coordinates.X);
                v.minrelY = (float)(v.miny - v.coordinates.Y);
                v.maxrelX = (float)(v.maxx - v.coordinates.X);
                v.maxrelY = (float)(v.maxy - v.coordinates.Y);
                // Compute required widths/heights
                v.reqWidth = Math.Abs(v.maxx - v.minx);
                v.reqHeight = Math.Abs(v.maxy - v.miny);

                // Generate density region
                if (bShowDensityRegions) {
                    float x = (float)(v.coordinates.X + (1.0 / 2.0) * Math.Abs(v.minrelX - v.maxrelX));
                    float y = (float)(v.coordinates.Y + (1.0 / 2.0) * Math.Abs(v.minrelY - v.maxrelY));
                    float w = Math.Abs(v.minrelX - v.maxrelX);
                    float h = Math.Abs(v.minrelY - v.maxrelY);
                    v.reqRegion = Primitives.FastRectangle(x + v.minrelX, y + v.minrelY, w, h, Cached.BrushLimeGreen, 0.25, false, "");
                }
            }
            #endregion
        }
        #endregion
    }
}
