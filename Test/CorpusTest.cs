using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    public class CorpusTest
    {
        protected List<List<string>> ReadCorpus(string fileName){
            var corpus = new List<List<string>>();
            var input = new StreamReader(fileName);
            var line = input.ReadLine();
            while (line != null){
                var words = line.Split(" ");
                corpus.Add(words.ToList());
                line = input.ReadLine();
            }
            input.Close();
            return corpus;
        }

    }
}