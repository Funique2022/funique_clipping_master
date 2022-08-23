using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ffmpeg_helper
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (StreamReader docs = new StreamReader("config.xml", Encoding.UTF8, true))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FileStructure));
                    FileStructure strcture = (FileStructure)serializer.Deserialize(docs);
                    Worker worker = new Worker(strcture);
                    worker.Run();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{ex.Message}\n{ex.Data}\n{ex.StackTrace}");
            }
        }
    }
}
