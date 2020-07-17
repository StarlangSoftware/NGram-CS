using NGram;
using NUnit.Framework;

namespace Test
{
    public class NoSmoothingTest : SimpleSmoothingTest
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            SimpleSmoothing<string> simpleSmoothing = new NoSmoothing<string>();
            simpleUniGram.CalculateNGramProbabilities(simpleSmoothing);
            simpleBiGram.CalculateNGramProbabilities(simpleSmoothing);
            simpleTriGram.CalculateNGramProbabilities(simpleSmoothing);
            complexUniGram.CalculateNGramProbabilities(simpleSmoothing);
            complexBiGram.CalculateNGramProbabilities(simpleSmoothing);
            complexTriGram.CalculateNGramProbabilities(simpleSmoothing);
        }

        [Test]
        public void TestPerplexitySimple()
        {
            Assert.AreEqual(12.318362, simpleUniGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(1.573148, simpleBiGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(1.248330, simpleTriGram.GetPerplexity(simpleCorpus), 0.0001);
        }

        [Test]
        public void TestPerplexityComplex()
        {
            Assert.AreEqual(3220.299369, complexUniGram.GetPerplexity(trainCorpus), 0.0001);
            Assert.AreEqual(32.362912, complexBiGram.GetPerplexity(trainCorpus), 0.0001);
            Assert.AreEqual(2.025259, complexTriGram.GetPerplexity(trainCorpus), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesSimple()
        {
            Assert.AreEqual(5 / 35.0, simpleUniGram.GetProbability("<s>"), 0.0);
            Assert.AreEqual(0.0, simpleUniGram.GetProbability("mahmut"), 0.0);
            Assert.AreEqual(1.0 / 35.0, simpleUniGram.GetProbability("kitabı"), 0.0);
            Assert.AreEqual(4 / 5.0, simpleBiGram.GetProbability("<s>", "ali"), 0.0);
            Assert.AreEqual(0 / 2.0, simpleBiGram.GetProbability("ayşe", "ali"), 0.0);
            Assert.AreEqual(0.0, simpleBiGram.GetProbability("mahmut", "ali"), 0.0);
            Assert.AreEqual(2 / 4.0, simpleBiGram.GetProbability("at", "mehmet"), 0.0);
            Assert.AreEqual(1 / 4.0, simpleTriGram.GetProbability("<s>", "ali", "top"), 0.0);
            Assert.AreEqual(0 / 1.0, simpleTriGram.GetProbability("ayşe", "kitabı", "at"), 0.0);
            Assert.AreEqual(0.0, simpleTriGram.GetProbability("ayşe", "topu", "at"), 0.0);
            Assert.AreEqual(0.0, simpleTriGram.GetProbability("mahmut", "evde", "kal"), 0.0);
            Assert.AreEqual(2 / 3.0, simpleTriGram.GetProbability("ali", "topu", "at"), 0.0);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesComplex()
        {
            Assert.AreEqual(20000 / 376019.0, complexUniGram.GetProbability("<s>"), 0.0);
            Assert.AreEqual(50 / 376019.0, complexUniGram.GetProbability("atatürk"), 0.0);
            Assert.AreEqual(11 / 20000.0, complexBiGram.GetProbability("<s>", "mustafa"), 0.0);
            Assert.AreEqual(3 / 138.0, complexBiGram.GetProbability("mustafa", "kemal"), 0.0);
            Assert.AreEqual(1 / 11.0, complexTriGram.GetProbability("<s>", "mustafa", "kemal"), 0.0);
            Assert.AreEqual(1 / 3.0, complexTriGram.GetProbability("mustafa", "kemal", "atatürk"), 0.0);
        }
    }
}