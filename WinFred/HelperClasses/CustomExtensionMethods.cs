using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using James.Workflows;

namespace James.HelperClasses
{
    public static class CustomExtensionMethods
    {
        /// <summary>
        /// Custom Extension Methods for providing a possibilty to convert an Icon to ImageSource
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static ImageSource ToImageSource(this Icon icon)
        {
            var tmp = DateTime.Now;
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            Console.WriteLine((DateTime.Now - tmp).TotalMilliseconds);
            return imageSource;
        }

        /// <summary>
        /// Custom Extension Method for creating a LINQ style ForEach on a IEnumerable
        /// because a .ToList() generates a copy of the list before (performance)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration"></param>
        /// <param name="action"></param>
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        /// <summary>
        /// Returns the next component using the id
        /// </summary>
        /// <param name="workflowComponent">actual component</param>
        /// <param name="id">identifies the connection</param>
        /// <returns>next component</returns>
        public static WorkflowComponent GetNext(this WorkflowComponent workflowComponent, int id)
        {
            return GetNext(workflowComponent.ParentWorkflow, id);
        }

        /// <summary>
        /// Returns the next component using the id
        /// </summary>
        /// <param name="parent">workflow</param>
        /// <param name="id">identifies the connection</param>
        /// <returns>next component</returns>
        public static WorkflowComponent GetNext(this Workflow parent, int id)
        {
            return parent.Components.First(component => component.Id == id);
        }
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            return items.TakeWhile(item => !predicate(item)).Count();
        }
    }
}
