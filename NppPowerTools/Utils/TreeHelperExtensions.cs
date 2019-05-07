﻿using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace System.Windows
{
    public static class TreeHelperExtensions
    {
        public static FrameworkElement GetTopFrameworkElement(this FrameworkElement element)
        {
            return !(element.Parent is FrameworkElement) ? element : (element.Parent as FrameworkElement).GetTopFrameworkElement();
        }

        public static object GetDataContext(this object element)
        {
            if (element is FrameworkElement fe)
                return fe.DataContext;
            else if (element is FrameworkContentElement fce)
                return fce.DataContext;

            return null;
        }

        /// <summary>
        /// To find a parent (Ancestor) of the specified type in the visual tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <typeparam name="T">The type of the parent we search for</typeparam>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static T FindVisualParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            // get parent item
            var parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        /// <summary>
        /// To find a parent (Ancestor) of the specified type in the logical tree
        /// Begin from the current child and go up until it find the parent or reach the root
        /// </summary>
        /// <typeparam name="T">The type of the parent we search for</typeparam>
        /// <param name="child">A DependencyObject child from where to start the search</param>
        /// <returns>The first found parent of the specified type, or null if none are found</returns>
        public static T FindLogicalParent<T>(this DependencyObject child)
            where T : DependencyObject
        {
            // get parent item
            var parentObject = LogicalTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        /// <summary>
        /// Get an enumerable of all sub elements in logical tree of the current element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<object> LogicalTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            foreach (var child in LogicalTreeHelper.GetChildren(node).OfType<DependencyObject>()
                .SelectMany(depObj => depObj.LogicalTreeDepthFirstTraversal()))
                yield return child;
        }

        /// <summary>
        /// Get an enumerable of all sub elements in visual tree of the current element
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static IEnumerable<object> VisualTreeDepthFirstTraversal(this DependencyObject node)
        {
            if (node == null) yield break;
            yield return node;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(node); i++)
            {
                var child = VisualTreeHelper.GetChild(node, i);
                foreach (var d in child.VisualTreeDepthFirstTraversal())
                {
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Return an enumerable of the visual ancestory of the current DependencyObject (including the starting point).
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> VisualTreeAncestory(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException("dependencyObject");

            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }
        }

        /// <summary>
        /// Return an enumerable of the logical ancestory of the current DependencyObject (including the starting point).
        /// </summary>
        /// <param name="dependencyObject"></param>
        /// <returns></returns>
        public static IEnumerable<DependencyObject> LogicalTreeAncestory(this DependencyObject dependencyObject)
        {
            if (dependencyObject == null) throw new ArgumentNullException("dependencyObject");

            while (dependencyObject != null)
            {
                yield return dependencyObject;
                dependencyObject = LogicalTreeHelper.GetParent(dependencyObject);
            }
        }
    }
}
