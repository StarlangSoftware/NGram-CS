using System.Collections.Generic;

namespace NGram
{
    public abstract class TrainedSmoothing<TSymbol> : SimpleSmoothing<TSymbol>
    {
        protected abstract void LearnParameters(List<List<TSymbol>> corpus, int N);

        /**
         * <summary>Calculates new lower bound.</summary>
         * <param name="current">current value.</param>
         * <param name="currentLowerBound">current lower bound</param>
         * <param name="currentUpperBound">current upper bound</param>
         * <param name="numberOfParts">number of parts between lower and upper bound.</param>
         * <returns>new lower bound</returns>
         */
        protected double NewLowerBound(double current, double currentLowerBound, double currentUpperBound,
            int numberOfParts)
        {
            if (current != currentLowerBound)
            {
                return current - (currentUpperBound - currentLowerBound) / numberOfParts;
            }

            return current / numberOfParts;
        }

        /**
         * <summary>Calculates new upper bound.</summary>
         * <param name="current">current value.</param>
         * <param name="currentLowerBound">current lower bound</param>
         * <param name="currentUpperBound">current upper bound</param>
         * <param name="numberOfParts">number of parts between lower and upper bound.</param>
         * <returns>new upper bound</returns>
         */
        protected double NewUpperBound(double current, double currentLowerBound, double currentUpperBound,
            int numberOfParts)
        {
            if (current != currentUpperBound)
            {
                return current + (currentUpperBound - currentLowerBound) / numberOfParts;
            }

            return current * numberOfParts;
        }

        /**
         * <summary>Wrapper function to learn parameters of the smoothing method and set the N-gram probabilities.</summary>
         *
         * <param name="corpus">Train corpus used to optimize parameters of the smoothing method.</param>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         */
        public void Train(List<List<TSymbol>> corpus, NGram<TSymbol> nGram)
        {
            LearnParameters(corpus, nGram.GetN());
            SetProbabilities(nGram);
        }
    }
}