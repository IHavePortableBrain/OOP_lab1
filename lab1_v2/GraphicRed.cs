using lab1_v2.Figures;
using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

// todo
//5. баг если не отжимать лкм в пределах пб?
// 6. баг рисование курвой после очистки
//7. рисование при невыбранной фигуре
//9 redo и undo не меняет состояние панели выбранной фигуры
//10. не удалять фигуру после рисования а количество заданных точек менять
//11. баг тянущаяся курва сменить фигуру
// ask
//1. как свапать переменные закрываемые свойствами?(передать как ref)
//3. как зная обявленные классы
//4.лайфхаки отладчика?

namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        private Bitmap bmp, bmpStore;
        private Graphics graph;
        private Pen pen;
        private Figure specifiedFigure = null;

        private const string UndoFileName = "undo.dat";
        private const string RedoFileName = "redo.dat";
        private FileStream undoFile = new FileStream(UndoFileName, FileMode.Create);
        private FileStream redoFile = new FileStream(RedoFileName, FileMode.Create);
        private BinaryFormatter formatter = new BinaryFormatter();
        private UInt32 undoFiguresCount = 0;
        private UInt32 redoFiguresCount = 0;

        private enum State : int { draw, pending };
        private State state = State.pending;

        public form_graphic()
        {
            InitializeComponent();
            LoadFigures();
        }

        private void LVfigures_SelectedIndexChanged(object sender, EventArgs e)
        {
            //specifiedFigure.pointCount = 0;
            //pen.Color = colorDialog.Color;
            //pen.Width = (float)numericUpDown1.Value;

            //state = State.init;
            //if (LVfigures.SelectedIndices.Count > 0)
            //{
            //    enFig = (EnFig)LVfigures.SelectedIndices[0];

            //    switch (enFig)
            //    {
            //        case EnFig.curve:
            //            specifiedFigure = new MyCurve();
            //            break;
            //        case EnFig.ellipse:
            //            specifiedFigure = new MyEllipse();
            //            break;
            //        case EnFig.line:
            //            specifiedFigure = new MyLine();
            //            break;
            //        case EnFig.rect:
            //            specifiedFigure = new MyRectangle();
            //            break;
            //    }
            //}
        }

        private void GetFigureAndPen()
        {
            pen.Color = colorDialog.Color;
            pen.Width = (float)numericUpDown1.Value;
            if (FiguresListBox.SelectedIndex > -1)
            {
                Type type = FiguresListBox.SelectedItem as Type;
                specifiedFigure = (Figure)Activator.CreateInstance(type);
                specifiedFigure.pointCount = 0;
            }
        }

        private void FiguresListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetFigureAndPen();
        }

        private void DrawSpecifiedFigure()
        {
            specifiedFigure.Modify();
            specifiedFigure.Draw(graph, pen);
            PB.Image = bmp;
        }

        private void SerializeSpecifiedFigure()
        {
            formatter.Serialize(undoFile, specifiedFigure);
            undoFiguresCount++;
            undoFile.Flush();
        }

        private void LoadFigures()
        {
            Assembly asm = Assembly.GetExecutingAssembly();

            foreach (Type type in asm.GetTypes())
            {
                if ((type.Namespace == "lab1_v2.Figures") && (type.GetInterface("IGUIIcon") != null))
                {
                    FiguresListBox.Items.Add(type);
                }
            }
            //FiguresListBox.SelectedIndex = 0;
            //LVfigures.Items.Insert(LVfigures.Items.Count, new ListViewItem());
        }

        private void Form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать;
            PB.Width = 599;
            bmp = new Bitmap(PB.Height, PB.Width);
            graph = Graphics.FromImage(bmp);
            specifiedFigure = new MyLine();
            pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
        }

        private void ClearFormCanvas()
        {
            PB.Image = null;
            graph.Clear(Color.White);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            state = State.pending;
            specifiedFigure.pointCount = 0;

            undoFile.Dispose();
            redoFile.Dispose();
            undoFile = new FileStream(UndoFileName, FileMode.Create);
            redoFile = new FileStream(RedoFileName, FileMode.Create);
            undoFiguresCount = 0;
            redoFiguresCount = 0;

            ClearFormCanvas();
        }

        private void DrawAndSerializeSpecifiedFigure()
        {
            DrawSpecifiedFigure();
            SerializeSpecifiedFigure();
        }

        private void StopDrawing()
        {
            SerializeSpecifiedFigure();
            specifiedFigure.pointCount = 0;
            state = State.pending;
        }

        private void PB_MouseDown(object sender, MouseEventArgs e)
        {
            if (state == State.pending)
            {
                bmpStore = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), bmp.PixelFormat);
                GetFigureAndPen();
                state = State.draw;
            }

            if (specifiedFigure.pointCount < Figure.MaxPointCount - 1)
            {
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;
            }
            else
            {
                StopDrawing();
            }
            if (specifiedFigure.pointCount >= Figure.MinDrawPointCount)
            {
                RestoreBmp();
                DrawSpecifiedFigure();

                if (!(specifiedFigure is IManyPointFigure))
                {
                    StopDrawing();
                }
            }
        }

        private void PB_MouseMove(object sender, MouseEventArgs e)
        {
            if (state == State.draw)
            {
                RestoreBmp();
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;//для curve не прокатит
                DrawSpecifiedFigure();
                specifiedFigure.pointCount--;
            }
        }

        private void Undo()
        {
            state = State.pending;
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
            state = State.pending;
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

        private void Form_graphic_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 26)//ctrl+z
            {
                if (undoFiguresCount > 0)
                {
                    Undo();
                    graph = Graphics.FromImage(bmp);
                    PB.Image = bmp;
                }
                else
                {
                    ClearFormCanvas();
                    state = State.pending;
                    specifiedFigure.pointCount = 0;
                }
            }
            else
            if (e.KeyChar == 25 && (redoFiguresCount > 0))//ctrl+y
            {
                Redo();
                graph = Graphics.FromImage(bmp);
                PB.Image = bmp;
            }
        }

        private void Form_graphic_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                specifiedFigure.DrawMode = DrawMode.none;
        }

        private void Form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                specifiedFigure.DrawMode = DrawMode.shift;
            else
            if (e.KeyCode == Keys.Q)
                StopDrawing();
        }

        private void RestoreBmp()
        {
            bmp.Dispose();
            bmp = bmpStore.Clone(new Rectangle(0, 0, bmpStore.Width, bmpStore.Height), bmpStore.PixelFormat);
            graph.Dispose();
            graph = Graphics.FromImage(bmp);
        }

        private void BtnColor_Click(object sender, EventArgs e)
        {
            colorDialog.ShowDialog();
            pen.Color = colorDialog.Color;
        }

        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
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

        private void NumericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
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