using NGram;
using NUnit.Framework;

namespace Test
{
    public class LaplaceSmoothingTest : SimpleSmoothingTest
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            SimpleSmoothing<string> simpleSmoothing = new LaplaceSmoothing<string>();
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
            Assert.AreEqual(12.809502, simpleUniGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(6.914532, simpleBiGram.GetPerplexity(simpleCorpus), 0.0001);
            Assert.AreEqual(7.694528, simpleTriGram.GetPerplexity(simpleCorpus), 0.0001);
        }

        [Test]
        public void TestPerplexityComplex()
        {
            Assert.AreEqual(4085.763010, complexUniGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(24763.660225, complexBiGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(49579.187475, complexTriGram.GetPerplexity(testCorpus), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesSimple()
        {
            Assert.AreEqual((5 + 1) / (35 + simpleUniGram.VocabularySize() + 1), simpleUniGram.GetProbability("<s>"),
                0.0);
            Assert.AreEqual((0 + 1) / (35 + simpleUniGram.VocabularySize() + 1), simpleUniGram.GetProbability("mahmut"),
                0.0);
            Assert.AreEqual((1 + 1) / (35 + simpleUniGram.VocabularySize() + 1), simpleUniGram.GetProbability("kitabı"),
                0.0);
            Assert.AreEqual((4 + 1) / (5 + simpleBiGram.VocabularySize() + 1),
                simpleBiGram.GetProbability("<s>", "ali"),
                0.0);
            Assert.AreEqual((0 + 1) / (2 + simpleBiGram.VocabularySize() + 1),
                simpleBiGram.GetProbability("ayşe", "ali"),
                0.0);
            Assert.AreEqual(1 / (simpleBiGram.VocabularySize() + 1), simpleBiGram.GetProbability("mahmut", "ali"), 0.0);
            Assert.AreEqual((2 + 1) / (4 + simpleBiGram.VocabularySize() + 1),
                simpleBiGram.GetProbability("at", "mehmet"),
                0.0);
            Assert.AreEqual((1 + 1) / (4.0 + simpleTriGram.VocabularySize() + 1),
                simpleTriGram.GetProbability("<s>", "ali", "top"), 0.0);
            Assert.AreEqual((0 + 1) / (1.0 + simpleTriGram.VocabularySize() + 1),
                simpleTriGram.GetProbability("ayşe", "kitabı", "at"), 0.0);
            Assert.AreEqual(1 / (simpleTriGram.VocabularySize() + 1),
                simpleTriGram.GetProbability("ayşe", "topu", "at"),
                0.0);
            Assert.AreEqual(1 / (simpleTriGram.VocabularySize() + 1),
                simpleTriGram.GetProbability("mahmut", "evde", "kal"), 0.0);
            Assert.AreEqual((2 + 1) / (3.0 + simpleTriGram.VocabularySize() + 1),
                simpleTriGram.GetProbability("ali", "topu", "at"), 0.0);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesComplex()
        {
            Assert.AreEqual((20000 + 1) / (376019.0 + complexUniGram.VocabularySize() + 1),
                complexUniGram.GetProbability("<s>"), 0.0);
            Assert.AreEqual((50 + 1) / (376019.0 + complexUniGram.VocabularySize() + 1),
                complexUniGram.GetProbability("atatürk"), 0.0);
            Assert.AreEqual((11 + 1) / (20000.0 + complexBiGram.VocabularySize() + 1),
                complexBiGram.GetProbability("<s>", "mustafa"), 0.0);
            Assert.AreEqual((3 + 1) / (138.0 + complexBiGram.VocabularySize() + 1),
                complexBiGram.GetProbability("mustafa", "kemal"), 0.0);
            Assert.AreEqual((1 + 1) / (11.0 + complexTriGram.VocabularySize() + 1),
                complexTriGram.GetProbability("<s>", "mustafa", "kemal"), 0.0);
            Assert.AreEqual((1 + 1) / (3.0 + complexTriGram.VocabularySize() + 1),
                complexTriGram.GetProbability("mustafa", "kemal", "atatürk"), 0.0);
        }
    }
}