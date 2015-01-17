using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Insilico {
    public class ValueColorScheme {
        public List<Tuple<float, float, SolidColorBrush>> regions = new List<Tuple<float, float, SolidColorBrush>>();
        public SolidColorBrush GetColor(float value) { // Possibly very slow, FIXME
            List<SolidColorBrush> possibilities = regions.Where(q => value >= q.Item1 && value < q.Item2).Select(q => q.Item3).ToList();
            if (possibilities.Any()) {
                return possibilities.First();
            }
            return null;
        }
    }

    public static partial class Scheme {

        public static Tuple<float, float, SolidColorBrush> lowGreen = new Tuple<float, float, SolidColorBrush>(0.0f, 0.65f, Cached.BrushLimeGreen);
        public static Tuple<float, float, SolidColorBrush> medYellow = new Tuple<float, float, SolidColorBrush>(0.65f, 0.85f, Cached.BrushYellow);
        public static Tuple<float, float, SolidColorBrush> highRed = new Tuple<float, float, SolidColorBrush>(0.85f, 1.0f, Cached.BrushRed);

        public static ValueColorScheme SimpleRYG = new ValueColorScheme() { regions = { lowGreen, medYellow, highRed } };
    }
}


