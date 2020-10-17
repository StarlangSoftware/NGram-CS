using System.IO;

namespace NGram
{
    public class MultipleFile
    {
        private StreamReader streamReader;
        private int index;
        private readonly string[] fileNameList;

        public MultipleFile(params string[] fileNameList){
            index = 0;
            this.fileNameList = fileNameList;
            streamReader = new StreamReader(fileNameList[index]);
        }

        public void Close(){
            streamReader.Close();
        }

        public string ReadLine(){
            var tmpLine = streamReader.ReadLine();
            if (tmpLine != null){
                return tmpLine;
            }
            streamReader.Close();
            index++;
            streamReader = new StreamReader(fileNameList[index]);
            return streamReader.ReadLine();
        }

        public StreamReader GetStreamReader(){
            return streamReader;
        }
        
    }
}