using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Logitech.Windows
{

    #region VisualTreeServices

    public static class VisualTreeServices
    {
        public static DependencyObject GetRoot(DependencyObject reference)
        {
            if (reference == null) return null;
            var parent = VisualTreeHelper.GetParent(reference);
            while (parent != null)
            {
                reference = parent;
                parent = VisualTreeHelper.GetParent(reference);
            }
            return reference;
        }

        public static IEnumerable<DependencyObject> GetAncestors(DependencyObject reference)
        {
            if (reference == null) yield break;
            var parent = VisualTreeHelper.GetParent(reference);
            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable<DependencyObject> GetChildren(DependencyObject reference)
        {
            if (reference is Visual || reference is Visual3D)
            {
                var count = VisualTreeHelper.GetChildrenCount(reference);
                for (var i = 0; i < count; i++)
                    yield return VisualTreeHelper.GetChild(reference, i);
            }
        }

        public static IEnumerable<DependencyObject> GetDescendants(DependencyObject reference)
        {
            if (reference == null) yield break;
            foreach (var child in GetChildren(reference))
                foreach (var item in GetTree(child))
                    yield return item;
        }

        public static IEnumerable<DependencyObject> GetTree(DependencyObject reference)
        {
            if (reference == null) yield break;
            yield return reference;
            foreach (var child in GetChildren(reference))
                foreach (var item in GetTree(child))
                    yield return item;
        }

        public static IEnumerable<T> GetTreeItemsOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) yield break;
            foreach (var item in GetTree(reference))
            {
                var asT = item as T;
                if (asT != null) yield return asT;
            }
        }

        public static IEnumerable<T> GetDescendantsOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) yield break;
            foreach (var item in GetDescendants(reference))
            {
                var asT = item as T;
                if (asT != null) yield return asT;
            }
        }

        public static IEnumerable<T> GetAncestorsOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) yield break;
            foreach (var ancestor in GetAncestors(reference))
            {
                var asT = ancestor as T;
                if (asT != null) yield return asT;
            }
        }

        public static T GetFirstTreeItemOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) return null;
            foreach (var item in GetTreeItemsOfType<T>(reference))
                return item;
            return null;
        }

        public static T GetFirstDescendantOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) return null;
            foreach (var item in GetDescendantsOfType<T>(reference))
                return item;
            return null;
        }

        public static T GetFirstAncestorOfType<T>(DependencyObject reference) where T : class
        {
            if (reference == null) return null;
            foreach (var item in GetAncestorsOfType<T>(reference))
                return item;
            return null;
        }
    }

    #endregion

    #region LogicalTreeServices

    public static class LogicalTreeServices
    {
        public static DependencyObject GetRoot(DependencyObject reference)
        {
            var parent = LogicalTreeHelper.GetParent(reference);
            while (parent != null)
            {
                reference = parent;
                parent = LogicalTreeHelper.GetParent(reference);
            }
            return reference;
        }

        public static IEnumerable<DependencyObject> GetAncestors(DependencyObject reference)
        {
            var parent = LogicalTreeHelper.GetParent(reference);
            while (parent != null)
            {
                yield return parent;
                parent = LogicalTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable GetChildren(DependencyObject reference)
        {
            return LogicalTreeHelper.GetChildren(reference);
        }

        public static IEnumerable GetDescendants(DependencyObject reference)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(reference))
            {
                if (child is DependencyObject)
                {
                    foreach (var item in GetTree(child as DependencyObject))
                        yield return item;
                }
                else
                    yield return child;
            }
        }

        public static IEnumerable GetTree(DependencyObject reference)
        {
            yield return reference;
            foreach (var item in GetDescendants(reference))
                yield return item;
        }

        public static IEnumerable<T> GetTreeItemsOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var item in GetTree(reference))
            {
                var asT = item as T;
                if (asT != null) yield return asT;
            }
        }

        public static IEnumerable<T> GetDescendantsOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var item in GetDescendants(reference))
            {
                var asT = item as T;
                if (asT != null) yield return asT;
            }
        }

        public static IEnumerable<T> GetAncestorsOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var ancestor in GetAncestors(reference))
            {
                var asT = ancestor as T;
                if (asT != null) yield return asT;
            }
        }

        public static T GetFirstTreeItemOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var item in GetTreeItemsOfType<T>(reference))
                return item;
            return null;
        }

        public static T GetFirstDescendantOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var item in GetDescendantsOfType<T>(reference))
                return item;
            return null;
        }

        public static T GetFirstAncestorOfType<T>(DependencyObject reference) where T : class
        {
            foreach (var item in GetAncestorsOfType<T>(reference))
                return item;
            return null;
        }
    }

    #endregion

    #region DependencyObjectExtensions

    public static class DependencyObjectExtensions
    {
        public static DependencyObject GetLogicalRoot(this DependencyObject reference)
        {
            return LogicalTreeServices.GetRoot(reference);
        }

        public static IEnumerable<DependencyObject> GetLogicalAncestors(this DependencyObject reference)
        {
            return LogicalTreeServices.GetAncestors(reference);
        }

        public static IEnumerable GetLogicalChildren(this DependencyObject reference)
        {
            return LogicalTreeServices.GetChildren(reference);
        }

        public static IEnumerable GetLogicalDescendants(this DependencyObject reference)
        {
            return LogicalTreeServices.GetDescendants(reference);
        }

        public static IEnumerable GetLogicalTree(this DependencyObject reference)
        {
            return LogicalTreeServices.GetTree(reference);
        }

        public static IEnumerable<T> GetLogicalTreeItemsOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetTreeItemsOfType<T>(reference);
        }

        public static IEnumerable<T> GetLogicalDescendantsOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetDescendantsOfType<T>(reference);
        }

        public static IEnumerable<T> GetLogicalAncestorsOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetAncestorsOfType<T>(reference);
        }

        public static T GetFirstLogicalTreeItemOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetFirstTreeItemOfType<T>(reference);
        }

        public static T GetFirstLogicalDescendantOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetFirstDescendantOfType<T>(reference);
        }

        public static T GetFirstLogicalAncestorOfType<T>(this DependencyObject reference) where T : class
        {
            return LogicalTreeServices.GetFirstAncestorOfType<T>(reference);
        }

        public static DependencyObject GetVisualRoot(this DependencyObject reference)
        {
            return VisualTreeServices.GetRoot(reference);
        }

        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject reference)
        {
            return VisualTreeServices.GetAncestors(reference);
        }

        public static IEnumerable<DependencyObject> GetVisualChildren(this DependencyObject reference)
        {
            return VisualTreeServices.GetChildren(reference);
        }

        public static IEnumerable<DependencyObject> GetVisualDescendants(this DependencyObject reference)
        {
            return VisualTreeServices.GetDescendants(reference);
        }

        public static IEnumerable<DependencyObject> GetVisualTree(this DependencyObject reference)
        {
            return VisualTreeServices.GetTree(reference);
        }

        public static IEnumerable<T> GetVisualTreeItemsOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetTreeItemsOfType<T>(reference);
        }

        public static IEnumerable<T> GetVisualDescendantsOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetDescendantsOfType<T>(reference);
        }

        public static IEnumerable<T> GetVisualAncestorsOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetAncestorsOfType<T>(reference);
        }

        public static T GetFirstVisualTreeItemOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetFirstTreeItemOfType<T>(reference);
        }

        public static T GetFirstVisualDescendantOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetFirstDescendantOfType<T>(reference);
        }

        public static T GetFirstVisualAncestorOfType<T>(this DependencyObject reference) where T : class
        {
            return VisualTreeServices.GetFirstAncestorOfType<T>(reference);
        }


        public static IEnumerable<BindingExpression> EnumerateBindings(this DependencyObject reference)
        {
            var enumerator = reference.GetLocalValueEnumerator();
            while (enumerator.MoveNext())
            {
                var entry = enumerator.Current;
                if (BindingOperations.IsDataBound(reference, entry.Property))
                    yield return entry.Value as BindingExpression;
            }
        }
    }

    #endregion
}