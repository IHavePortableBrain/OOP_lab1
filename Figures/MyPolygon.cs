using System.Drawing;
using System;

namespace Figures
{
    [Serializable]
    public sealed class MyPolygon : Figure, IManyPointFigure, IGUIIcon
    {
        private const string IconPath = "icons\\polygon.png";

        public MyPolygon() : base()
        {

        }
        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            PointF[] toDraw = new PointF[pointCount];
            Array.Copy(pointFs, toDraw, pointCount);
            graph.DrawPolygon(pen, toDraw);
        }

        public Icon GetIcon()
        {
            return new Icon(IconPath);
        }
    }
}