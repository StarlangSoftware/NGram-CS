using System.IO;

namespace NGram
{
    public class MultipleFile
    {
        private StreamReader streamReader;
        private int index;
        private readonly string[] fileNameList;

        /// <summary>
        /// Constructor for MultipleFile class. Initializes the buffer reader with the first input file
        /// from the fileNameList. MultipleFile supports simple multipart file system, where a text file is divided
        /// into multiple files.
        /// </summary>
        /// <param name="fileNameList"></param>
        public MultipleFile(params string[] fileNameList){
            index = 0;
            this.fileNameList = fileNameList;
            streamReader = new StreamReader(fileNameList[index]);
        }

        /// <summary>
        /// Closes the buffer reader.
        /// </summary>
        public void Close(){
            streamReader.Close();
        }

        /// <summary>
        /// Reads a single line from the current file. If the end of file is reached for the current file,
        /// next file is opened and a single line from that file is read. If all files are read, the method
        /// returns null.
        /// </summary>
        /// <returns>Read line from the current file.</returns>
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

        /// <summary>
        /// Accessor for the buffered reader
        /// </summary>
        /// <returns>Buffered reader</returns>
        public StreamReader GetStreamReader(){
            return streamReader;
        }
        
    }
}