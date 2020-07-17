using NGram;
using NUnit.Framework;

namespace Test
{
    public class InterpolatedSmoothingTest : SimpleSmoothingTest
    {
        [SetUp]
        public void SetUp()
        {
            base.SetUp();
            var interpolatedSmoothing = new InterpolatedSmoothing<string>();
            complexBiGram.CalculateNGramProbabilities(validationCorpus, interpolatedSmoothing);
            complexTriGram.CalculateNGramProbabilities(validationCorpus, interpolatedSmoothing);
        }

        [Test]
        public void TestPerplexityComplex()
        {
            Assert.AreEqual(917.214864, complexBiGram.GetPerplexity(testCorpus), 0.0001);
            Assert.AreEqual(3000.451177, complexTriGram.GetPerplexity(testCorpus), 0.0001);
        }

        [Test]
        public void TestCalculateNGramProbabilitiesComplex()
        {
            Assert.AreEqual(0.000418, complexBiGram.GetProbability("<s>", "mustafa"), 0.0001);
            Assert.AreEqual(0.005555, complexBiGram.GetProbability("mustafa", "kemal"), 0.0001);
            Assert.AreEqual(0.014406, complexTriGram.GetProbability("<s>", "mustafa", "kemal"), 0.0001);
            Assert.AreEqual(0.058765, complexTriGram.GetProbability("mustafa", "kemal", "atatürk"), 0.0001);
        }
    }
}