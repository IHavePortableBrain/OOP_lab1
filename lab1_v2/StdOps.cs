using System.IO;

namespace lab1_v2
{
    internal class StdOps
    {
        private const int BUFSIZ = 1024 * 1024;

        public static void Swap<T>(ref T first, ref T second)
        {
            T temp = first;
            first = second;
            second = temp;
        }

        public static void CopyStream(Stream destination, Stream source)
        {
            int count;
            byte[] buffer = new byte[BUFSIZ];
            destination.Seek(0L, SeekOrigin.End);
            while ((count = source.Read(buffer, 0, buffer.Length)) > 0)
                destination.Write(buffer, 0, count);
        }

        public static byte[] LoadFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            byte[] buffer = new byte[(int)fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            return buffer;
        }
    }
}