using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using James.Properties;
using James.Workflows;
using James.Workflows.Actions;
using James.Workflows.Outputs;
using James.Workflows.Triggers;
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
            var segments = new List<PathSegment>();
            var currPos = new Point(SourcePoint.X, SourcePoint.Y);
            var up = SourcePoint.Y > destination.Y;

            if (destination.X > SourcePoint.X)
            {
                var middle = new Point((SourcePoint.X + destination.X) / 2, (SourcePoint.Y + destination.Y) / 2);
                segments.Add(new BezierSegment(SourcePoint, new Point(middle.X, SourcePoint.Y), middle, true));
                segments.Add(new BezierSegment(middle, new Point(middle.X, destination.Y), destination, true));
            }
            else
            {
                double radius = 10;
                segments.Add(DrawCircleSector(ref currPos, radius, false, up, true));
                currPos.Y += (up ? -15 : +15);
                segments.Add(new LineSegment(currPos, true));
                segments.Add(DrawCircleSector(ref currPos, radius, true, up, false));
                currPos.X = destination.X;
                segments.Add(new LineSegment(currPos, true));
                up = currPos.Y > destination.Y;

                radius = Math.Min(radius, Math.Abs(currPos.Y - destination.Y)/2);
                segments.Add(DrawCircleSector(ref currPos, radius, true, up, true));
                if (radius > 9)
                {
                    currPos.Y = destination.Y + (up ? radius : -radius);
                    segments.Add(new LineSegment(currPos, true));
                }
                segments.Add(DrawCircleSector(ref currPos, radius, false, up, false));
            }

            var geo = new PathGeometry();
            geo.Figures.Add(new PathFigure(SourcePoint, segments, false));
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
        ///     Generates the beautiful curved Path from one position to the other
        /// </summary>
        /// <param name="start">Startpoint</param>
        /// <param name="radius"></param>
        /// <param name="left"></param>
        /// <param name="up"></param>
        /// <param name="rightCurved">Should it be curved right?</param>
        /// <returns></returns>
        private BezierSegment DrawCircleSector(ref Point start, double radius, bool left, bool up, bool rightCurved)
        {
            var newX = start.X + ((left) ? -radius : radius);
            var newY = start.Y + ((up) ? -radius : radius);
            var destination = new Point(newX, newY);
            Point middle;
            if (rightCurved)
            {
                middle = new Point(destination.X, start.Y);
            }
            else
            {
                middle = new Point(start.X, destination.Y);
            }

            var segment = new BezierSegment(start, middle, destination, true);
            start = destination;
            return segment;
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