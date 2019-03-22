﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_v2.Figures
{
    [Serializable]
    public class MyEllipse : Figure
    {
        public float width, height;

        public MyEllipse() : base()
        {

        }

        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            width = pointFs[1].X - pointFs[0].X;
            height = pointFs[1].Y - pointFs[0].Y;
            graph.DrawEllipse(pen, pointFs[0].X, pointFs[0].Y, width, height);
        }
    }
}