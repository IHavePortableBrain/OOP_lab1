using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_v2.Figures
{
    [Serializable]
    public sealed class MyLine : Figure
    {

        public MyLine() : base()
        {

        }
        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            graph.DrawLine(pen, pointFs[0], pointFs[1]);
        }
    }
}
