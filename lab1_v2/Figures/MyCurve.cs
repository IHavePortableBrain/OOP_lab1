﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_v2.Figures
{
    [Serializable]
    public sealed class MyCurve : Figure
    {
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
    }
}
