using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using James.Workflows;
using MahApps.Metro.Controls;
using System.Windows.Controls;
using Task = System.Threading.Tasks.Task;

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
            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                icon.Handle,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            return imageSource;
        }

        /// <summary>
        /// Generates the output string for a given FormatString and arguments
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static string InsertArguments(this string format, string[] arguments)
        {
            string output = format;
            for (int i = 0; i < arguments.Length; i++)
            {
                output = output.Replace("{" + i + "}", arguments[i]);
            }
            output = output.Replace("{#}", arguments.Length.ToString());
            output = output.Replace("{...}", string.Join(" ", arguments));
            return Regex.Replace(output, @"{[0-9]+}", string.Empty);
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
            return parent.Components.FirstOrDefault(component => component.Id == id);//todo check , delete openoutput with one single income
        }

        /// <summary>
        /// Returns the index of the element in the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static int FindIndex<T>(this IEnumerable<T> items, Predicate<T> predicate)
        {
            return items.TakeWhile(item => !predicate(item)).Count();
        }

        /// <summary>
        /// Converts MahApps.Metro.Controls.HotKey to GlobalHotKey.HotKey for the GlobalHotKey api
        /// </summary>
        /// <param name="hotkey"></param>
        /// <returns></returns>
        public static GlobalHotKey.HotKey ToGlobalHotKey(this HotKey hotkey)
        {
            return new GlobalHotKey.HotKey(hotkey.Key, hotkey.ModifierKeys);
        }

        public static MetroWindow GetWindow(this UserControl uc)
        {
            return (MetroWindow)Window.GetWindow(uc);
        }

        /// <summary>
        /// Used for an intuitive way of caching fileIcon inline
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string AndCacheFileIcon(this string path)
        {
            Task.Run(() => FileIconCache.Instance.GetFileIcon(path));
            return path;
        }
    }
}
