using NGram;
using NUnit.Framework;

namespace Test
{
    public class AdditiveSmoothingTest : SimpleSmoothingTest
    {
        double delta1, delta2, delta3;

        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            var additiveSmoothing = new AdditiveSmoothing<string>();
            complexUniGram.CalculateNGramProbabilities(validationCorpus, additiveSmoothing);
            delta1 = additiveSmoothing.GetDelta();
            complexBiGram.CalculateNGramProbabilities(validationCorpus, additiveSmoothing);
            delta2 = additiveSmoothing.GetDelta();
            complexTriGram.CalculateNGramProbabilities(validationCorpus, additiveSmoothing);
            delta3 = additiveSmoothing.GetDelta();
        }

        [Test]
        public void TestPerplexityComplex()
        {
            Assert.AreEqual(4043.947022, complexUniGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(9220.218871, complexBiGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(30695.701941, complexTriGram.GetPerplexity(testCorpus), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesComplex()
        {
            Assert.AreEqual((20000 + delta1) / (376019.0 + delta1 * (complexUniGram.VocabularySize() + 1)),
                complexUniGram.GetProbability("<s>"), 0.0);
            Assert.AreEqual((50 + delta1) / (376019.0 + delta1 * (complexUniGram.VocabularySize() + 1)),
                complexUniGram.GetProbability("atatürk"), 0.0);
            Assert.AreEqual((11 + delta2) / (20000.0 + delta2 * (complexBiGram.VocabularySize() + 1)),
                complexBiGram.GetProbability("<s>", "mustafa"), 0.0);
            Assert.AreEqual((3 + delta2) / (138.0 + delta2 * (complexBiGram.VocabularySize() + 1)),
                complexBiGram.GetProbability("mustafa", "kemal"), 0.0);
            Assert.AreEqual((1 + delta3) / (11.0 + delta3 * (complexTriGram.VocabularySize() + 1)),
                complexTriGram.GetProbability("<s>", "mustafa", "kemal"), 0.0);
            Assert.AreEqual((1 + delta3) / (3.0 + delta3 * (complexTriGram.VocabularySize() + 1)),
                complexTriGram.GetProbability("mustafa", "kemal", "atatürk"), 0.0);
        }
    }
}