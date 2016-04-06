using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using James.Properties;
using James.Workflows;
using MahApps.Metro;

namespace James.WorkflowEditor
{
    /// <summary>
    ///     Calculates the curved Path from the source- to the destinationComponent
    ///     Inheritance not possible because Path is sealed.
    /// </summary>
    public class CustomPath
    {
        public readonly Point SourcePoint;

        public CustomPath(WorkflowComponent sourceComponent, Point sourcePoint)
        {
            SourcePoint = sourcePoint;
            CreatePath();
            Source = sourceComponent;
        }

        public CustomPath(WorkflowComponent sourceComponent, Point sourcePoint, Point destionationPoint)
        {
            SourcePoint = sourcePoint;
            CreatePath();
            Source = sourceComponent;
            CalcPath(destionationPoint);
            this.Path.MouseEnter += (sender, args) =>
            {
                Path.Stroke = (Brush)ThemeManager.GetResourceFromAppStyle(null, "AccentColorBrush");
                Canvas.SetZIndex(Path, -1);
            };
            this.Path.MouseLeave += (sender, args) =>
            {
                Path.Stroke = Brushes.White;
                Canvas.SetZIndex(Path, -2);
            };
        }

        public WorkflowComponent Source { get; set; }
        public WorkflowComponent Destination { get; set; }
        public Path Path { get; set; }

        /// <summary>
        /// Recalculates the path
        /// </summary>
        private void CreatePath()
        {
            Path = new Path
            {
                Stroke = Brushes.White,
                StrokeThickness = 2,
                ToolTip = Resources.CustomLine_RightClickToRemoveLine,
                StrokeStartLineCap = PenLineCap.Square,
                StrokeEndLineCap = PenLineCap.Square
            };
        }

        /// <summary>
        ///     Calculates the curved Path to the destination
        /// </summary>
        /// <param name="destination"></param>
        public void CalcPath(Point destination)
        {
            float magic = 8;
            var segments = new List<PathSegment>();
            if (destination.X > SourcePoint.X)
            {
                Point c1 = new Point(SourcePoint.X + (magic - 1) * (destination.X - SourcePoint.X) / magic, SourcePoint.Y +(destination.Y - SourcePoint.Y) / magic);
                Point c2 = new Point(SourcePoint.X + (destination.X - SourcePoint.X) / magic, SourcePoint.Y + (magic - 1) * (destination.Y - SourcePoint.Y) / magic);
                segments.Add(new BezierSegment(c1, c2, destination, true));
            }
            else
            {
                double distanceX = Math.Abs(SourcePoint.X - destination.X)/ 3;
                Point c1 = new Point(SourcePoint.X + Math.Min((magic-1) * (SourcePoint.X - destination.X) / magic, distanceX) + 20, SourcePoint.Y + (magic - 1)*(destination.Y- SourcePoint.Y)/magic);
                Point c2 = new Point(destination.X - Math.Min((magic - 1) * (SourcePoint.X - destination.X) / magic, distanceX) - 20, destination.Y - (magic - 1) * (destination.Y - SourcePoint.Y) / magic);
                segments.Add(new BezierSegment(c1, c2, destination, true));
            }

            var geo = new PathGeometry();
            geo.Figures.Add(new PathFigure(SourcePoint, segments, false) {StartPoint = SourcePoint});
            Path.Data = geo;
            Canvas.SetZIndex(Path, -2);
        }

        /// <summary>
        ///     Draws a straight line to the destination
        /// </summary>
        /// <param name="destination"></param>
        public void ChangeDestination(Point destination)
        {
            //Shorten vector by 2 px in length:
            ShortenLengthOfVector(SourcePoint, ref destination, 2);

            var segments = new List<PathSegment>();
            var currPos = new Point(SourcePoint.X, SourcePoint.Y);
            segments.Add(new LineSegment(currPos, true));
            segments.Add(new LineSegment(destination, true));
            var geo = new PathGeometry();
            geo.Figures.Add(new PathFigure(SourcePoint, segments, false));
            Path.Data = geo;
        }

        /// <summary>
        /// Shortens the distance of the vector from source to destination by a amount of lengthToShorten pixels.
        /// </summary>
        /// <param name="source">The starting point of the vector</param>
        /// <param name="destination">The end point of the vector</param>
        /// <param name="lengthToShorten">distance in px to shorten</param>
        private void ShortenLengthOfVector(Point source, ref Point destination, int lengthToShorten)
        {
            double deltaX = destination.X - source.X;
            double deltaY = destination.Y - source.Y;
            double length = Math.Sqrt(deltaX * deltaX + deltaY * deltaY); //pythagoras
            destination.Y = deltaY * (length - lengthToShorten) / length + source.Y;
            destination.X = deltaX * (length - lengthToShorten) / length + source.X;
        }

        /// <summary>
        ///     Deletes the connection from the components
        /// </summary>
        public void DeleteConnection()
        {
            var runnableWorkflowComponent = Source;
            runnableWorkflowComponent?.ConnectedTo.Remove(Destination.Id);
        }
    }
}