namespace XamlPowerToys.UI.DragDrop {
    using System;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// Represents an InsertionAdorner
    /// </summary>
    public class InsertionAdorner : Adorner {
        static readonly Pen _pen;
        static readonly PathGeometry _triangle;
        readonly AdornerLayer _adornerLayer;
        readonly Boolean _isSeparatorHorizontal;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is in first half.
        /// </summary>
        /// <value><c>true</c> if this instance is in first half; otherwise, <c>false</c>.</value>
        public Boolean IsInFirstHalf { get; set; }

        // Create the pen and triangle in a static constructor and freeze them to improve performance.
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertionAdorner"/> class.
        /// </summary>
        static InsertionAdorner() {
            _pen = new Pen {Brush = Brushes.Gray, Thickness = 2};
            _pen.Freeze();

            var firstLine = new LineSegment(new Point(0, -5), false);
            firstLine.Freeze();
            var secondLine = new LineSegment(new Point(0, 5), false);
            secondLine.Freeze();

            var figure = new PathFigure {StartPoint = new Point(5, 0)};
            figure.Segments.Add(firstLine);
            figure.Segments.Add(secondLine);
            figure.Freeze();

            _triangle = new PathGeometry();
            _triangle.Figures.Add(figure);
            _triangle.Freeze();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InsertionAdorner"/> class.
        /// </summary>
        /// <param name="isSeparatorHorizontal">if set to <c>true</c> [is separator horizontal].</param>
        /// <param name="isInFirstHalf">if set to <c>true</c> [is in first half].</param>
        /// <param name="adornedElement">The adorned element.</param>
        /// <param name="adornerLayer">The adorner layer.</param>
        public InsertionAdorner(Boolean isSeparatorHorizontal, Boolean isInFirstHalf, UIElement adornedElement, AdornerLayer adornerLayer)
            : base(adornedElement) {
            this._isSeparatorHorizontal = isSeparatorHorizontal;
            this.IsInFirstHalf = isInFirstHalf;
            this._adornerLayer = adornerLayer;
            this.IsHitTestVisible = false;

            this._adornerLayer.Add(this);
        }

        /// <summary>
        /// Detaches this instance.
        /// </summary>
        public void Detach() {
            this._adornerLayer.Remove(this);
        }

        // This draws one line and two triangles at each end of the line.
        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext) {
            Point startPoint;
            Point endPoint;

            CalculateStartAndEndPoint(out startPoint, out endPoint);
            drawingContext.DrawLine(_pen, startPoint, endPoint);

            if (this._isSeparatorHorizontal) {
                DrawTriangle(drawingContext, startPoint, 0);
                DrawTriangle(drawingContext, endPoint, 180);
            } else {
                DrawTriangle(drawingContext, startPoint, 90);
                DrawTriangle(drawingContext, endPoint, -90);
            }
        }

        void CalculateStartAndEndPoint(out Point startPoint, out Point endPoint) {
            startPoint = new Point();
            endPoint = new Point();

            Double width = this.AdornedElement.RenderSize.Width;
            Double height = this.AdornedElement.RenderSize.Height;

            if (this._isSeparatorHorizontal) {
                endPoint.X = width;
                if (!this.IsInFirstHalf) {
                    startPoint.Y = height;
                    endPoint.Y = height;
                }
            } else {
                endPoint.Y = height;
                if (!this.IsInFirstHalf) {
                    startPoint.X = width;
                    endPoint.X = width;
                }
            }
        }

        void DrawTriangle(DrawingContext drawingContext, Point origin, Double angle) {
            drawingContext.PushTransform(new TranslateTransform(origin.X, origin.Y));
            drawingContext.PushTransform(new RotateTransform(angle));

            drawingContext.DrawGeometry(_pen.Brush, null, _triangle);

            drawingContext.Pop();
            drawingContext.Pop();
        }
    }
}