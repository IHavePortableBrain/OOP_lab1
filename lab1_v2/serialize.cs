using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace lab1_v2
{
    class Serialization
    {
        static public void Serialize()
        {
            TestSerialization Test1 = new TestSerialization();
            TestSerialization Test2 = new TestSerialization();
            Test1.Field1 = 42;
            Test1.Field2 = "Blad";
            Test2.Field1 = 4;
            Test2.Field2 = "ad";
            BinaryFormatter formatter = new BinaryFormatter();

            // создаем поток байт (бинарный файл) 
            using (FileStream fs = new FileStream("test.dat", FileMode.OpenOrCreate))
            {
                // сериализация (сохранение объекта в поток байт) 
                formatter.Serialize(fs, Test1);
            }

            Console.WriteLine("Before desir: " + Test2.ToString());
            // открываем поток байт (бинарный файл) 
            using (FileStream fs = new FileStream("test.dat", FileMode.Open))
            {
                // десериализация (создание объекта из потока байт) 
                Test2 = (TestSerialization)formatter.Deserialize(fs);
            }
            Console.WriteLine("After desir: " + Test2.ToString());
            Console.ReadLine();
        }
    }

    [Serializable()]
    class TestSerialization
    {
        public int Field1 { get; set; }
        public string Field2 { get; set; }
    }
}
