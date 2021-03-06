﻿using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures
{
    [Serializable]
    public sealed class MyLine : Figure, IGUIIcon
    {
        private const string IconPath = "icons\\line.png";

        public MyLine() : base()
        {

        }
        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            graph.DrawLine(pen, pointFs[0], pointFs[1]);
        }

        public Icon GetIcon()
        {
            return new Icon(IconPath);
        }
    }
}
