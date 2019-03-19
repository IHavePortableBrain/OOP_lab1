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
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


using System.Threading;

// todo
// 2. correct naming
// 3. get rid of cases and switches for shapes
//5. баг если не отжимать лкм в пределах пб?
// 6. баг рисование курвой после очистки
//7. рисование при невыбранной фигуре
//9 redo и undo не меняет состояние панели выбранной фигуры
// ask
//1. как свапать переменные закрываемые свойствами?(передать как ref)
//2. Общее начало виртуального(абстрактного) метода у потомков?.
//3. как зная обявленные классы
namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        Bitmap Bmp, BmpStore;
        Graphics Graph;
        Pen Pen;
        Figure SpecifiedFigure = null;
        

        private const string UndoFileName = "ctrlz.dat";
        private const string RedoFileName = "ctrly.dat";
        FileStream UndoFile = new FileStream(UndoFileName, FileMode.Create);
        FileStream RedoFile = new FileStream(RedoFileName, FileMode.Create);
        BinaryFormatter Formatter = new BinaryFormatter();
        UInt32 UndoFiguresCount = 0;
        UInt32 RedoFiguresCount = 0;

        enum EnFig : int { curve, ellipse, line, rect };
        enum State : int { draw, wait, init };

        EnFig enFig;
        State state = State.init;

        public form_graphic()
        {
            InitializeComponent();
        }

        private void LVfigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            SpecifiedFigure.PointCount = 0;
            Pen.Color = colorDialog.Color;
            Pen.Width = (float)numericUpDown1.Value;

            state = State.init;
            if (LVfigures.SelectedIndices.Count > 0)
            {
                enFig = (EnFig)LVfigures.SelectedIndices[0];

                switch (enFig)
                {
                    case EnFig.curve:
                        SpecifiedFigure = new MyCurve();
                        break;
                    case EnFig.ellipse:
                        SpecifiedFigure = new MyEllipse();
                        break;
                    case EnFig.line:
                        SpecifiedFigure = new MyLine();
                        break;
                    case EnFig.rect:
                        SpecifiedFigure = new MyRectangle();
                        break;
                }
            }
        }

        private void DrawSpecifiedFigure()
        {
            SpecifiedFigure.Modify();
            SpecifiedFigure.Draw(Graph, Pen);
            PB.Image = Bmp;
        }

        private void LoadFigures()
        {
            
            LVfigures.Items.Insert(LVfigures.Items.Count, new ListViewItem());
        }

        private void form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать, why drawing metod is sepetated from class; попросить научить пользоваться отладчиком по памяти
            PB.Width = 599;
            Bmp = new Bitmap(PB.Height, PB.Width);
            Graph = Graphics.FromImage(Bmp);
            SpecifiedFigure = new MyLine();
            Pen = new Pen(SpecifiedFigure.PenColor, SpecifiedFigure.PenWidth);
            LoadFigures();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            state = State.wait;
            PB.Image = null;
            //BmpStore.Dispose(); 
            if (Graph != null)
            {
                UndoFile.Dispose();
                RedoFile.Dispose();
                UndoFile = new FileStream(UndoFileName, FileMode.Create);
                RedoFile = new FileStream(RedoFileName, FileMode.Create);
                UndoFiguresCount = 0;
                RedoFiguresCount = 0;

                Graph.Clear(Color.White);
                SpecifiedFigure.PointCount = 0;
            }
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == State.wait && enFig == EnFig.curve)
            {
                restoreBmp();
                if (SpecifiedFigure.PointCount == Figure.MaxPointCount)
                    SpecifiedFigure.PointCount = 0;
                SpecifiedFigure.PointFs[SpecifiedFigure.PointCount++] = e.Location;
                if (SpecifiedFigure.PointCount > 1)
                    DrawAndSerializeSpecifiedFigure();
                PB.Image = Bmp;
            }
            else
            if (state == State.wait || state == State.init)
            {
                BmpStore = Bmp.Clone(new System.Drawing.Rectangle(0, 0, Bmp.Width, Bmp.Height), Bmp.PixelFormat);
                SpecifiedFigure.PointFs[SpecifiedFigure.PointCount++] = e.Location;
                SpecifiedFigure.PointFs[SpecifiedFigure.PointCount++] = e.Location;
                state = State.draw;
            }

        }

        private void DrawAndSerializeSpecifiedFigure()
        {
            DrawSpecifiedFigure();
            Formatter.Serialize(UndoFile, SpecifiedFigure);
            UndoFiguresCount++;
            UndoFile.Flush();
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                SpecifiedFigure.PointFs[1] = e.Location;
                DrawAndSerializeSpecifiedFigure();
                if (enFig != EnFig.curve)
                {
                    SpecifiedFigure.PointCount = 0;
                    state = State.init;
                }
                else
                    state = State.wait;
            }

        }

        private void Undo()
        {
            PB.Image = null;
            if (Graph != null)
            {
                Graph.Clear(Color.White);
            }
            UndoFile.Position = 0;
            UndoFiguresCount--;
            for (int i = 0; i < UndoFiguresCount; i++)
            {
                SpecifiedFigure = (Figure)Formatter.Deserialize(UndoFile);
                Pen = new Pen(SpecifiedFigure.PenColor, SpecifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
            Formatter.Serialize(RedoFile, Formatter.Deserialize(UndoFile));
            RedoFiguresCount++;
            RedoFile.Flush();
            UndoFile.Flush();
        }

        private void Redo()
        {
            PB.Image = null;
            if (Graph != null)
            {
                Graph.Clear(Color.White);
            }
            RedoFile.Position = 0;
            for (int i = 0; i < RedoFiguresCount; i++)
            {
                SpecifiedFigure = (Figure)Formatter.Deserialize(RedoFile);
            }
            Formatter.Serialize(UndoFile, SpecifiedFigure);
            RedoFiguresCount--;
            UndoFiguresCount++;
            UndoFile.Position = 0;
            for (int i = 0; i < UndoFiguresCount; i++)
            {
                SpecifiedFigure = (Figure)Formatter.Deserialize(UndoFile);
                Pen = new Pen(SpecifiedFigure.PenColor, SpecifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
            RedoFile.Flush();
            UndoFile.Flush();
        }

        private void form_graphic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26 && (UndoFiguresCount > 0))//ctrl+z
            {
                Undo();
                Graph = Graphics.FromImage(Bmp);
                PB.Image = Bmp;
            }
            if (e.KeyChar == 25 && (RedoFiguresCount > 0))//ctrl+y
            {
                Redo();
                Graph = Graphics.FromImage(Bmp);
                PB.Image = Bmp;
            }
        }

        private void form_graphic_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                SpecifiedFigure.DrawMode = DrawMode.none;
        }

        private void form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                SpecifiedFigure.DrawMode = DrawMode.shift;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                restoreBmp();
                SpecifiedFigure.PointFs[1] = e.Location;
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

    }
}
