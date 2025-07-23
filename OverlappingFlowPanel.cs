using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CLTL
{
	public class OverlappingCanvasPanel : Panel
	{
		public static readonly DependencyProperty LeftOffsetProperty =
			DependencyProperty.Register("LeftOffset", typeof(double), typeof(OverlappingCanvasPanel), new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double LeftOffset
		{
			get { return (double)GetValue(LeftOffsetProperty); }
			set { SetValue(LeftOffsetProperty, value); }
		}

		public static readonly DependencyProperty TopOffsetProperty =
			DependencyProperty.Register("TopOffset", typeof(double), typeof(OverlappingCanvasPanel), new FrameworkPropertyMetadata(5.0, FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public double TopOffset
		{
			get { return (double)GetValue(TopOffsetProperty); }
			set { SetValue(TopOffsetProperty, value); }
		}

		protected override Size MeasureOverride(Size availableSize)
		{
			double desiredWidth = 0;
			double desiredHeight = 0;

			foreach (UIElement child in InternalChildren)
			{
				child.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			}

			if (InternalChildren.Count > 0)
			{
				desiredWidth = (InternalChildren.Count - 1) * LeftOffset + InternalChildren[InternalChildren.Count - 1].DesiredSize.Width;
				desiredHeight = (InternalChildren.Count - 1) * TopOffset + InternalChildren[InternalChildren.Count - 1].DesiredSize.Height;

				double maxChildWidth = 0;
				double maxChildHeight = 0;
				foreach (UIElement child in InternalChildren)
				{
					maxChildWidth = System.Math.Max(maxChildWidth, child.DesiredSize.Width);
					maxChildHeight = System.Math.Max(maxChildHeight, child.DesiredSize.Height);
				}
				desiredWidth = (InternalChildren.Count - 1) * LeftOffset + maxChildWidth;
				desiredHeight = (InternalChildren.Count - 1) * TopOffset + maxChildHeight;
			}

			return new Size(desiredWidth, desiredHeight);
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			double currentLeft = 0;
			double currentTop = 0;
			int zIndex = 0;

			foreach (UIElement child in InternalChildren)
			{
				child.Arrange(new Rect(currentLeft, currentTop, child.DesiredSize.Width, child.DesiredSize.Height));

				SetZIndex(child, zIndex);

				currentLeft += LeftOffset;
				currentTop += TopOffset;
				zIndex++;
			}

			return finalSize;
		}
	}
}