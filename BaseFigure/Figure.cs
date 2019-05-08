using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Figures
{
    public enum DrawMode : int { none, shift, ctrl };

    [Serializable]
    public class Figure:ICloneable
    {
        //[NonSerialized]
        readonly public static uint MaxPointCount = 20;
        readonly public static uint MinDrawPointCount = 2;
        readonly public static float DefaultPenWidth = 3;
        readonly public static Color DefaultPenColor = Color.Black;

        public uint pointCount;
        public DrawMode DrawMode;
        public PointF[] pointFs = new PointF[MaxPointCount];
        
        public Color PenColor { get; set; }//protected set
        public float PenWidth { get; set; }//protected set


        public Figure()//protected
        {
            PenColor = DefaultPenColor;
            PenWidth = DefaultPenWidth;
            DrawMode = DrawMode.none;
            pointCount = 0;
        }

        public object Clone()
        {
            Figure copy = (Figure)this.MemberwiseClone();
            copy.pointFs = new PointF[MaxPointCount];
            Array.Copy(this.pointFs, copy.pointFs, MaxPointCount);
            return copy;
        }

        public virtual void Normalize()
        {

        }
        //before Modify specify PointFs  
        public virtual void Modify() {
            if (DrawMode == DrawMode.shift)//обращение за границу бмп может быть айайай?
            {
                float Width = pointFs[1].X - pointFs[0].X,
                      Height = pointFs[1].Y - pointFs[0].Y;
                float MaxDelta = Math.Max(Math.Abs(Width), Math.Abs(Height));
                //MaxDelta = (MaxDelta == Math.Abs(Width)) ? Width : Height;
                pointFs[1].Y = pointFs[0].Y + (Height > 0 ? 1 : -1) * MaxDelta;
                pointFs[1].X = pointFs[0].X + (Width > 0 ? 1 : -1) * MaxDelta;
            }
        }
        //before drawing specify PointFs  
        public virtual void Draw(Graphics graph, Pen pen) {
            PenColor = pen.Color;
            PenWidth = pen.Width;
            if (pointCount < MinDrawPointCount)
                throw new InvalidOperationException();
        }//;
    }

    #region
    //[Serializable]
    //public sealed class MyPolyline : Figure
    //{
    //    public MyPolyline(Pen pen,PointF[] pointFs) : base(pointFs.Length)
    //    {
    //        Pen = pen;
    //        for (int i = 0; i < pointFs.Length; i++)
    //        {
    //            this.PointFs[i] = pointFs[i];
    //        }
    //    }
    //    public override void Draw(Graphics graph)
    //    {
    //        graph.DrawLines(Pen, PointFs);
    //    }
    //}

    

    //[Serializable]
    //public sealed class MyPie : MyEllipse
    //{
    //    public float StartAngle, SweepAngle;
    //    public MyPie(Pen pen,MyRectangle rect, float startAngle, float sweepAngle) : base(pen, rect.PointFs[0].X, rect.PointFs[0].Y, rect.Width, rect.Height)
    //    {
    //        Pen = pen;
    //        this.StartAngle = startAngle;
    //        this.SweepAngle = sweepAngle;
    //    }
    //    public override void Draw(Graphics graph)
    //    {
    //        graph.DrawPie(Pen, PointFs[0].X, PointFs[0].Y, Width, Height, StartAngle, SweepAngle);
    //    }
    //}
    #endregion
}
