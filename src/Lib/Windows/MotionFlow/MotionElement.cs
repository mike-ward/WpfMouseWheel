using System;
using System.Diagnostics;
using System.Windows;

namespace Logitech.Windows.MotionFlow
{

    #region IMotionElementInput

    public interface IMotionElementInput : IInputElement
    {
    }

    #endregion

    #region IMotionElement

    public interface IMotionElement : IMotionElementInput
    {
        int Id { get; }
        string Name { get; set; }
    }

    #endregion

    #region MotionElement

    public class MotionElement : ContentElement, IMotionElement
    {
        #region Static

        #region Fields

        private static int _refCount;

        #endregion

        #region Methods

        public static string TransmitMethodSuffix(IMotionInfo info, int nativeDelta)
        {
            return nativeDelta == 0 || Math.Sign(nativeDelta) == info.NativeDirection ? "++" : "--";
        }

        #endregion

        #endregion

        #region Instance

        #region Initialization

        public MotionElement()
        {
            Id = ++_refCount;
            Name = Id.ToString("'M'00");
        }

        #endregion

        #region Object

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region IMotionElement

        public int Id { get; protected set; }
        public string Name { get; set; }

        #endregion

        #endregion
    }

    #endregion

    #region MotionElementLink

    public class MotionElementLink : MotionElement
    {
        #region Fields

        private IMotionElementInput _next;

        #endregion

        #region Methods

        [DebuggerStepThrough]
        protected IMotionElementInput GetNext(bool setParent = true)
        {
            if (setParent)
                _next.SetParent(this);
            return _next;
        }

        [DebuggerStepThrough]
        protected void SetNext(IMotionElementInput value, bool setParent = true)
        {
            _next = value;
            if (setParent)
                value.SetParent(this);
        }

        #endregion
    }

    #endregion

    #region MotionElementExtensions

    public static class MotionElementExtensions
    {
        public static void SetParent(this IMotionElementInput reference, DependencyObject parent)
        {
            var element = reference as ContentElement;
            if (element != null) ContentOperations.SetParent(element, parent);
            else throw new ArgumentException("Given reference is not a ContentElement");
        }
    }

    #endregion
}