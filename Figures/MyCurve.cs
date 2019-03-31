using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures
{
    [Serializable]
    public sealed class MyCurve : Figure, IManyPointFigure, IGUIIcon
    {
        private const string IconPath = "icons\\curve.png";

        public MyCurve() : base()
        {

        }
        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            PointF[] toDraw = new PointF[pointCount];
            Array.Copy(pointFs, toDraw, pointCount);
            graph.DrawCurve(pen, toDraw);
        }

        public Icon GetIcon()
        {
            return new Icon(IconPath);
        }
    }
}
