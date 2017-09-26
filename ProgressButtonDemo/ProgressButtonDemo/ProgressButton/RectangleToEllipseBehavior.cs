using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Microsoft.Xaml.Interactivity;

namespace ProgressButtonDemo
{
   public class RectangleToEllipseBehavior : Behavior<Rectangle>
    {
        /// <summary>
        /// 获取或设置Progress的值
        /// </summary>  
        public double Progress
        {
            get { return (double)GetValue(ProgressProperty); }
            set { SetValue(ProgressProperty, value); }
        }

        /// <summary>
        /// 标识 Progress 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ProgressProperty =
            DependencyProperty.Register("Progress", typeof(double), typeof(RectangleToEllipseBehavior), new PropertyMetadata(0d, OnProgressChanged));

        private static void OnProgressChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var target = obj as RectangleToEllipseBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnProgressChanged(oldValue, newValue);
        }


        protected virtual void OnProgressChanged(double oldValue, double newValue)
        {
            UpdateStrokeDashArray();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateStrokeDashArray();
        }


        private void UpdateStrokeDashArray()
        {
            if (AssociatedObject == null || AssociatedObject.ActualHeight == 0 || AssociatedObject.ActualWidth == 0)
                return;

            var radius = Math.Min(AssociatedObject.ActualHeight, AssociatedObject.ActualWidth) / 2;
            radius = radius * Progress;
            AssociatedObject.RadiusX = radius;
            AssociatedObject.RadiusY = radius;
        }
    }
}
