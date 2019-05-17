using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Figures
{
    //if u want to load picture with another persons user figures u must obtain them, otherwise exception will appear
    [Serializable]
    public sealed class MyUserFigure : Figure, IGUIIcon, IFetchDependent
    {
        private const string IconPath = "icons\\user.png";
        private const string LibUserFiguresName = "UserFigureLib.dll";
        private const string LibFiguresName = "FiguresLib.dll";

        public List<Figure> Elements { get; private set; }

        public MyUserFigure() : base()
        {
            Elements = new List<Figure>();
        }

        public override void Draw(Graphics graph, Pen pen)
        {
            base.Draw(graph, pen);
            //graph.Clip.Translate(pointFs[0].X, pointFs[0].Y);
            foreach (Figure element in Elements)
            {
                Figure changedElement = (Figure)element.Clone();

                for (int i = 0; i < changedElement.pointCount; i++)
                {
                    changedElement.pointFs[i].X = pointFs[0].X + element.pointFs[i].X * (pointFs[1].X - pointFs[0].X) / graph.VisibleClipBounds.Width;
                    changedElement.pointFs[i].Y = pointFs[0].Y + element.pointFs[i].Y * (pointFs[1].Y - pointFs[0].Y) / graph.VisibleClipBounds.Height;
                }
                changedElement.Draw(graph, new Pen(changedElement.PenColor, changedElement.PenWidth));
            }
             //graph.Clip.Translate(-pointFs[0].X, -pointFs[0].Y);
        }

        //if point count < minPointCount Sets pointcount to minPointCount
        public static int SaveUserFigure(string UndoFileName, string userFigureFilePath)
        {
            if (!File.Exists(Path.GetFullPath(UndoFileName)))
                return 1;
            BinaryFormatter formatter = new BinaryFormatter();
            MyUserFigure tempFigure = new MyUserFigure();
            //if (tempFigure.pointCount < MinDrawPointCount)
               // tempFigure.pointCount = MinDrawPointCount;
            using (FileStream undoFileStream = new FileStream(UndoFileName, FileMode.Open))
                while (undoFileStream.Position < undoFileStream.Length)
                    tempFigure.Elements.Add((Figure)formatter.Deserialize(undoFileStream));
            using (FileStream userFigureFileStream = new FileStream(userFigureFilePath, FileMode.Create))
                formatter.Serialize(userFigureFileStream, tempFigure);
            return 0;
        }

        public Icon GetIcon()
        {
            return new Icon(IconPath);
        }

        public void Fetch(List<Figure> figures)
        {
            Elements = figures;
        }

    }
}
