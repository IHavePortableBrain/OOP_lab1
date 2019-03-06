using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace lab1_v2
{
    public class Figure
    {
        private readonly int PointCount;
        public PointF[] PointFs;
        protected Figure(int PointCount)
        {
            if (PointCount > 0)
            {
                this.PointCount = PointCount;
                PointFs = new PointF[this.PointCount];
            }
            else
            {
                //throw exeption
            }
        }

        public virtual void Draw(Graphics graph, Pen pen) { }//;
    }

    public sealed class MyLine : Figure
    {
        public MyLine(float fromX, float fromY, float toX, float toY) : base(2)
        {
            PointFs[0] = new PointF(fromX, fromY);
            PointFs[1] = new PointF(toX, toY);
        }

        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawLine(pen, PointFs[0].X, PointFs[0].Y, PointFs[1].X, PointFs[1].Y);
        }
    }

    public sealed class MyPolygon : Figure
    {
        public MyPolygon(PointF[] pointFs) : base(pointFs.Length)
        {
            for (int i = 0; i < pointFs.Length; i++)
            {
                this.PointFs[i] = pointFs[i];
            }
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawPolygon(pen, PointFs);
        }
    }

    public class MyEllipse : Figure
    {
        public float Width, Height;
        public MyEllipse(float leftTopX, float leftTopY, float width, float height) : base(1)
        {
            PointFs[0].X = leftTopX;
            PointFs[0].Y = leftTopY;
            this.Width = width;
            this.Height = height;
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawEllipse(pen, PointFs[0].X, PointFs[0].Y, Width, Height);
        }
    }

    public sealed class MyPie : MyEllipse
    {
        public float StartAngle, SweepAngle;
        public MyPie(MyRectangle rect, float startAngle, float sweepAngle) : base(rect.PointFs[0].X, rect.PointFs[0].Y, rect.Width, rect.Height)
        {
            this.StartAngle = startAngle;
            this.SweepAngle = sweepAngle;
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawPie(pen, PointFs[0].X, PointFs[0].Y, Width, Height, StartAngle, SweepAngle);
        }
    }

    public sealed class MyCurve : Figure
    {
        public MyCurve(PointF[] pointFs) : base(pointFs.Length)
        {
            for (int i = 0; i < pointFs.Length; i++)
            {
                this.PointFs[i] = pointFs[i];
            }
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawCurve(pen, PointFs);
        }
    }

    public sealed class MyRectangle : Figure
    {
        public float Width, Height;
        public MyRectangle(float leftTopX, float leftTopY, float width, float height) : base(1)
        {
            PointFs[0].X = leftTopX;
            PointFs[0].Y = leftTopY;
            this.Width = width;
            this.Height = height;
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawRectangle(pen, PointFs[0].X, PointFs[0].Y, Width, Height);
        }
    }

    public sealed class MyPolyline : Figure
    {
        public MyPolyline(PointF[] pointFs) : base(pointFs.Length)
        {
            for (int i = 0; i < pointFs.Length; i++)
            {
                this.PointFs[i] = pointFs[i];
            }
        }
        public override void Draw(Graphics graph, Pen pen)
        {
            graph.DrawLines(pen, PointFs);
        }
    }
}
