using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using Figures;

// todo
//ДЕСЕРИАЛИЗАЦИЯ НЕ УДАЛЯЕТ СЕРИЮ ИЗ ФАЙЛА!!!
//2 длл.Одна для Figure остальные, вторая для остальных . Обращение к MyCurve нельзя в основном проекте
//forech po papke izvlech full path and load type
//5. баг если не отжимать лкм в пределах пб?
//9 redo и undo не меняет состояние панели выбранной фигуры
//11. баг тянущаяся курва сменить фигуру
//12.mouse leave event + ctrl z
//!!!!13. нормальное сохранение.
//4 лаба длл фигуры подгрузка.
// ask
//1. как свапать переменные закрываемые свойствами?(передать как ref)
//4.лайфхаки отладчика?
//5.отличие виртуального переопределения от простого перекрытия имени метода

namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        private Bitmap bmp, bmpStore;
        private Graphics graph;
        private Pen pen;
        private Figure specifiedFigure = null; //Figure dynamic

        private const string LibPath = "..\\..\\Lib";  //"..\\..\\Lib"; "D:\\! 4 сем\\ООТПИСП\\лабы\\lab1\\lab1_v2\\lab1_v2\\Lib\\FiguresLib.dll"
        private const string LibFiguresName = "FiguresLib.dll";
        private const string UndoFileName = "undo.dat";
        private const string RedoFileName = "redo.dat";
        private FileStream undoFile = new FileStream(UndoFileName, FileMode.Create);
        private FileStream redoFile = new FileStream(RedoFileName, FileMode.Create);
        private BinaryFormatter formatter = new BinaryFormatter();
        private UInt32 undoFiguresCount = 0;
        private UInt32 redoFiguresCount = 0;

        private enum State : int { draw, pending };
        private State state = State.pending;
        
        private void Form_graphic_Load(object sender, EventArgs e)
        {
            PB.Height = 1200;//костыль показать;
            PB.Width = 599;
            bmp = new Bitmap(PB.Height, PB.Width);
            graph = Graphics.FromImage(bmp);
            pen = new Pen(Figure.DefaultPenColor, Figure.DefaultPenWidth);
            if (FiguresListBox.Items.Count > 0)
                FiguresListBox.SelectedIndex = 0;//initial figure pick
        }

        public form_graphic()
        {
            InitializeComponent();
            LoadFigures();

        }

        static byte[] LoadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            return buffer;
        }

        //scan declared types and add to GUI those which can be drawed 
        private void LoadFigures()
        {
            #region trash
            //Assembly asm = Assembly.GetExecutingAssembly();

            //foreach (Type type in asm.GetTypes())
            //{
            //    if (((type.Namespace == "lab1_v2.Figures") || (type.Namespace == "Figures")) && (type.GetInterface("IGUIIcon") != null))
            //    {
            //        FiguresListBox.Items.Add(type);
            //    }
            //}

            //foreach (string filePath in Directory.GetFiles(LibPath)) {
            //asm = Assembly.LoadFrom(filePath); //"..\\..\\..\\Figures\\bin\\Debug\\FiguresLib.dll" 
            //AssemblyName[] arn = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

            //Assembly asm = Assembly.LoadFile(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName); //  LibFiguresName,  @"D:\! 4 сем\ООТПИСП\лабы\lab1\lab1_v2\lab1_v2\Lib\FiguresLib.dll"
            //Assembly asm = Assembly.LoadFrom(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName);
            //AppDomain.CurrentDomain.DefineDynamicAssembly(asm.GetName(), System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
            #endregion

            byte[] rawAssembly = LoadFile(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName);
            Assembly asm = AppDomain.CurrentDomain.Load(rawAssembly);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);


            foreach (Type type in asm.GetTypes())
            {
                if ((type.Namespace == "Figures") && (type.GetInterface("IGUIIcon") != null))
                {
                    FiguresListBox.Items.Add(type);
                }
            }

        }

        static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AppDomain domain = (AppDomain)sender;

            byte[] rawAssembly = LoadFile(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName);
            Assembly assembly = domain.Load(rawAssembly);

            return assembly;
        }

        //get specified parametrs of pen and figure and assign them
        private void GetFigureAndPen()
        {
            pen.Color = colorDialog.Color;
            pen.Width = (float)numericUpDown1.Value;
            if (FiguresListBox.SelectedIndex > -1)
            {
                Type type = FiguresListBox.SelectedItem as Type;
                specifiedFigure = (Figure)Activator.CreateInstance(type);//(Figure)Activator.CreateInstance(type) Convert.ChangeType(Activator.CreateInstance(type), type.BaseType);
                specifiedFigure.pointCount = 0;
            }
            else
            {
                throw new MissingMemberException();
            }
        }

        private void FiguresListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetFigureAndPen();
        }

        private void DrawSpecifiedFigure()
        {
            specifiedFigure.Modify(); //apply drawing mods
            specifiedFigure.Draw(graph, pen);
            PB.Image = bmp;
        }

        private void SerializeSpecifiedFigure()
        {
            formatter.Serialize(undoFile, specifiedFigure);
            undoFiguresCount++;
            undoFile.Flush();
        }

        private void ClearPaintBoxCanvas()
        {
            PB.Image = null;
            //if (graph != null)
                graph.Clear(Color.White);
        }

        private void ClearCanvasAndState()
        {
            state = State.pending;
            specifiedFigure.pointCount = 0;

            undoFile.Dispose();
            redoFile.Dispose();
            undoFile = new FileStream(UndoFileName, FileMode.Create);
            redoFile = new FileStream(RedoFileName, FileMode.Create);
            undoFiguresCount = 0;
            redoFiguresCount = 0;

            ClearPaintBoxCanvas();
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
                //save picture
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
                specifiedFigure.pointFs[specifiedFigure.pointCount++] = e.Location;
                DrawSpecifiedFigure();
                specifiedFigure.pointCount--;
            }
        }

        private void PB_MouseLeave(object sender, EventArgs e)
        {
            if (state == State.draw)
            {
                specifiedFigure.pointCount++;//prev point value from mouse move event steel is in array[pointCount + 1]
                StopDrawing();
            }
        }

        private void DrawAllUndoFiguresAndRefreshCount()
        {
            undoFiguresCount = 0;
            undoFile.Seek(0L, SeekOrigin.Begin);
            while (undoFile.Position < undoFile.Length)
            {
                specifiedFigure = (Figure)formatter.Deserialize(undoFile);
                undoFiguresCount++;
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
        }

        private void DrawUndoCountFigures()
        {
            
            for (int i = 0; i < undoFiguresCount; i++)//print undoFiguresCount - 1 serialized figures loop
            {
                specifiedFigure = (Figure)formatter.Deserialize(undoFile);
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
        }

        private void Undo()
        {
            state = State.pending;
            ClearPaintBoxCanvas();
            undoFile.Position = 0;
            undoFiguresCount--;
            DrawUndoCountFigures();
            long newLength = undoFile.Position;//move last serialized figure from undo to redo file
            formatter.Serialize(redoFile, formatter.Deserialize(undoFile));
            undoFile.SetLength(newLength);
            redoFiguresCount++;
            redoFile.Flush();
            undoFile.Flush();
        }

        private void Redo()
        {
            state = State.pending;
            ClearPaintBoxCanvas();
            redoFile.Position = 0;
            for (int i = 0; i < redoFiguresCount - 1; i++) //skipping n - 1 redo figures loop
            {
                specifiedFigure = (Figure)formatter.Deserialize(redoFile);
            }
            long newLength = redoFile.Position;//move last serialized figure from redo to undo file
            formatter.Serialize(undoFile, formatter.Deserialize(redoFile));
            redoFile.SetLength(newLength);
            undoFile.Flush();
            redoFiguresCount--;
            undoFiguresCount++;
            undoFile.Position = 0;
            DrawUndoCountFigures();
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
                    ClearPaintBoxCanvas();
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
                specifiedFigure.DrawMode = Figures.DrawMode.none;
        }

        private void Form_graphic_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
                specifiedFigure.DrawMode = Figures.DrawMode.shift;
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

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string saveUndoFiguresFile = saveFileDialog.FileName;
                FileStream saveUndoFiguresStream = new FileStream(saveUndoFiguresFile, FileMode.Create);
                undoFile.Seek(0L, SeekOrigin.Begin);
                undoFile.CopyTo(saveUndoFiguresStream);
                saveUndoFiguresFile = null;
                saveUndoFiguresStream.Dispose();
                MessageBox.Show(String.Format("Saved to {0}", saveFileDialog.FileName));
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string openUndoFiguresFile = openFileDialog.FileName;
                    FileStream copyUndoFiguresStream = new FileStream(openUndoFiguresFile, FileMode.Open);
                    copyUndoFiguresStream.Seek(0L, SeekOrigin.Begin);
                    ClearCanvasAndState();
                    copyUndoFiguresStream.CopyTo(undoFile);
                    openUndoFiguresFile = null;
                    copyUndoFiguresStream.Dispose();
                    DrawAllUndoFiguresAndRefreshCount();
                }
            }
            catch(System.Runtime.Serialization.SerializationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Invalid serialization file {0}", openFileDialog.FileName));
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearCanvasAndState();
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
    }
}