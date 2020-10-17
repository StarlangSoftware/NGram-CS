using System.Collections.Generic;
using NGram;
using NUnit.Framework;

namespace Test
{
    public class NGramTest : CorpusTest
    {
        List<List<string>> simpleCorpus, trainCorpus, testCorpus, validationCorpus;
        NGram<string> simpleUniGram, simpleBiGram, simpleTriGram;
        NGram<string> complexUniGram, complexBiGram, complexTriGram;

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

        [Test]
        public void TestGetCountSimple()
        {
            Assert.AreEqual(5, simpleUniGram.GetCount(new [] {"<s>"}), 0.0);
            Assert.AreEqual(0, simpleUniGram.GetCount(new [] {"mahmut"}), 0.0);
            Assert.AreEqual(1, simpleUniGram.GetCount(new [] {"kitabı"}), 0.0);
            Assert.AreEqual(4, simpleBiGram.GetCount(new [] {"<s>", "ali"}), 0.0);
            Assert.AreEqual(0, simpleBiGram.GetCount(new [] {"ayşe", "ali"}), 0.0);
            Assert.AreEqual(0, simpleBiGram.GetCount(new [] {"mahmut", "ali"}), 0.0);
            Assert.AreEqual(2, simpleBiGram.GetCount(new [] {"at", "mehmet"}), 0.0);
            Assert.AreEqual(1, simpleTriGram.GetCount(new [] {"<s>", "ali", "top"}), 0.0);
            Assert.AreEqual(0, simpleTriGram.GetCount(new [] {"ayşe", "kitabı", "at"}), 0.0);
            Assert.AreEqual(0, simpleTriGram.GetCount(new [] {"ayşe", "topu", "at"}), 0.0);
            Assert.AreEqual(0, simpleTriGram.GetCount(new [] {"mahmut", "evde", "kal"}), 0.0);
            Assert.AreEqual(2, simpleTriGram.GetCount(new [] {"ali", "topu", "at"}), 0.0);
        }

        [Test]
        public void TestGetCountComplex()
        {
            Assert.AreEqual(20000, complexUniGram.GetCount(new [] {"<s>"}), 0.0);
            Assert.AreEqual(50, complexUniGram.GetCount(new [] {"atatürk"}), 0.0);
            Assert.AreEqual(11, complexBiGram.GetCount(new [] {"<s>", "mustafa"}), 0.0);
            Assert.AreEqual(3, complexBiGram.GetCount(new [] {"mustafa", "kemal"}), 0.0);
            Assert.AreEqual(1, complexTriGram.GetCount(new [] {"<s>", "mustafa", "kemal"}), 0.0);
            Assert.AreEqual(1, complexTriGram.GetCount(new [] {"mustafa", "kemal", "atatürk"}), 0.0);
        }

        [Test]
        public void TestVocabularySizeSimple()
        {
            Assert.AreEqual(15, simpleUniGram.VocabularySize(), 0.0);
        }

        [Test]
        public void TestVocabularySizeComplex()
        {
            Assert.AreEqual(57625, complexUniGram.VocabularySize(), 0.0);
            complexUniGram = new NGram<string>(testCorpus, 1);
            Assert.AreEqual(55485, complexUniGram.VocabularySize(), 0.0);
            complexUniGram = new NGram<string>(validationCorpus, 1);
            Assert.AreEqual(35663, complexUniGram.VocabularySize(), 0.0);
        }

        [Test]
        public void TestSaveAsText()
        {
            simpleUniGram.SaveAsText("simple1.txt");
            simpleBiGram.SaveAsText("simple2.txt");
            simpleTriGram.SaveAsText("simple3.txt");
        }
        
        [Test]
        public void TestLoadMultiPart(){
            simpleUniGram = new NGram<string>("../../../simple1part1.txt", "../../../simple1part2.txt");
            simpleBiGram = new NGram<string>("../../../simple2part1.txt", "../../../simple2part2.txt", "../../../simple2part3.txt");
            simpleTriGram = new NGram<string>("../../../simple3part1.txt", "../../../simple3part2.txt", "../../../simple3part3.txt", "../../../simple3part4.txt");
            TestGetCountSimple();
            TestVocabularySizeSimple();
        }

    }
}