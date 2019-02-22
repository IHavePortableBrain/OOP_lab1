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


using System.Threading;


namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        public const int maxPointCount = 20;
        public int pointCount  = 0;
        Bitmap bmp, bmpStore;
        Graphics graph;
        Pen Pen;
        Point[] Points = new Point[maxPointCount];
        enum EnFig:int {curve, ellipse, line,rect};
        enum State: int {draw, wait, init};
        EnFig enFig;
        State state = State.init;

        public abstract class figure
        {
            private readonly int PointCount;
            public PointF[] pointFs;
            protected figure(int PointCount)
            {
                if (PointCount > 0)
                {
                    this.PointCount = PointCount;
                    pointFs = new PointF[this.PointCount];
                }
                else
                {
                    //throw exeption
                }
            }

            public abstract void Draw(Graphics graph, Pen pen);
        }

        public sealed class MyLine : figure
        {
            public MyLine(float fromX, float fromY, float toX, float toY) : base(2)
            {
                pointFs[0] = new PointF(fromX, fromY);
                pointFs[1] = new PointF(toX, toY);
            }
            ~MyLine(){
            }
            public override void Draw(Graphics graph,Pen pen)
            {
                graph.DrawLine(pen, pointFs[0].X, pointFs[0].Y, pointFs[1].X, pointFs[1].Y);
            }
        }

        public sealed class MyPolygon : figure
        {
            public MyPolygon(PointF[] pointFs) : base(pointFs.Length)
            {
                for (int i = 0; i < pointFs.Length; i++)
                {
                    this.pointFs[i] = pointFs[i];
                }
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawPolygon(pen, pointFs);
            }
        }

        public class MyEllipse : figure
        {
            public float width, height;
            public MyEllipse(float leftTopX, float leftTopY, float width, float height) : base(1)
            {
                pointFs[0].X = leftTopX;
                pointFs[0].Y = leftTopY;
                this.width = width;
                this.height = height;
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawEllipse(pen, pointFs[0].X, pointFs[0].Y, width, height);
            }
        }

        public sealed class MyPie : MyEllipse
        {
            public float startAngle, sweepAngle;
            public MyPie(MyRectangle rect, float startAngle, float sweepAngle) : base(rect.pointFs[0].X, rect.pointFs[0].Y, rect.width, rect.height)
            {
                this.startAngle = startAngle;
                this.sweepAngle = sweepAngle;
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawPie(pen, pointFs[0].X, pointFs[0].Y,width, height, startAngle, sweepAngle);
            }
        }

        public sealed class MyCurve : figure
        {
            public MyCurve(PointF[] pointFs) : base(pointFs.Length)
            {
                for (int i = 0; i < pointFs.Length; i++)
                {
                    this.pointFs[i] = pointFs[i];
                }
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawCurve(pen, pointFs);
            }
        }

        public sealed class MyRectangle : figure
        {
            public float width, height;
            public MyRectangle(float leftTopX, float leftTopY, float width, float height) : base(1)
            {
                pointFs[0].X = leftTopX;
                pointFs[0].Y = leftTopY;
                this.width = width;
                this.height = height;
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawRectangle(pen, pointFs[0].X, pointFs[0].Y, width, height);
            }
        }

        public sealed class MyPolyline : figure
        {
            public MyPolyline(PointF[] pointFs) : base(pointFs.Length)
            {
                for (int i = 0; i < pointFs.Length; i++)
                {
                    this.pointFs[i] = pointFs[i];
                }
            }
            public override void Draw(Graphics graph, Pen pen)
            {
                graph.DrawLines(pen, pointFs);
            }
        }

        public form_graphic()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            PointF[] testPointFs = new PointF[4];
            testPointFs[0] = new PointF(0, 0);
            testPointFs[1] = new PointF(100, 300);
            testPointFs[2] = new PointF(100, 400);
            testPointFs[3] = new PointF(120, 390);

            PointF[] testPointFs2 = new PointF[5];
            testPointFs2[0] = new PointF(0, 500);
            testPointFs2[1] = new PointF(10, 340);
            testPointFs2[2] = new PointF(22, 300);
            testPointFs2[3] = new PointF(50, 400);
            testPointFs2[4] = new PointF(35, 340);

            List<figure> figures = new List<figure>();
            figures.Add(new MyLine(0, 0, 0, 0));
            figures.Add(new MyEllipse(0, 0, 100, 300));
            figures.Add(new MyCurve(testPointFs));
            figures.Add(new MyRectangle(50, 50, 50, 100));
            figures.Add(new MyPolyline(testPointFs));
            figures.Add(new MyPolygon(testPointFs2));
            foreach (figure figure in figures)
            {
                figure.Draw(graph, Pen);
            }

            //MyLine myLine = new MyLine(0, 0, 100, 40);
            //MyEllipse myEllipse = new MyEllipse(12, 12, 100, 200);
            //MyCurve myCurve = new MyCurve(testPointFs);
            //MyRectangle myRectangle = new MyRectangle(30, 70, 100, 200);
            //MyPie myPie = new MyPie(myRectangle, 20,300);
            //MyPolyline myPolyline = new MyPolyline(testPointFs);
            //MyPolygon myPolygon = new MyPolygon(testPointFs2);
            //myLine.Draw(graph, Pen);
            //Pen.Color = Color.Red;
            //Pen.Brush = Brushes.SeaShell;
            //myEllipse.Draw(graph, Pen);
            //myCurve.Draw(graph, Pen);
            //myRectangle.Draw(graph, Pen);
            //myPie.Draw(graph, Pen);
            //myPolyline.Draw(graph, Pen);
            //myPolygon.Draw(graph, Pen);

            PB.Image = bmp;
        }



        private void form_graphic_Activated(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<figure> figures = new List<figure>();
            PointF[] pointFs = new PointF[] { new PointF(50,50),
                                              new PointF(75,40),
                                              new PointF(100,50)};

            MyRectangle myRectangle = new MyRectangle(50, 50, 50, 50);
            MyPolyline myPolyline = new MyPolyline(pointFs);
            figures.Add(myRectangle);
            figures.Add(myPolyline);
            foreach (figure figure in figures)
            {
                figure.Draw(graph, Pen);

                PB.Image = bmp;
                TimeSpan interval = new TimeSpan(0, 0, 2);
                Thread.Sleep(interval);

            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LVfigures_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            
        }

        private void LVfigures_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (LVfigures.SelectedIndices.Count > 0)
            {
                try
                {
                    enFig = (EnFig)LVfigures.SelectedIndices[0];
                }
                catch (Exception exc)
                {
                    //exe
                }
            }
        }

        private void LVfigures_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {

        }

        static void swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }

        private void DrawSpecifiedFigure()
        {
            int topLeftX,topLeftY,delta,width,height;

            delta = Points[1].X - Points[0].X;
            if (delta > 0)
            {
                topLeftX = Points[0].X;
                width = delta;
            }
            else
            {
                topLeftX = Points[1].X;
                width = Points[0].X - Points[1].X;
            }

            delta = Points[1].Y - Points[0].Y;
            if (delta > 0)
            {
                topLeftY = Points[0].Y;
                height = delta;
            }
            else
            {
                topLeftY = Points[1].Y;
                height = Points[0].Y - Points[1].Y;
            }


            switch (enFig)
            {      
                case EnFig.curve:

                    break;
                case EnFig.ellipse:
                    MyEllipse myEllipse = new MyEllipse(topLeftX, topLeftY, width,height);
                    myEllipse.Draw(graph, Pen);
                    break;
                case EnFig.line:
                    MyLine myLine = new MyLine(Points[0].X, Points[0].Y, Points[1].X, Points[1].Y);
                    myLine.Draw(graph, Pen);
                    break;
                case EnFig.rect:
                    MyRectangle myRectangle = new MyRectangle(topLeftX, topLeftY, width, height);
                    myRectangle.Draw(graph, Pen);
                    break;
                default:
                    break;
            }
        }


        private void form_graphic_Enter(object sender, EventArgs e)
        {
            
        }

        private void form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать, why drawing metod is sepetated from class; попросить научить пользоваться отладчиком по памяти
            PB.Width = 599;
            bmp = new Bitmap(PB.Height, PB.Width);
            graph = Graphics.FromImage(bmp);
            Pen = new Pen(Color.Green);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            state = State.wait;
            PB.Image = null;
            if (graph != null)
            {
                graph.Clear(Color.White);
            }
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            bmpStore = bmp.Clone(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
            Points[0] = e.Location;
            state = State.draw;
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            Points[1] = e.Location;
            DrawSpecifiedFigure();

            state = State.wait;
            PB.Image = bmp;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            
        }

        private void form_graphic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26 && state != State.init)//ctrl+z
            {
                swap(ref bmp, ref bmpStore);
                graph = Graphics.FromImage(bmp);


                PB.Image = bmp;
            }
        }

        private void form_graphic_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            
        }

        private void form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void form_graphic_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                bmp.Dispose();
                bmp = bmpStore.Clone(new System.Drawing.Rectangle(0, 0, bmpStore.Width, bmpStore.Height), bmpStore.PixelFormat);
                graph.Dispose();
                graph = Graphics.FromImage(bmp);
                Points[1] = e.Location;
                DrawSpecifiedFigure();
                PB.Image = bmp;   
            } 
        }
    }
}
