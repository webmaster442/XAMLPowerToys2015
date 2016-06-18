namespace XamlPowerToys.UI.DragDrop {
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    public class DraggedAdorner : Adorner {

        readonly AdornerLayer _adornerLayer;
        readonly ContentPresenter _contentPresenter;
        Double _left;
        Double _top;
        protected override Int32 VisualChildrenCount => 1;

        public DraggedAdorner(Object dragDropData, DataTemplate dragDropTemplate, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement) {
            this._adornerLayer = adornerLayer;

            this._contentPresenter = new ContentPresenter();
            this._contentPresenter.Content = dragDropData;
            this._contentPresenter.ContentTemplate = dragDropTemplate;
            this._contentPresenter.Opacity = 0.7;

            this._adornerLayer.Add(this);
        }

        public void Detach() {
            this._adornerLayer.Remove(this);
        }

        public override GeneralTransform GetDesiredTransform(GeneralTransform transform) {
            var result = new GeneralTransformGroup();

// ReSharper disable AssignNullToNotNullAttribute
            result.Children.Add(base.GetDesiredTransform(transform));

// ReSharper restore AssignNullToNotNullAttribute
            result.Children.Add(new TranslateTransform(this._left, this._top));

            return result;
        }

        public void SetPosition(Double left, Double top) {
            // -1 and +13 align the dragged adorner with the dashed rectangle that shows up
            // near the mouse cursor when dragging.
            this._left = left - 1;
            this._top = top + 13;
            if (this._adornerLayer != null) {
                try {
                    this._adornerLayer.Update(this.AdornedElement);
                } catch (InvalidOperationException) {
                }
            }
        }

        protected override Size ArrangeOverride(Size finalSize) {
            this._contentPresenter.Arrange(new Rect(finalSize));
            return finalSize;
        }

        protected override Visual GetVisualChild(Int32 index) {
            return this._contentPresenter;
        }

        protected override Size MeasureOverride(Size constraint) {
            this._contentPresenter.Measure(constraint);
            return this._contentPresenter.DesiredSize;
        }

    }
}
