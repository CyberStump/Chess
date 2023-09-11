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

            if (!dataFile.Exists)
                throw new FileNotFoundException("DataManager(): file " + Path + " doesn't exists! ");
        }        


        public void Write(string text)
        {
            StreamWriter SW = new StreamWriter(Path);
            SW.Write(text);
        }


        public void WriteLine(string text)
        {
            StreamWriter SW = new StreamWriter(Path);
            SW.WriteLine(text);
        }


        public string Read()
        {
            StreamReader SR = new StreamReader(Path);
            return SR.ReadToEnd();
        }


        public string ReadHead()
        {
            StreamReader SR = new StreamReader(Path);
            return SR.ReadLine();
        } 



    }
}
