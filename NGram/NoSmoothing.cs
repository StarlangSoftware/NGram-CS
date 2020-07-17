namespace NGram
{
    public class NoSmoothing<TSymbol> : SimpleSmoothing<TSymbol>
    {
        public override void SetProbabilities(NGram<TSymbol> nGram, int level)
        {
            nGram.SetProbabilityWithPseudoCount(0.0, level);
        }
    }
}