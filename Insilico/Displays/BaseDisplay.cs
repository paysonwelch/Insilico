using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {

    /// <summary>Base class for all display types</summary>
    public abstract class BaseDisplay {

        public int zOrder = 0;
        public string handle;
        public bool bResizeToFitCanvas = true;
        public bool bDrawWithinCanvas = true;

        protected Canvas targetCanvas;
        public Canvas TargetCanvas {
            get {
                return targetCanvas;
            }
            set {
                targetCanvas = value;
                // Size plot to match canvas size
                if (bResizeToFitCanvas) {
                    targetCanvas.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    targetCanvas.Arrange(new Rect(0, 0, targetCanvas.DesiredSize.Width, targetCanvas.DesiredSize.Height));
                    height = (int)targetCanvas.ActualHeight;
                    width = (int)targetCanvas.ActualWidth;
                    // Position plot to match canvas
                    Thickness newMargin = targetCanvas.Margin;
                    //xo = (int)newMargin.Left;
                    //yo = (int)newMargin.Top;
                }
                // Only draw within the specified canvas
                if (bDrawWithinCanvas) {
                    targetCanvas.ClipToBounds = true;
                }
            }
        }

        public Rectangle border;
        public Rectangle background;

        /// <summary> All UIElements which make up this display </summary>
        public HashSet<UIElement> elements = new HashSet<UIElement>();
        /// <summary> Original data in display </summary>
        public float[] oData; 
        /// <summary>Newest data (what we will step to)</summary>
        public float[] nData; 
        /// <summary>Difference between original and new data (for smooth stepping)</summary>
        public float[] dData;
        /// <summary>X-offset for display</summary>
        public int xo;
        /// <summary>Y-offset for display</summary>
        public int yo;

        private bool bActive = true;

        protected DisplayLayout displayLayout = Layouts.RadioactiveChocolate;
        public DisplayLayout Layout {
            get {
                return displayLayout;
            }
            set {
                displayLayout = value;
            }
        }

        /// <summary>Width remaining inside the display after all margin padding, borders, and required spacing is accounted for</summary>
        protected int interiorWidth;
        /// <summary>Height remaining inside the display after all margin padding, borders, and required spacing is accounted for</summary>
        protected int interiorHeight;

        protected int requiredHorizonalMargin;
        protected int requiredVerticalMargin;

        /// <summary>Prevents the engine from rendering any changes to this display</summary>
        public void Deactivate() { bActive = false; }

        /// <summary>Tells the engine that we wish to see real-time changes to this display. Also calculates various initial parameters.</summary>
        public void Activate() {
            stepSize = (1.0f / (float)stepCount);
            requiredHorizonalMargin = this.displayLayout.borderThickness * 4 + this.displayLayout.interiorPadding * 4;
            requiredVerticalMargin = this.displayLayout.borderThickness * 4 + this.displayLayout.interiorPadding * 4;
            interiorWidth = width - requiredHorizonalMargin;
            interiorHeight = height - requiredVerticalMargin;
            ComputeMetrics();
            ComputeActiveElements();
            ComputeDecorations();
            ComputeDisplayContainer();
            bActive = true; 
        }

        /// <summary>Returns whether this display is currently active</summary>
        public bool IsActive { get { return bActive; } }

        /// <summary>Determine the value of important parameters</summary>
        public abstract void ComputeMetrics();          // 

        /// <summary>Assemble our initial set of data-driven UIElements</summary>
        public abstract void ComputeActiveElements();

        /// <summary>Restarts the thread</summary>
        public abstract void ComputeDecorations();

        /// <summary>Mean-lines, labels, backgrounds, etc</summary>
        public abstract void Compute();

        /// <summary>Generates borders and backgrounds for the display</summary>
        public void ComputeDisplayContainer() {
            if (this.displayLayout.bShowBackground) {
                background = Primitives.CreateRectangle(xo, yo, width, height, this.displayLayout.backgroundColor);
                background.Opacity = 0.2;
                elements.Add(background);
                Canvas.SetZIndex(background, zOrder);
            }
            if (this.displayLayout.bShowBorder) {
                border = Primitives.CreateBorder(xo, yo, width, height, this.displayLayout.backgroundColor);
                border.StrokeThickness = this.displayLayout.borderThickness;
                border.Stroke = this.displayLayout.borderColor;
                border.Fill = Cached.BrushTransparent;
                elements.Add(border);
            }
        }

        /// <summary>Adds all UIElements to the specified Canvas object if they aren't already present</summary>
        /// <param name="c"></param>
        public void Render(Canvas c) {
            if (c != null) {
                foreach (UIElement e in this.elements) {
                    if (!c.Children.Contains(e)) c.Children.Add(e);
                }
            }
        }

        #region Accessors
        protected int width = 100;
        protected int height = 100;

        public int Width {
            get { return width; }
            set {
                width = value;
            }
        }
        public int Height {
            get { return height; }
            set {
                height = value;
            }
        }
        #endregion

        /// <summary>Number of animation steps per value change</summary>
        public float stepCount = 35;
        /// <summary>How many steps are remaining in the current value change</summary>
        public float stepsRemaining = 0; // Make accessor
        /// <summary>A value calculated from the range and stepCount</summary>
        protected float stepSize;

        public float widthConsumed;
        public float heightConsumed;
        public float widthRemaining;
        public float heightRemaining;
    }
}
