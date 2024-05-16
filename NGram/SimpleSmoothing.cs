namespace NGram
{
    public abstract class SimpleSmoothing<TSymbol>
    {
        public abstract void SetProbabilities(NGram<TSymbol> nGram, int level);

        /// <summary>
        /// Calculates the N-Gram probabilities with simple smoothing.
        /// </summary>
        /// <param name="nGram">N-Gram for which simple smoothing calculation is done.</param>
        public void SetProbabilities(NGram<TSymbol> nGram){
            SetProbabilities(nGram, nGram.GetN());
        }
        
    }
}