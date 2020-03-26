namespace NGram
{
    public class LaplaceSmoothing<TSymbol> : SimpleSmoothing<TSymbol>
    {
        private readonly double _delta;

        public LaplaceSmoothing()
        {
            _delta = 1.0;
        }

        public LaplaceSmoothing(double delta)
        {
            this._delta = delta;
        }

        /**
         * <summary>Wrapper function to set the N-gram probabilities with laplace smoothing.</summary>
         *
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         */
        public override void SetProbabilities(NGram<TSymbol> nGram, int level)
        {
            nGram.SetProbabilityWithPseudoCount(_delta, level);
        }
    }
}