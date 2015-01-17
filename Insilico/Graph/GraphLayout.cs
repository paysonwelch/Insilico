using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Insilico {
    public enum GraphLayoutType { RadialExpansion, LinearSpacing };

    public class GraphLayout {
        public GraphLayoutType layoutType;
        public List<AngleConstraint> RadialExpansionRules = new List<AngleConstraint>();                                // Type  -> (minTheta, maxTheta)
        public Dictionary<int, VertexStyleTemplate> VertexStyleRules = new Dictionary<int, VertexStyleTemplate>();      // Type  -> VertexStyle
        public Dictionary<int, EdgeStyleTemplate> EdgeStyleRules = new Dictionary<int, EdgeStyleTemplate>();            // Type  -> EdgeStyle
        public List<AngleConstraintOverride> VertextAngleOverrideRules = new List<AngleConstraintOverride>();           // OverriderType/OverriddenType: (minTheta, maxTheta)
        public SolidColorBrush DefaultClusterConnectorBrush = Cached.BrushDarkGray;
        public List<int> IgnoreVertexList = new List<int>();                                                            // List of vertex types we don't want to plot
        public float radiusReqFudge = 1.5f;
    }

    // FIXME: Find a more logical place to keep these
    public class AngleConstraint {
        public string handle;
        public int fromType;
        public int toType;
        public float minAngle;
        public float maxAngle;
        public AngleConstraint(string handle, int fromType, int toType, float minAngle, float maxAngle) {
            this.handle = handle;
            this.fromType = fromType;
            this.toType = toType;
            this.minAngle = minAngle;
            this.maxAngle = maxAngle;
        }
        public override string ToString() {
            return handle + " " + toType + "      (" + Math.Round(minAngle, 2) + " -> " + Math.Round(maxAngle, 2) + ")";
        }
    }

    // FIXME: Find a more logical place to keep these
    public class AngleConstraintOverride {
        public string handle;
        public int overriddenType;
        public int overridingType;
        public AngleConstraint masterAngleConstraint;
        public AngleConstraintOverride(string handle, int overriddenType, int overridingType, AngleConstraint masterAngleConstraint) {
            this.handle = handle;
            this.overriddenType = overriddenType;
            this.overridingType = overridingType;
            this.masterAngleConstraint = masterAngleConstraint;
        }
        public override string ToString() {
            return handle + " " + overriddenType + "->" + overridingType + "      (" + Math.Round(masterAngleConstraint.minAngle, 2) + " -> " + Math.Round(masterAngleConstraint.maxAngle, 2) + ")";
        }
    }
}