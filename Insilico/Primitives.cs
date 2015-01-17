using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Insilico {
    public static class Primitives {

        public static Ellipse CreateEllipse(double x, double y, double width, double height, SolidColorBrush color, double opacity = 1, string tooltip = "") {
            Ellipse myEllipse = new Ellipse();
            myEllipse.Fill = color;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = color;
            myEllipse.Width = width;
            myEllipse.Height = height;
            myEllipse.Opacity = opacity;
            if (tooltip != "") myEllipse.ToolTip = tooltip;
            double desiredCenterX = x;
            double desiredCenterY = y;
            double left = desiredCenterX - (myEllipse.Width / 2);
            double top = desiredCenterY - (myEllipse.Height / 2);
            myEllipse.Margin = new Thickness(left, top, 0, 0);
            Canvas.SetZIndex(myEllipse, 5);
            return myEllipse;
        }

        // Creates a grid with ellipse and text encapsulated (text doesn't work)
        public static Grid CreateCombinedEllipse(TextBlock text, double x, double y, double width, double height, SolidColorBrush fillColor, SolidColorBrush borderColor, double opacity, string tooltip = "") {
            var grid = new Grid();
            var myEllipse = new Ellipse();
            myEllipse.Fill = fillColor;
            myEllipse.StrokeThickness = 2;
            myEllipse.Stroke = borderColor;
            myEllipse.Width = width;
            myEllipse.Height = height;
            //myEllipse.Opacity = opacity;
            if (tooltip != "") myEllipse.ToolTip = tooltip;
            double desiredCenterX = x;
            double desiredCenterY = y;
            double left = desiredCenterX - (myEllipse.Width / 2);
            double top = desiredCenterY - (myEllipse.Height / 2);
            myEllipse.Margin = new Thickness(left, top, 0, 0);
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;
            grid.Children.Add(text);
            grid.Children.Add(myEllipse);
            Canvas.SetZIndex(grid, 5);
            return grid;
        }

        // Creates a border containing a textbox
        public static Border CreateCombinedRectangle(TextBlock text, double x, double y, double width, double height, SolidColorBrush color, double opacity, double haloOpacity, SolidColorBrush borderColor, CornerRadius radius, Border cachedBorder, string tooltip = "") {
            // Attempt to use a cached Border before creating a new one
            Border border;
            if (cachedBorder == null) border = new Border();
            else border = cachedBorder;
            border.Background = color;
            border.BorderThickness = Cached.StandardBorderThickness;
            border.BorderBrush = borderColor;
            border.Width = width;
            border.Height = height;
            //border.Opacity = opacity;
            border.CornerRadius = radius;
            border.VerticalAlignment = VerticalAlignment.Center;
            border.HorizontalAlignment = HorizontalAlignment.Center;
            if (tooltip != "") border.ToolTip = tooltip;
            float left = (float)(x - (border.Width / 2));
            float top = (float)(y - (border.Height / 2));
            border.Margin = new Thickness(left, top, 0, 0);
            Canvas.SetZIndex(border, 5);
            return border;
        }

        public static Rectangle CreateRectangle(double x, double y, double width, double height, SolidColorBrush color, double opacity, bool bRoundedRectangleCorners = false, string tooltip = "") {
            Rectangle myRectangle = new Rectangle();
            myRectangle.Fill = color;
            myRectangle.StrokeThickness = 0;
            myRectangle.Stroke = color;
            myRectangle.Width = width;
            myRectangle.Height = height;
            myRectangle.Opacity = opacity;
            if (tooltip != "") myRectangle.ToolTip = tooltip;

            if (bRoundedRectangleCorners) {
                myRectangle.RadiusX = 7;
                myRectangle.RadiusY = 7;
            }
            //double desiredCenterX = x;
            //double desiredCenterY = y;
            //double left = desiredCenterX - (myRectangle.Width / 2);
            //double top = desiredCenterY - (myRectangle.Height / 2);
            myRectangle.Margin = new Thickness(x, y-height, 0, 0);
            /*            
            double desiredCenterX = x;
            double desiredCenterY = y;
            double left = desiredCenterX - (myRectangle.Width / 2);
            double top = desiredCenterY - (myRectangle.Height / 2);
            myRectangle.Margin = new Thickness(left, top, 0, 0);
             */
            Canvas.SetZIndex(myRectangle, 5);
            return myRectangle;
        }

        public static Rectangle CreateRectangle(double x, double y, double width, double height, SolidColorBrush color) {
            Rectangle rect = new Rectangle();
            rect.Width = width;
            rect.Height = height;
            rect.Fill = color;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }

        public static Rectangle CreateBorder(double x, double y, double width, double height, SolidColorBrush color) {
            Rectangle rect = new Rectangle();
            rect.Width = width;
            rect.Height = height;
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            return rect;
        }

        public static TextBlock CreateTextBlock(string msg, Typeface typeface, int sz, SolidColorBrush foregroundColor, SolidColorBrush backgroundColor, double x, double y, bool centeredOnPoint = false, string tooltip = "") {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = msg;
            textBlock.FontFamily = typeface.FontFamily;
            textBlock.FontWeight = typeface.Weight;
            textBlock.FontSize = sz;
            textBlock.Foreground = foregroundColor;
            textBlock.Background = backgroundColor;
            if (tooltip != "") textBlock.ToolTip = tooltip;
            textBlock.HorizontalAlignment = HorizontalAlignment.Center;
            Canvas.SetLeft(textBlock, x);
            Canvas.SetTop(textBlock, y);
            Canvas.SetZIndex(textBlock, 5);
            if (centeredOnPoint) {
                Size mySize = Primitives.MeasureString(textBlock, typeface);
                Canvas.SetLeft(textBlock, x - mySize.Width / 2.0);
                Canvas.SetTop(textBlock, y - mySize.Height / 2.0);
            }
            return textBlock;
        }

        /// <summary>
        /// Measures the actual size of a rendered label taking into consideration the font size, family, etc
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="typeface"></param>
        /// <returns></returns>
        public static Size MeasureString(TextBlock tb, Typeface typeface) {
            var formattedText = new FormattedText(
                tb.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                typeface,
                tb.FontSize,
                Brushes.Black);
            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
