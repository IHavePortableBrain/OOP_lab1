using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1_v2.Figures
{
    [Serializable]
    public sealed class MyRectangle : Figure
    {
        public float width, height;
        public MyRectangle() : base()
        {

        }

        public override void Normalize()
        {
            if (pointFs[0].X > pointFs[1].X)
            {
                //StdOps.swap(ref PointFs[0].X,ref PointFs[1].X);
                float tmp = pointFs[0].X;
                pointFs[0].X = pointFs[1].X;
                pointFs[1].X = tmp;
            }

            if (pointFs[0].Y > pointFs[1].Y)
            {
                //StdOps.swap(ref PointFs[0].Y,ref PointFs[1].Y);
                float tmp = pointFs[0].Y;
                pointFs[0].Y = pointFs[1].Y;
                pointFs[1].Y = tmp;
            }
        }

        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);

            //Normalize();
            width = Math.Abs(pointFs[1].X - pointFs[0].X);
            height = Math.Abs(pointFs[1].Y - pointFs[0].Y);
            graph.DrawRectangle(pen, Math.Min(pointFs[0].X, pointFs[1].X), Math.Min(pointFs[0].Y, pointFs[1].Y), width, height);
        }
    }

}
