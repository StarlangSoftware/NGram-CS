using System.Collections.Generic;
using NGram;
using NUnit.Framework;

namespace Test
{
    public class SimpleSmoothingTest : CorpusTest
    {
        protected List<List<string>> simpleCorpus, trainCorpus, testCorpus, validationCorpus;
        protected NGram<string> simpleUniGram, simpleBiGram, simpleTriGram;
        protected NGram<string> complexUniGram, complexBiGram, complexTriGram;

        [SetUp]
        public void SetUp()
        {
            var text1 = new List<string> {"<s>", "ali", "topu", "at", "mehmet", "ayşeye", "gitti", "</s>"};
            var text2 = new List<string> {"<s>", "ali", "top", "at", "ayşe", "eve", "gitti", "</s>"};
            var text3 = new List<string> {"<s>", "ayşe", "kitabı", "ver", "</s>"};
            var text4 = new List<string> {"<s>", "ali", "topu", "mehmete", "at", "</s>"};
            var text5 = new List<string> {"<s>", "ali", "topu", "at", "mehmet", "ayşeyle", "gitti", "</s>"};
            simpleCorpus = new List<List<string>> {text1, text2, text3, text4, text5};

            simpleUniGram = new NGram<string>(simpleCorpus, 1);
            simpleBiGram = new NGram<string>(simpleCorpus, 2);
            simpleTriGram = new NGram<string>(simpleCorpus, 3);
            trainCorpus = ReadCorpus("../../../train.txt");
            complexUniGram = new NGram<string>(trainCorpus, 1);
            complexBiGram = new NGram<string>(trainCorpus, 2);
            complexTriGram = new NGram<string>(trainCorpus, 3);
            testCorpus = ReadCorpus("../../../test.txt");
            validationCorpus = ReadCorpus("../../../validation.txt");
        }
    }
}