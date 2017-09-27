using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Microsoft.Xaml.Interactivity;

namespace ProgressButtonDemo
{
    public class AdjustToSquareBehavior : Behavior<Panel>
    {
        private Size _originalSize;

        /// <summary>
        /// 获取或设置ContentElement的值
        /// </summary>  
        public FrameworkElement ContentElement
        {
            get { return (FrameworkElement)GetValue(ContentElementProperty); }
            set { SetValue(ContentElementProperty, value); }
        }

        /// <summary>
        /// 标识 ContentElement 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ContentElementProperty =
            DependencyProperty.Register("ContentElement", typeof(FrameworkElement), typeof(AdjustToSquareBehavior), new PropertyMetadata(null, OnContentElementChanged));

        private static void OnContentElementChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AdjustToSquareBehavior target = obj as AdjustToSquareBehavior;
            FrameworkElement oldValue = (FrameworkElement)args.OldValue;
            FrameworkElement newValue = (FrameworkElement)args.NewValue;
            if (oldValue != newValue)
                target.OnContentElementChanged(oldValue, newValue);
        }

        protected virtual void OnContentElementChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
                newValue.SizeChanged -= OnContentElementSizeChanged;

            if (newValue != null)
            {
                newValue.SizeChanged += OnContentElementSizeChanged;
                if (Percentage == 0)
                    _originalSize = newValue.RenderSize;
            }

        }

        private void OnContentElementSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Percentage == 0)
                _originalSize = e.NewSize;

            UpdateTargetSize();
        }


        /// <summary>
        /// 获取或设置Percentage的值
        /// </summary>  
        public double Percentage
        {
            get { return (double)GetValue(PercentageProperty); }
            set { SetValue(PercentageProperty, value); }
        }

        /// <summary>
        /// 标识 Percentage 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PercentageProperty =
            DependencyProperty.Register("Percentage", typeof(double), typeof(AdjustToSquareBehavior), new PropertyMetadata(1d, OnPercentageChanged));

        private static void OnPercentageChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            AdjustToSquareBehavior target = obj as AdjustToSquareBehavior;
            double oldValue = (double)args.OldValue;
            double newValue = (double)args.NewValue;
            if (oldValue != newValue)
                target.OnPercentageChanged(oldValue, newValue);
        }

        protected virtual void OnPercentageChanged(double oldValue, double newValue)
        {
            UpdateTargetSize();
        }

        private void UpdateTargetSize()
        {
            if (ContentElement == null)
                return;

            if (AssociatedObject == null)
                return;

            if (_originalSize.Width == 0 || _originalSize.Height == 0 || double.IsNaN(Percentage))
                return;

            if (Percentage == 0)
            {
                AssociatedObject.Height = double.NaN;
                AssociatedObject.Width = double.NaN;
                return;
            }


            var width = _originalSize.Width;
            var height = _originalSize.Height;
            if (width > height)
            {
                AssociatedObject.Width = width - (width - height) * Percentage;
                AssociatedObject.Height = height;
            }
            else
            {
                AssociatedObject.Height = height - (height - width) * Percentage;
                AssociatedObject.Width = width;
            }
            
        }
    }
}
