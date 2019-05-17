using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using System.Collections.Generic;
using Figures;
using System.Xml.Linq;

using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using NetLib;

// todo
//ДЕСЕРИАЛИЗАЦИЯ НЕ УДАЛЯЕТ СЕРИЮ ИЗ ФАЙЛА!!!
//2 длл.Одна для Figure остальные, вторая для остальных . Обращение к MyCurve нельзя в основном проекте
//forech po papke izvlech full path and load type
//5. баг если не отжимать лкм в пределах пб?
//9 redo и undo не меняет состояние панели выбранной фигуры
//11. баг тянущаяся курва сменить фигуру
//12.mouse leave event + ctrl z
//4 to save exactly what user drawed add mouse position point to figure points on stop drawing button press
// ask
//1. как свапать переменные закрываемые свойствами?(передать как ref)
//4.лайфхаки отладчика?
//5.отличие виртуального переопределения от простого перекрытия имени метода
//6 pt=rotected хреново работает PenColor set чекай и конструктор Figure

namespace lab1_v2
{
    public partial class form_graphic : Form
    {
        private Bitmap bmp, bmpStore;
        private Graphics graph;
        private Pen pen;
        private Figure specifiedFigure = null; //Figure dynamic

        private const string XValuesName = "Values";
        private const string XPathValuesName = "PathValues";
        private const string CfgDirectory = "cfg";
        private const string CfgFileName = "config.xml";
        private const string XlibDirectoryName = "LibDirectory";
        private const string XlibFiguresName = "LibFiguresName";
        private const string XlibUserFiguresName = "LibUserFiguresName";
        private const string XundoFileName = "UndoFileName";
        private const string XredoFileName = "RedoFileName";
        private const string XreceivedFileName = "ReceivedFileName";
        private const string XpulledFileName = "PulledFileName";
        private const string XuserFiguresDirectoryName = "UserFiguresDirectory";
        private const string XserializationExtensionName = "SerializationExtension";

        private const string XSettingValuesName = "Settings";
        private const string XPBWidthName = "PBWidth";
        private const string XPBHeightName = "PBHeight";
        private const string XColorsName = "Colors";
        private const string XColorName = "Color";

        private string LibDirectory = null;//"..\\..\\Lib";  //"..\\..\\Lib"; "D:\\! 4 сем\\ООТПИСП\\лабы\\lab1\\lab1_v2\\lab1_v2\\Lib\\FiguresLib.dll"
        private string LibFiguresName = null;//"FiguresLib.dll";
        private string LibUserFiguresName = null;//"UserFigureLib.dll";
        private string UndoFileName = null;//"undo.dat";
        private string RedoFileName = null;//"redo.dat";
        private string ReceivedFileName = null;//"received.dat";
        private string PulledFileName = null;//"pulled.dat";
        private string UserFiguresDirectory = null;//"user figures";
        private string SerializationExtension = null;//".dat";
        private int PBWidth = 0;
        private int PBHeight = 0;

        private static Assembly figuresAsm;

        private BinaryFormatter formatter = new BinaryFormatter();
        private UInt32 undoFiguresCount = 0;
        private UInt32 redoFiguresCount = 0;

        Socket clientSocket;
        IPEndPoint serverIpEndPoint;

        private enum State : int { draw, pending };
        private State state = State.pending;
        
        private void Form_graphic_Load(object sender, EventArgs e)
        {
            Reload();
        }

        private void Reload()
        {
            new FileStream(UndoFileName, FileMode.Create).Dispose(); //Clear file
            new FileStream(RedoFileName, FileMode.Create).Dispose();
            PB.Width = PBWidth;
            PB.Height = PBHeight;
            bmp = new Bitmap(PB.Width, PB.Height);
            graph = Graphics.FromImage(bmp);
            pen = new Pen(Figure.DefaultPenColor, Figure.DefaultPenWidth);
            if (FiguresListBox.Items.Count > 0)
                FiguresListBox.SelectedIndex = 0;//initial figure pick
            if (UserFiguresListBox.Items.Count > 0)
                UserFiguresListBox.SelectedIndex = 0;//initial figure pick
        }

        public form_graphic()
        {
            InitializeComponent();
            string cfgPath = CfgDirectory + "\\" + CfgFileName;
            bool isOk = true;
            do
                try
                {
                    if (!isOk && openFileDialog.ShowDialog() == DialogResult.OK)
                        cfgPath = openFileDialog.FileName;
                    //InitCfgs(cfgPath);
                    LoadCfgs(cfgPath);
                    LoadFigures();
                    isOk = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error occured: {0}. Reload configs.", ex.Message));
                    isOk = false;
                }
            while (!isOk);
            
        }

        private void InitCfgs(string cfgFilePath)
        {
            XDocument xdoc = new XDocument(
                new XElement(XValuesName,
                    new XElement(XPathValuesName,
                        new XElement(XlibDirectoryName, "..\\..\\Lib"),
                        new XElement(XlibFiguresName, "FiguresLib.dll"),
                        new XElement(XlibUserFiguresName, "UserFigureLib.dll"),
                        new XElement(XundoFileName, "undo.dat"),
                        new XElement(XredoFileName, "redo.dat"),
                        new XElement(XlibFiguresName, "FiguresLib.dll"),
                        new XElement(XreceivedFileName, "received.dat"),
                        new XElement(XpulledFileName, "pulled.dat"),
                        new XElement(XuserFiguresDirectoryName, "user figures"),
                        new XElement(XserializationExtensionName, ".dat")),
                    new XElement(XSettingValuesName,
                        new XElement(XPBHeightName, "550"),
                        new XElement(XPBWidthName, "1000")),
                        new XElement(XColorsName,
                            new XElement(XColorName, "0"))));
            xdoc.Save(cfgFilePath); 
        }

        private void SaveCfgs(string cfgFilePath)
        {
            XDocument xdoc = XDocument.Load(cfgFilePath);
            XElement xRoot = xdoc.Element(XValuesName);
            XElement xColors = xRoot.Element(XColorsName);
            xColors.RemoveAll();
            foreach (int color in colorDialog.CustomColors)
            {
                xColors.Add(new XElement(XColorName, color.ToString()));
            }
            xRoot.Element(XSettingValuesName).Element(XPBHeightName).Value = PB.Height.ToString();
            xRoot.Element(XSettingValuesName).Element(XPBWidthName).Value = PB.Width.ToString();
            xdoc.Save(cfgFilePath);
        }

        private void LoadCfgs(string cfgFilePath)
        {
            XDocument xdoc = XDocument.Load(cfgFilePath);
            XElement xRoot = xdoc.Element(XValuesName);
            XElement xPathNode = xRoot.Element(XPathValuesName);
            LibDirectory = xPathNode.Element(XlibDirectoryName).Value;
            LibFiguresName = xPathNode.Element(XlibFiguresName).Value;
            LibUserFiguresName = xPathNode.Element(XlibUserFiguresName).Value;
            UndoFileName = xPathNode.Element(XundoFileName).Value;
            RedoFileName = xPathNode.Element(XredoFileName).Value;
            ReceivedFileName = xPathNode.Element(XreceivedFileName).Value;
            PulledFileName = xPathNode.Element(XpulledFileName).Value;
            UserFiguresDirectory = xPathNode.Element(XuserFiguresDirectoryName).Value;
            SerializationExtension = xPathNode.Element(XserializationExtensionName).Value;

            int colorPos = 0;
            int[] colorTmp = new int[colorDialog.CustomColors.Length];  
            foreach (XElement xColor in xRoot.Element(XColorsName).Elements(XColorName))
            {
                colorTmp[colorPos++] = int.Parse(xColor.Value);
                colorPos %= colorDialog.CustomColors.Length;
            }
            colorDialog.CustomColors = colorTmp;

            XElement xSettingNode = xRoot.Element(XSettingValuesName);
            PBHeight = int.Parse(xSettingNode.Element(XPBHeightName).Value);
            PBWidth = int.Parse(xSettingNode.Element(XPBWidthName).Value);
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

            byte[] rawAssembly = StdOps.LoadFile(Environment.CurrentDirectory + "\\" + LibDirectory + "\\"+ LibFiguresName);
            figuresAsm = AppDomain.CurrentDomain.Load(rawAssembly);
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolve);

            //figuresAsm = Assembly.LoadFrom(Path.GetFullPath(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName));
            //AppDomain.CurrentDomain.Load(figuresAsm.GetName());//figuresAsm.FullName

            FiguresListBox.Items.Clear();
            foreach (Type type in figuresAsm.GetTypes())
            {
                if ((type.Namespace == "Figures") && (type.GetInterface("IGUIIcon") != null))
                {
                    FiguresListBox.Items.Add(type);
                }
            }
            
            RefreshUserList();
        }

        public static Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //AppDomain domain = (AppDomain)sender;

            //byte[] rawAssembly = StdOps.LoadFile(Environment.CurrentDirectory + @"\..\..\Lib\" + LibFiguresName);
            //Assembly assembly = domain.Load(rawAssembly);
 
            return figuresAsm;//assembly figuresAsm
        }

        //get specified parametrs of pen and figure and assign them
        private void GetFigureAndPen()
        {
            pen.Color = colorDialog.Color;
            pen.Width = (float)numericPenWidth.Value;
            if (FiguresListBox.SelectedIndex > -1)
            {
                Type type = FiguresListBox.SelectedItem as Type;
                specifiedFigure = (Figure)Activator.CreateInstance(type);//(Figure)Activator.CreateInstance(type) Convert.ChangeType(Activator.CreateInstance(type), type.BaseType);
                GetFetchableFigureFromFileIfItIs();

                specifiedFigure.pointCount = 0;
            }
            else
            {
                throw new MissingMemberException();
            }
        }

        private void FiguresListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFigureAndPen();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
        }

        private void UserFiguresListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                GetFetchableFigureFromFileIfItIs();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
        }

        private void DrawSpecifiedFigure()
        {
            specifiedFigure.Modify(); //apply drawing mods
            specifiedFigure.Draw(graph, pen);
            PB.Image = bmp;
        }
        
        private void SerializeSpecifiedFigure()
        {
            FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
            undoFile.Seek(0L, SeekOrigin.End);
            formatter.Serialize(undoFile, specifiedFigure);
            undoFiguresCount++;
            undoFile.Flush();
            undoFile.Dispose();
        }

        private void ClearPaintBoxCanvas(string initPictureFile = null)
        {
            PB.Image = null;
            //if (graph != null)
                graph.Clear(Color.White);
            if (initPictureFile != null && File.Exists(initPictureFile))
            {
                using (FileStream initPictureFileStream = new FileStream(initPictureFile, FileMode.Open))
                    DrawAllFileFiguresAndRefreshCount(initPictureFileStream, out uint dummy);
            }
        }

        private void ClearCanvasAndState(string initPictureFile = null)
        {
            state = State.pending;
            specifiedFigure.pointCount = 0;

            //undoFile.Dispose();
            //redoFile.Dispose();
            //undoFile = new FileStream(UndoFileName, FileMode.Create);
            //redoFile = new FileStream(RedoFileName, FileMode.Create);
            new FileStream(UndoFileName, FileMode.Create).Dispose();
            new FileStream(RedoFileName, FileMode.Create).Dispose();
            if (initPictureFile != PulledFileName)
                new FileStream(PulledFileName, FileMode.Create).Dispose();

            undoFiguresCount = 0;
            redoFiguresCount = 0;

            ClearPaintBoxCanvas(initPictureFile);
            //new FileStream(PulledFileName, FileMode.Create).Dispose();
        }

        private void DrawAndSerializeSpecifiedFigure()
        {
            DrawSpecifiedFigure();
            SerializeSpecifiedFigure();
        }

        private void StopDrawing()
        {
            specifiedFigure.pointCount++;//prev point value from mouse move event steel is in array[pointCount + 1]
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
                //GetFigureAndPen();
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
                StopDrawing();
            }
        }

        private void DrawAllFileFiguresAndRefreshCount(FileStream figureFile, out uint figureCount)
        {
            figureCount = 0;
            figureFile.Seek(0L, SeekOrigin.Begin);
            while (figureFile.Position < figureFile.Length)
            {
                specifiedFigure = (Figure)formatter.Deserialize(figureFile);
                figureCount++;
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
        }

        private void DrawCountFigures(FileStream file,uint count)
        {
            for (int i = 0; i < count; i++)//print undoFiguresCount - 1 serialized figures loop
            {
                specifiedFigure = (Figure)formatter.Deserialize(file);
                pen = new Pen(specifiedFigure.PenColor, specifiedFigure.PenWidth);
                DrawSpecifiedFigure();
            }
        }

        private void Undo()
        {
            state = State.pending;
            ClearPaintBoxCanvas(PulledFileName);
            FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
            FileStream redoFile = new FileStream(RedoFileName, FileMode.OpenOrCreate);
            redoFile.Seek(0, SeekOrigin.End);
            undoFiguresCount--;
            DrawCountFigures(undoFile, undoFiguresCount);
            long newLength = undoFile.Position;//move last serialized figure from undo to redo file
            formatter.Serialize(redoFile, formatter.Deserialize(undoFile));
            undoFile.SetLength(newLength);
            redoFiguresCount++;
            redoFile.Flush();
            undoFile.Flush();
            undoFile.Dispose();
            redoFile.Dispose();
        }

        private void Redo()
        {
            state = State.pending;
            ClearPaintBoxCanvas(PulledFileName);
            FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
            FileStream redoFile = new FileStream(RedoFileName, FileMode.OpenOrCreate);
            undoFile.Seek(0, SeekOrigin.End);
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
            DrawCountFigures(undoFile, undoFiguresCount);
            redoFile.Flush();
            undoFile.Flush();
            undoFile.Dispose();
            redoFile.Dispose();
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
                    //
                }
                GetFigureAndPen();
                specifiedFigure.pointCount = 0;//
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

        private void NumericPenWidth_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                pen.Width = (float)numericPenWidth.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
        }

        private void NumericPenWidth_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                pen.Width = (float)numericPenWidth.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
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
                //undoFile.Seek(0L, SeekOrigin.Begin);
                FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
                undoFile.CopyTo(saveUndoFiguresStream);
                saveUndoFiguresFile = null;
                saveUndoFiguresStream.Dispose();
                undoFile.Dispose();
                MessageBox.Show(String.Format("Saved to {0}", saveFileDialog.FileName));
            }
        }

        private void LoadConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    //SaveCfgs(CfgDirectory + "\\" + CfgFileName);
                    ClearCanvasAndState();
                    LoadCfgs(openFileDialog.FileName);
                    Reload();
                    LoadFigures();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error occured: {0}. Reload configs.", ex.Message));
                }
            }
        }

        private void SaveConfigsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    InitCfgs(openFileDialog.FileName);
                    SaveCfgs(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
                }
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            FileStream copyUndoFiguresStream = null;
            FileStream undoFile = null;
            string openUndoFiguresFile;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    openUndoFiguresFile = openFileDialog.FileName;
                    copyUndoFiguresStream = new FileStream(openUndoFiguresFile, FileMode.Open);
                    copyUndoFiguresStream.Seek(0L, SeekOrigin.Begin);
                    ClearCanvasAndState();
                    undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
                    copyUndoFiguresStream.CopyTo(undoFile);
                    DrawAllFileFiguresAndRefreshCount(undoFile,out undoFiguresCount);
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
            finally
            {
                openUndoFiguresFile = null;
                if (copyUndoFiguresStream != null)
                    copyUndoFiguresStream.Dispose();
                if (undoFile != null)
                    undoFile.Dispose();
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearCanvasAndState();
        }

        private void BtnAddUserFigure_Click(object sender, EventArgs e)
        {
            try
            {
                string userFigureFilePath = UserFiguresDirectory + "\\" + UserFigureNameTextBox.Text + SerializationExtension;
                Type userFigureType = figuresAsm.GetType("Figures.MyUserFigure");
                MethodInfo methodInfo = userFigureType.GetMethod("SaveUserFigure");
                string[] argv = new string[2] { UndoFileName, userFigureFilePath };
                methodInfo.Invoke(null, argv);
                //MyUserFigure.SaveUserFigure(UndoFileName, userFigureFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
        }

        private void GetFetchableFigureFromFileIfItIs()
        {
            if (specifiedFigure is IFetchDependent && UserFiguresListBox.SelectedIndex != -1)//specifiedFetchDependentFigure
            {
                using (FileStream figureFileStream = new FileStream(((FileInfo)UserFiguresListBox.SelectedItem).FullName, FileMode.Open))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    figureFileStream.Seek(0L, SeekOrigin.Begin);
                    Type userFigureType = figuresAsm.GetType("Figures.MyUserFigure");
                    specifiedFigure = formatter.Deserialize(figureFileStream) as Figure;
                }
                //specifiedFetchDependentFigure.FetchFigures(userFigureElements);
            }
        }

        private void BtnRefreshUserList_Click(object sender, EventArgs e)
        {
            try
            {
                RefreshUserList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Error occured: {0}.", ex.Message));
            }
        }

        private void RefreshUserList()
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(UserFiguresDirectory);
            UserFiguresListBox.Items.Clear();
            foreach (FileInfo userFigureFile in directoryInfo.GetFiles("*" + SerializationExtension))
                UserFiguresListBox.Items.Add(userFigureFile);
        }



        private void BtnStartHosting_Click(object sender, EventArgs e)
        {
            try
            {
                if (numericPort.Validate())
                {
                    Task.Run(() => ServerRoutine(Convert.ToInt16(numericPort.Value)));
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {

            }

        }

        private void BtnPull_Click(object sender, EventArgs e)
        {
            FileStream receivedFileStream = null;
            FileStream undoFile = null;
            try
            {
                NetOps.SendAcknowledgement(NetOps.SignalPull, clientSocket);
                string pulledFileName = PulledFileName;
                NetOps.ReceiveFile(clientSocket, ref pulledFileName);
                //pulledFileName = new FileStream(pulledFileName, FileMode.Open);
                //undoFile = new FileStream(UndoFileName, FileMode.Create); //OpenOrCreate if pull dont delete client work
                //CopyStream(undoFile, receivedFileStream);
                ClearCanvasAndState(pulledFileName);
                //DrawAllFileFiguresAndRefreshCount(receivedFileStream, out undoFiguresCount);//может лучше рисовать все фигуры принитого файла и сериализовать их а не аппендить файл и рисовать полученнный?
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (receivedFileStream != null)
                    receivedFileStream.Dispose();
                if (undoFile != null)
                    undoFile.Dispose();
            }
        }

        private void BtnCommit_Click(object sender, EventArgs e)
        {
            try
            {
                NetOps.SendAcknowledgement(NetOps.SignalCommit, clientSocket);
                NetOps.SendFile(UndoFileName, clientSocket);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                ClientStart(IPAddress.Parse(maskedTextBox.Text), Convert.ToInt16(numericPort.Value));
            }
            catch  (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                NetOps.SendAcknowledgement(NetOps.SignalDisconnect, clientSocket);
                clientSocket.Disconnect(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClientStart(IPAddress serverIP,int ListenPort)
        {
            //todo if validate numeric and mask
            serverIpEndPoint = new IPEndPoint(serverIP, ListenPort);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(serverIpEndPoint);
        }

        private void ServerRoutine(int ListenPort)
        {
            try
            {
                IPEndPoint listenIPEndPoint = new IPEndPoint(IPAddress.Any, ListenPort);
                Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                listenSocket.Bind(listenIPEndPoint);
                listenSocket.Listen(10);
                MessageBox.Show(String.Format("Start hosting accepting {0} IP,{1} port", listenIPEndPoint.Address, listenIPEndPoint.Port));
                while (true)
                {
                    Socket clientSocket = listenSocket.Accept();
                    try
                    {
                        Task.Run(() => ServeOneClient(clientSocket));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        NetOps.SendReply(ex.Message, clientSocket);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ServeOneClient(Socket clientServeSocket)
        {
            bool isServeEnd = false;
            int clientSignal = NetOps.SignalDisconnect;
            byte[] buf = new byte[1024];

            IPEndPoint clientEP = (IPEndPoint)clientServeSocket.RemoteEndPoint;
            MessageBox.Show(String.Format("Connected with {0} at port {1}", clientEP.Address, clientEP.Port));
            while (!isServeEnd)
            {
                try
                {
                    clientSignal = NetOps.GetAcknowlegment(clientServeSocket);

                    switch (clientSignal)
                    {
                        case NetOps.SignalDisconnect:
                            isServeEnd = true;
                            MessageBox.Show(String.Format("Client ip {0} at port {1} disconnect", clientEP.Address, clientEP.Port));
                            clientServeSocket.Disconnect(false);
                            clientServeSocket.Dispose();
                            break;

                        case NetOps.SignalPull:
                            NetOps.SendFile(UndoFileName, clientServeSocket);
                            break;
                            
                        case NetOps.SignalCommit:
                            string receivedFileName = ReceivedFileName;
                            NetOps.ReceiveFile(clientServeSocket, ref receivedFileName);
                            CommitFile(receivedFileName);
                            FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
                            DrawAllFileFiguresAndRefreshCount(undoFile,out undoFiguresCount);
                            undoFile.Dispose();
                            break;

                        default:
                            NetOps.SendAcknowledgement(NetOps.SignalError, clientServeSocket);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    NetOps.SendAcknowledgement(NetOps.SignalError, clientServeSocket);
                    NetOps.SendReply(ex.Message, clientServeSocket);
                }
            }
        }

        private void CommitFile(string receivedFileName)
        {
            FileStream undoFile = new FileStream(UndoFileName, FileMode.OpenOrCreate);
            FileStream receivedFileStream = new FileStream(receivedFileName, FileMode.Open);
            StdOps.CopyStream(undoFile, receivedFileStream);
            receivedFileStream.Dispose();
            undoFile.Dispose();
        }

    }
}