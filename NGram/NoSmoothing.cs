namespace NGram
{
    public class NoSmoothing<TSymbol> : SimpleSmoothing<TSymbol>
    {
        /// <summary>
        /// Calculates the N-Gram probabilities with no smoothing
        /// </summary>
        /// <param name="nGram">N-Gram for which no smoothing is done.</param>
        /// <param name="level">Height of the NGram node.</param>
        public override void SetProbabilities(NGram<TSymbol> nGram, int level)
        {
            nGram.SetProbabilityWithPseudoCount(0.0, level);
        }
    }
}