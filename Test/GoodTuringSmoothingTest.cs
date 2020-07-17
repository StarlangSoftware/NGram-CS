using NGram;
using NUnit.Framework;

namespace Test
{
    public class GoodTuringSmoothingTest : SimpleSmoothingTest
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            SimpleSmoothing<string> simpleSmoothing = new GoodTuringSmoothing<string>();
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
            Assert.AreEqual(14.500734, simpleUniGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(2.762526, simpleBiGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(3.685001, simpleTriGram.GetPerplexity(simpleCorpus), 0.0001);
        }

        [Test]
        public void TestPerplexityComplex()
        {
            Assert.AreEqual(1290.97916, complexUniGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(8331.518540, complexBiGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(39184.430078, complexTriGram.GetPerplexity(testCorpus), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesSimple()
        {
            Assert.AreEqual(0.116607, simpleUniGram.GetProbability("<s>"), 0.0001);
            Assert.AreEqual(0.149464, simpleUniGram.GetProbability("mahmut"), 0.0001);
            Assert.AreEqual(0.026599, simpleUniGram.GetProbability("kitabı"), 0.0001);
            Assert.AreEqual(0.492147, simpleBiGram.GetProbability("<s>", "ali"), 0.0001);
            Assert.AreEqual(0.030523, simpleBiGram.GetProbability("ayşe", "ali"), 0.0001);
            Assert.AreEqual(0.0625, simpleBiGram.GetProbability("mahmut", "ali"), 0.0001);
            Assert.AreEqual(0.323281, simpleBiGram.GetProbability("at", "mehmet"), 0.0001);
            Assert.AreEqual(0.049190, simpleTriGram.GetProbability("<s>", "ali", "top"), 0.0001);
            Assert.AreEqual(0.043874, simpleTriGram.GetProbability("ayşe", "kitabı", "at"), 0.0001);
            Assert.AreEqual(0.0625, simpleTriGram.GetProbability("ayşe", "topu", "at"), 0.0001);
            Assert.AreEqual(0.0625, simpleTriGram.GetProbability("mahmut", "evde", "kal"), 0.0001);
            Assert.AreEqual(0.261463, simpleTriGram.GetProbability("ali", "topu", "at"), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesComplex()
        {
            Assert.AreEqual(0.050745, complexUniGram.GetProbability("<s>"), 0.0001);
            Assert.AreEqual(0.000126, complexUniGram.GetProbability("atatürk"), 0.0001);
            Assert.AreEqual(0.000497, complexBiGram.GetProbability("<s>", "mustafa"), 0.0001);
            Assert.AreEqual(0.014000, complexBiGram.GetProbability("mustafa", "kemal"), 0.0001);
            Assert.AreEqual(0.061028, complexTriGram.GetProbability("<s>", "mustafa", "kemal"), 0.0001);
            Assert.AreEqual(0.283532, complexTriGram.GetProbability("mustafa", "kemal", "atatürk"), 0.0001);
        }
    }
}