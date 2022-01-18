using System.IO;

namespace Chess
{
    class DataManager 
    {
        private string Path;

        public DataManager(string dataPath)
        {
            Path = dataPath;
            FileInfo dataFile = new FileInfo(Path);

            if (!dataFile.Exists) {
                File.Create(Path);
            }
        }        

        public void Write(string text)
        {
            using (StreamWriter SW = new StreamWriter(Path)) {
                SW.Write(text);
            }
        }

        public void WriteLine(string text)
        {
            using (StreamWriter SW = new StreamWriter(Path))
            {
                SW.WriteLine(text);
            }
        }

        public string Read()
        {
            using (StreamReader SR = new StreamReader(Path))
            {
                return SR.ReadToEnd();
            }                       
        }

        public string ReadHead()
        {
            using (StreamReader SR = new StreamReader(Path))
            {
                return SR.ReadLine();
            }
        } 



    }
}
