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
using lab1_v2.Figures;


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
//3. как зная обявленные классы
namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        Bitmap bmp, bmpStore;
        Graphics graph;
        Pen pen;
        Figure specifiedFigure = null;
        

        private const string UndoFileName = "ctrlz.dat";
        private const string RedoFileName = "ctrly.dat";
        FileStream undoFile = new FileStream(UndoFileName, FileMode.Create);
        FileStream redoFile = new FileStream(RedoFileName, FileMode.Create);
        BinaryFormatter formatter = new BinaryFormatter();
        UInt32 undoFiguresCount = 0;
        UInt32 redoFiguresCount = 0;

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
            specifiedFigure.pointCount = 0;
            pen.Color = colorDialog.Color;
            pen.Width = (float)numericUpDown1.Value;

            state = State.init;
            if (LVfigures.SelectedIndices.Count > 0)
            {
                enFig = (EnFig)LVfigures.SelectedIndices[0];

                switch (enFig)
                {
                    case EnFig.curve:
                        specifiedFigure = new MyCurve();
                        break;
                    case EnFig.ellipse:
                        specifiedFigure = new MyEllipse();
                        break;
                    case EnFig.line:
                        specifiedFigure = new MyLine();
                        break;
                    case EnFig.rect:
                        specifiedFigure = new MyRectangle();
                        break;
                }
            }
        }

        private void DrawSpecifiedFigure()
        {
            specifiedFigure.Modify();
            specifiedFigure.Draw(graph, pen);
            PB.Image = bmp;
        }

        private void LoadFigures()
        {
            
            LVfigures.Items.Insert(LVfigures.Items.Count, new ListViewItem());
        }

        private void form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать, why drawing metod is sepetated from class; попросить научить пользоваться отладчиком по памяти
            PB.Width = 599;
            bmp = new Bitmap(PB.Height, PB.Width);
            graph = Graphics.FromImage(bmp);
            specifiedFigure = new MyLine();
            pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
            LoadFigures();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            state = State.init;
            PB.Image = null;
            //BmpStore.Dispose(); 
            if (graph != null)
            {
                undoFile.Dispose();
                redoFile.Dispose();
                undoFile = new FileStream(UndoFileName, FileMode.Create);
                redoFile = new FileStream(RedoFileName, FileMode.Create);
                undoFiguresCount = 0;
                redoFiguresCount = 0;

                graph.Clear(Color.White);
                specifiedFigure.pointCount = 0;
            }
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == State.wait && enFig == EnFig.curve)
            {
                restoreBmp();
                if (specifiedFigure.pointCount == Figure.MaxPointCount)
                    specifiedFigure.pointCount = 0;
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;
                if (specifiedFigure.pointCount > 1)
                    DrawAndSerializeSpecifiedFigure();
                PB.Image = bmp;
            }
            else
            if (state == State.wait || state == State.init)
            {
                bmpStore = bmp.Clone(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;
                state = State.draw;
            }

        }

        private void DrawAndSerializeSpecifiedFigure()
        {
            DrawSpecifiedFigure();
            formatter.Serialize(undoFile, specifiedFigure);
            undoFiguresCount++;
            undoFile.Flush();
        }

        private void PB_MouseUp(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                specifiedFigure.pointFs[1] = e.Location;
                DrawAndSerializeSpecifiedFigure();
                if (enFig != EnFig.curve)
                {
                    specifiedFigure.pointCount = 0;
                    state = State.init;
                }
                else
                    state = State.wait;
            }

        }

        private void Undo()
        {
            PB.Image = null;
            if (graph != null)
            {
                graph.Clear(Color.White);
            }
            undoFile.Position = 0;
            undoFiguresCount--;
            for (int i = 0; i < undoFiguresCount; i++)
            {
                specifiedFigure = (Figure)formatter.Deserialize(undoFile);
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
            formatter.Serialize(redoFile, formatter.Deserialize(undoFile));
            redoFiguresCount++;
            redoFile.Flush();
            undoFile.Flush();
        }

        private void Redo()
        {
            PB.Image = null;
            if (graph != null)
            {
                graph.Clear(Color.White);
            }
            redoFile.Position = 0;
            for (int i = 0; i < redoFiguresCount; i++)
            {
                specifiedFigure = (Figure)formatter.Deserialize(redoFile);
            }
            formatter.Serialize(undoFile, specifiedFigure);
            redoFiguresCount--;
            undoFiguresCount++;
            undoFile.Position = 0;
            for (int i = 0; i < undoFiguresCount; i++)
            {
                specifiedFigure = (Figure)formatter.Deserialize(undoFile);
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
            redoFile.Flush();
            undoFile.Flush();
        }

        private void form_graphic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26 && (undoFiguresCount > 0))//ctrl+z
            {
                Undo();
                graph = Graphics.FromImage(bmp);
                PB.Image = bmp;
            }
            if (e.KeyChar == 25 && (redoFiguresCount > 0))//ctrl+y
            {
                Redo();
                graph = Graphics.FromImage(bmp);
                PB.Image = bmp;
            }
        }

        private void form_graphic_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                specifiedFigure.DrawMode = DrawMode.none;
        }

        private void form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                specifiedFigure.DrawMode = DrawMode.shift;
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                restoreBmp();
                specifiedFigure.pointFs[1] = e.Location;
                DrawSpecifiedFigure();
            }
        }

        //_____________________________________________________

        private void restoreBmp()
        {
            bmp.Dispose();
            bmp = bmpStore.Clone(new System.Drawing.Rectangle(0, 0, bmpStore.Width, bmpStore.Height), bmpStore.PixelFormat);
            graph.Dispose();
            graph = Graphics.FromImage(bmp);
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            pen.Color = colorDialog.Color;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                pen.Width = (float)numericUpDown1.Value;
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
                pen.Width = (float)numericUpDown1.Value;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
