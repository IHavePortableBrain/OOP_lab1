using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Shapes;

namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        Bitmap bmp;
        Graphics graph;
        Pen Pen;



        public abstract class figure
        {
            private readonly int PointCount;
            public PointF[] pointFs;
            protected figure(int PointCount)
            {
                if (PointCount > 1)
                {
                    this.PointCount = PointCount;
                    pointFs = new PointF[this.PointCount];
                }
                else
                {
                    //throw exeption
                }
            }
        }

        public sealed class MyLine : figure
        {
            public MyLine(float fromX, float fromY, float toX, float toY): base(2)
            {
                pointFs[0] = new PointF(fromX, fromY);
                pointFs[1] = new PointF(toX, toY);
            }
        }

        public sealed class MyPolygon : figure
        {
            public MyPolygon(int pointCount, PointF[] pointFs):base(pointCount)
            {
                for(int i = 0;i< pointCount; i++)
                {
                    this.pointFs[i] = pointFs[i];
                }
            }
        }

        public sealed class MyEllipse : figure
        {
            public float width, height;
            public MyEllipse(float leftTopX, float leftTopY, float width, float height):base(1)
            {
                this.pointFs[0] = new PointF(leftTopX, leftTopY);
                this.width = width;
                this.height = height;
            }

        }

        public sealed class MyRectangle: figure
        {
            public float width, height;
            public MyRectangle(float leftTopX, float leftTopY, float width, float height) : base(1)
            {
                this.pointFs[0] = new PointF(leftTopX, leftTopY);
                this.width = width;
                this.height = height;
            }
        }

        public form_graphic()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            graph.DrawEllipse(Pen, 29, 29, 29, 29);

            PointF[] testPointFs = new PointF[4];
            testPointFs[0] = new PointF(100, 200);
            testPointFs[1] = new PointF(400, 300);
            testPointFs[2] = new PointF(200, 300);
            testPointFs[3] = new PointF(200, 200);

            MyLine l = new MyLine(0, 0, 100, 40);
            MyPolygon p = new MyPolygon(4, testPointFs);
            graph.DrawLine(Pen, l.pointFs[0], l.pointFs[1]);
            graph.DrawPolygon(Pen, p.pointFs);
            graph.DrawEllipse(Pen, 0, 0, 300, 100);
            graph.DrawRectangle(Pen,50, 50, 50, 100);
            Shape shape;
            PB.Image = bmp;
        }

        private void form_graphic_Activated(object sender, EventArgs e)
        {
            bmp = new Bitmap(PB.Height, PB.Width);
            
            graph = Graphics.FromImage(bmp);
            Pen = new Pen(Color.Green);
        }
    }
}
