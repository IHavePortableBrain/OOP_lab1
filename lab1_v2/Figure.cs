﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace lab1_v2
{
    public enum DrawMode : int { none, shift, ctrl };

    [Serializable]
    public class Figure
    {
        readonly public static uint MaxPointCount = 20;
        public uint PointCount;
        public PointF[] PointFs = new PointF[MaxPointCount];
        
        private Color PenColor;
        private float PenWidth;
        [NonSerialized]
        private Pen BorderPen;
        public Pen Pen { set { BorderPen = value; PenColor = BorderPen.Color; PenWidth = BorderPen.Width; } get { return BorderPen; } } 
        public DrawMode DrawMode; 

        protected internal Figure(Pen pen)
        {
            Pen = pen;
            DrawMode = DrawMode.none;
            PointCount = 0;
        }

        public virtual void Normalize()
        {

        }
        //before Modify specify PointFs  
        public virtual void Modify() {

            if (DrawMode == DrawMode.shift)//обращение за границу бмп может быть айайай?
            {
                float Width = PointFs[1].X - PointFs[0].X,
                      Height = PointFs[1].Y - PointFs[0].Y;
                float MaxDelta = Math.Max(Math.Abs(Width), Math.Abs(Height));
                //MaxDelta = (MaxDelta == Math.Abs(Width)) ? Width : Height;
                PointFs[1].Y = PointFs[0].Y + (Height > 0 ? 1 : -1) * MaxDelta;
                PointFs[1].X = PointFs[0].X + (Width > 0 ? 1 : -1) * MaxDelta;
            }
        }
        //before drawing specify PointFs  
        public virtual void Draw(Graphics graph) { }//;
    }

    [Serializable]
    public sealed class MyLine : Figure
    {
        public MyLine(Pen pen) : base(pen)
        {

        }

        public override void Draw(Graphics graph)
        {
            if (PointCount < 2)
                throw new InvalidOperationException();
            graph.DrawLine(this.Pen, PointFs[0], PointFs[1]);
        }
    }

    [Serializable]
    public class MyEllipse : Figure
    {
        public float Width, Height;
        public MyEllipse(Pen pen) : base(pen)
        {

        }
        public override void Draw(Graphics graph)
        {
            if (PointCount < 2)
                throw new InvalidOperationException();
            Width = PointFs[1].X - PointFs[0].X;
            Height = PointFs[1].Y - PointFs[0].Y;
            graph.DrawEllipse(Pen, PointFs[0].X, PointFs[0].Y, Width, Height);
        }
    }

    [Serializable]
    public sealed class MyCurve : Figure
    {
        public MyCurve(Pen pen) : base(pen)
        {

        }
        public override void Draw(Graphics graph)
        {
            if (PointCount < 2)
                throw new InvalidOperationException();

            PointF[] ToDraw = new PointF[PointCount];
            Array.Copy(PointFs, ToDraw, PointCount);
            graph.DrawCurve(Pen, ToDraw);
        }
    }

    [Serializable]
    public sealed class MyRectangle : Figure
    {
        public float Width, Height;
        public MyRectangle(Pen pen) : base(pen)
        {

        }


        public override void Normalize()
        {
            if (PointFs[0].X > PointFs[1].X)
            {
                //StdOps.swap(ref PointFs[0].X,ref PointFs[1].X);
                float tmp = PointFs[0].X;
                PointFs[0].X = PointFs[1].X;
                PointFs[1].X = tmp;
            }

            if (PointFs[0].Y > PointFs[1].Y)
            {
                //StdOps.swap(ref PointFs[0].Y,ref PointFs[1].Y);
                float tmp = PointFs[0].Y;
                PointFs[0].Y = PointFs[1].Y;
                PointFs[1].Y = tmp;
            }
        }

        public override void Draw(Graphics graph)
        {
            if (PointCount < 2)
                throw new InvalidOperationException();
            //Normalize();
            Width = Math.Abs(PointFs[1].X - PointFs[0].X);
            Height = Math.Abs(PointFs[1].Y - PointFs[0].Y);
            graph.DrawRectangle(Pen,Math.Min(PointFs[0].X, PointFs[1].X), Math.Min(PointFs[0].Y, PointFs[1].Y), Width, Height);
        }
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
    //public sealed class MyPolygon : Figure
    //{
    //    public MyPolygon(Pen pen, PointF[] pointFs) : base(pointFs.Length)
    //    {
    //        Pen = pen;
    //        for (int i = 0; i < pointFs.Length; i++)
    //        {
    //            this.PointFs[i] = pointFs[i];
    //        }
    //    }
    //    public override void Draw(Graphics graph)
    //    {
    //        graph.DrawPolygon(Pen, PointFs);
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
