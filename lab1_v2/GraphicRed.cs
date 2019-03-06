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

// todo
// 2. correct naming
// 3. get rid of cases and switches for shapes
namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        Bitmap Bmp, BmpStore;
        Graphics Graph;
        Pen Pen;
        List<PointF> Points = new List<PointF>(0);
        enum EnFig : int { curve, ellipse, line, rect };
        enum State : int { draw, wait, init };
        enum KeyControl : int { none, shift, ctrl };
        KeyControl KeyCtrl = KeyControl.none;
        EnFig enFig;
        State state = State.init;

        public form_graphic()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MyLine my = new MyLine(40,40,40,40);
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

            List<Figure> figures = new List<Figure>();
            figures.Add(new MyLine(0, 0, 0, 0));
            figures.Add(new MyEllipse(0, 0, 100, 300));
            figures.Add(new MyCurve(testPointFs));
            figures.Add(new MyRectangle(50, 50, 50, 100));
            figures.Add(new MyPolyline(testPointFs));
            figures.Add(new MyPolygon(testPointFs2));
            foreach (Figure figure in figures)
            {
                figure.Draw(Graph, Pen);
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

            PB.Image = Bmp;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Figure> figures = new List<Figure>();
            PointF[] pointFs = new PointF[] { new PointF(50,50),
                                              new PointF(75,40),
                                              new PointF(100,50)};

            MyRectangle myRectangle = new MyRectangle(50, 50, 50, 50);
            MyPolyline myPolyline = new MyPolyline(pointFs);
            figures.Add(myRectangle);
            figures.Add(myPolyline);
            foreach (Figure figure in figures)
            {
                figure.Draw(Graph, Pen);

                PB.Image = Bmp;
                TimeSpan interval = new TimeSpan(0, 0, 2);
                Thread.Sleep(interval);

            }
        }

        private void LVfigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            Points.RemoveRange(0, Points.Count);
            state = State.init;

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

        private void DrawSpecifiedFigure()
        {

            float TopLeftX, TopLeftY, Delta, Width, Height;

            Delta = Points[1].X - Points[0].X;
            if (Delta > 0)
            {
                TopLeftX = Points[0].X;
                Width = Delta;
            }
            else
            {
                TopLeftX = Points[1].X;
                Width = Points[0].X - Points[1].X;
            }

            Delta = Points[1].Y - Points[0].Y;
            if (Delta > 0)
            {
                TopLeftY = Points[0].Y;
                Height = Delta;
            }
            else
            {
                TopLeftY = Points[1].Y;
                Height = Points[0].Y - Points[1].Y;
            }


            switch (enFig)
            {
                case EnFig.curve:
                    MyCurve Curve = new MyCurve(Points.ToArray());
                    Curve.Draw(Graph, Pen);
                    break;
                case EnFig.ellipse:
                    if (KeyCtrl == KeyControl.shift)
                    {
                        float temp = Math.Max(Width, Height);
                        if (temp == Width)
                        {
                            Height = temp;
                        }
                        else
                            Width = temp;
                    }
                    MyEllipse Ellipse = new MyEllipse(TopLeftX, TopLeftY, Width, Height);
                    Ellipse.Draw(Graph, Pen);
                    break;
                case EnFig.line:
                    MyLine Line = new MyLine(Points[0].X, Points[0].Y, Points[1].X, Points[1].Y);
                    Line.Draw(Graph, Pen);
                    break;
                case EnFig.rect:
                    if (KeyCtrl == KeyControl.shift)
                    {
                        float temp = Math.Max(Width, Height);
                        if (temp == Width)
                        {
                            Height = temp;
                        }
                        else
                            Width = temp;
                    }
                    MyRectangle Rectangle = new MyRectangle(TopLeftX, TopLeftY, Width, Height);
                    Rectangle.Draw(Graph, Pen);
                    break;
                default:
                    break;
            }
            PB.Image = Bmp;
        }

        private void form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать, why drawing metod is sepetated from class; попросить научить пользоваться отладчиком по памяти
            PB.Width = 599;
            Bmp = new Bitmap(PB.Height, PB.Width);
            Graph = Graphics.FromImage(Bmp);
            Pen = new Pen(Color.Green);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            state = State.wait;
            PB.Image = null;
            if (Graph != null)
            {
                Graph.Clear(Color.White);
                Points.RemoveRange(0, Points.Count);
            }
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == State.wait && enFig == EnFig.curve)
            {
                restoreBmp();
                Points.Add(e.Location);
                if (Points.Count > 1)
                    DrawSpecifiedFigure();
                PB.Image = Bmp;
            }
            else
            if (state == State.wait || state == State.init)
            {
                BmpStore = Bmp.Clone(new System.Drawing.Rectangle(0, 0, Bmp.Width, Bmp.Height), Bmp.PixelFormat);
                Points.Add(e.Location);
                Points.Add(e.Location);
                state = State.draw;
            }

        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                Points[1] = e.Location;
                DrawSpecifiedFigure();
                if (enFig != EnFig.curve)
                {
                    Points.RemoveRange(0, Points.Count);
                    state = State.init;
                }
                else
                    state = State.wait;
            }

        }

        private void form_graphic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26)//ctrl+z && state != State.init
            {
                swap(ref Bmp, ref BmpStore);
                Graph = Graphics.FromImage(Bmp);


                PB.Image = Bmp;
            }
        }

        private void form_graphic_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                KeyCtrl = KeyControl.none;
        }

        private void form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                KeyCtrl = KeyControl.shift;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                restoreBmp();
                Points[1] = e.Location;
                DrawSpecifiedFigure();
            }
        }
        //_____________________________________________________

        private void restoreBmp()
        {
            Bmp.Dispose();
            Bmp = BmpStore.Clone(new System.Drawing.Rectangle(0, 0, BmpStore.Width, BmpStore.Height), BmpStore.PixelFormat);
            Graph.Dispose();
            Graph = Graphics.FromImage(Bmp);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            Pen.Color = colorDialog.Color;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Pen.Width = (float)numericUpDown1.Value;
            }
            catch (Exception)
            {

                throw;
            }

        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                Pen.Width = (float)numericUpDown1.Value;
            }
            catch (Exception)
            {

                throw;
            }
        }

        static void swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }
    }
}
