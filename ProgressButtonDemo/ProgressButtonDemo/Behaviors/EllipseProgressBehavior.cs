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
    public class EllipseProgressBehavior : ProgressBehavior<Ellipse>
    {
        protected override void OnProgressChanged(double oldValue, double newValue)
        {
            UpdateStrokeDashArray();
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            UpdateStrokeDashArray();
        }

        protected virtual double GetTotalLength()
        {
            if (AssociatedObject == null)
                return 0;

            return (AssociatedObject.ActualHeight - AssociatedObject.StrokeThickness) * Math.PI;
        }


        private void UpdateStrokeDashArray()
        {
            if (AssociatedObject == null || AssociatedObject.StrokeThickness == 0)
                return;

            //if (target.ActualHeight == 0 || target.ActualWidth == 0)
            //    return;

            var totalLength = GetTotalLength();
            totalLength = totalLength / AssociatedObject.StrokeThickness;
            var section = Progress * totalLength;
            var result = new DoubleCollection { section, double.MaxValue };
            AssociatedObject.StrokeDashArray = result;
        }
    }
}
