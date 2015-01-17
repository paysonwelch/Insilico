using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Insilico {
    // Wrapper for an element
    public class BoundingBox {
        public UIElement element;
        public Vertex vertex;
        public BoundingBox(UIElement e, Vertex v) { element = e; vertex = v; }
    }

    // Generic vertex object
    public class Vertex {
        public BaseDisplay attachedDisplay;
        public float dxoff;
        public float dyoff;

        public int type;
        public bool bTopLevel;
        public string label;
        public string tooltip = "";
        public UIElement box;
        public List<Vertex> children = new List<Vertex>();
        public Point coordinates = new Point(50, 50);
        public Point transCoords = new Point(0, 0);
        public VertexStyleTemplate style; // Color, size, opacity, etc 
        public VertexAnimationTemplate animation;
        public ConcurrentDictionary<Edge, Edge> outgoingEdges = new ConcurrentDictionary<Edge, Edge>();
        public ConcurrentDictionary<Edge, Edge> incomingEdges = new ConcurrentDictionary<Edge, Edge>();
        public bool bIsolated;
        public Rectangle reqRegion;
        public TextBlock coordLabel;

        public override string ToString() { return label + " (" + type + ")"; }

        #region Survey
        // Pre-rendered objects
        // Not displayed until final rendering step, but whose metrics are used for spacing computation
        public TextBlock labelBlock;
        public Size labelBlockSize = new Size();

        public float reqHeight;
        public float reqWidth;

        public float minrelX;
        public float minrelY;
        public float maxrelX;
        public float maxrelY;

        public float minx = float.MaxValue;
        public float miny = float.MaxValue;
        public float maxx = 0;
        public float maxy = 0;
        #endregion

        #region Force
        public float charge = 100;
        public float mass = 100;
        public List<Vertex> neighbors = new List<Vertex>();
        public bool adding = true;
        #endregion
    }
}
