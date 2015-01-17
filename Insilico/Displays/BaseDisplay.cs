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
    public abstract class BaseDisplay {
        public int zOrder = 0;
        public string handle;

        public Rectangle border;
        public Rectangle background;
        public HashSet<UIElement> elements = new HashSet<UIElement>();

        public float[] oData; // Original data
        public float[] nData; // New data
        public float[] dData; // Difference between original and new (for smooth stepping)
        public int xo;
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

        protected int interiorWidth;
        protected int interiorHeight;
        protected int requiredHorizonalMargin;
        protected int requiredVerticalMargin;

        public void Deactivate() { bActive = false; }
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
        public bool IsActive { get { return bActive; } }

        public abstract void ComputeMetrics();          // Determine the value of important parameters
        public abstract void ComputeActiveElements();   // Assemble our initial set of data-driven UIElements
        public abstract void ComputeDecorations();      // Mean-lines, labels, backgrounds, etc
        public abstract void Compute();

        // Generates borders and backgrounds
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

        public void Render(Canvas c) {
            foreach (UIElement e in this.elements) {
                if (!c.Children.Contains(e)) c.Children.Add(e);
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

        public float stepCount = 35;
        public float stepsRemaining = 0; // Make accessor
        protected float stepSize;

        public int leftMargin = 0;
        public int rightMargin = 0;
        public int topMargin = 0;
        public int bottomMargin = 0;

        public float widthConsumed;
        public float heightConsumed;
        public float widthRemaining;
        public float heightRemaining;
    }
}
