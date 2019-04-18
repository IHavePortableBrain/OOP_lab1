using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace NetLib
{
    public static class NetOps
    {
        public const int SignalError = 6;
        public const int SignalCommit = 100;
        public const int SignalPull = 505;
        public const int SignalDisconnect = 5051;
        public const int SignalOK = 0;
        public const int BUFSIZ = 1024 * 1024;

        public static void ReceiveFile(Socket receiver, ref string fileName)
        {
            if (GetAcknowlegment(receiver) == SignalOK)
            {
                byte[] buf = new byte[BUFSIZ];

                int receivedBytesLen = receiver.Receive(buf);
                int fileNameLen = BitConverter.ToInt32(buf, 0);
                if (fileName == null)
                    fileName = Encoding.Unicode.GetString(buf, sizeof(int), fileNameLen);
                int fileContentLen = BitConverter.ToInt32(buf, sizeof(int) + fileNameLen);

                buf = new byte[fileContentLen];

                if (true) //some conditions when further data getting is soficient
                {
                    SendAcknowledgement(SignalOK, receiver);
                    BinaryWriter bWriter = new BinaryWriter(File.Open(fileName, FileMode.Create));
                    if (fileContentLen > 0)
                    {
                        receivedBytesLen = receiver.Receive(buf);
                        if (fileContentLen != receivedBytesLen)
                            throw new Exception("Miss file content");
                        bWriter.Write(buf, 0, receivedBytesLen);
                    }
                    bWriter.Close();
                }
            }
            else
                throw new Exception();
        }
        
        public static void SendFile(string filePath, Socket sender)
        {
            byte[] fileNameBytes = Encoding.Unicode.GetBytes(Path.GetFileName(filePath));
            byte[] fileNameLenBytes = BitConverter.GetBytes(fileNameBytes.Length);
            byte[] fileContent = File.ReadAllBytes(filePath);
            byte[] fileContentLen = BitConverter.GetBytes(fileContent.Length);
            byte[] sendBuf = new byte[fileContent.Length | (sizeof(int) + fileNameBytes.Length + sizeof(int))];//Marshal.SizeOf(filePath.Length)

            fileNameLenBytes.CopyTo(sendBuf, 0);
            fileNameBytes.CopyTo(sendBuf, sizeof(int));
            fileContentLen.CopyTo(sendBuf, sizeof(int) + fileNameBytes.Length);

            SendAcknowledgement(SignalOK, sender);
            sender.Send(sendBuf, sizeof(int) + fileNameBytes.Length + sizeof(int), SocketFlags.None);
            if (GetAcknowlegment(sender) == SignalOK)
            {
                fileContent.CopyTo(sendBuf, 0);//отпрравляет сразу и служебную инфу и контент а клиент думает что это только служебная
                sender.Send(sendBuf, fileContent.Length, SocketFlags.None);
            }
        }

        public static void SendAcknowledgement(int ACK, Socket sender)
        {
            sender.Send(BitConverter.GetBytes(ACK));
        }

        public static void SendReply(string reply, Socket receiver)
        {
            byte[] msg = Encoding.UTF8.GetBytes(reply);
            receiver.Send(msg);
        }

        public static int GetAcknowlegment(Socket receiver)
        {
            byte[] buf = new byte[sizeof(int)];
            receiver.Receive(buf, sizeof(int), SocketFlags.None);
            return BitConverter.ToInt32(buf, 0);
        }

        public static string RecieveExceptionMessage(Socket receiver)
        {
            byte[] buf = new byte[BUFSIZ];
            int bytesRec = receiver.Receive(buf, BUFSIZ, SocketFlags.None);
            return Encoding.UTF8.GetString(buf, 0, bytesRec);
        }
    }
}