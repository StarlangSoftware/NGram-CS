using System.Collections.Generic;

namespace NGram
{
    public class NoSmoothingWithDictionary<TSymbol> : NoSmoothing<TSymbol>
    {
        private readonly HashSet<TSymbol> _dictionary;

        /**
         * <summary>Constructor of {@link NoSmoothingWithDictionary}</summary>
         * <param name="dictionary">Dictionary to use in smoothing</param>
         */
        public NoSmoothingWithDictionary(HashSet<TSymbol> dictionary){
            this._dictionary = dictionary;
        }

        /**
         * <summary>Wrapper function to set the N-gram probabilities with no smoothing and replacing unknown words not found in {@link HashSet} the dictionary.</summary>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         */
        protected new void SetProbabilities(NGram<TSymbol> nGram, int level) {
            nGram.ReplaceUnknownWords(_dictionary);
            base.SetProbabilities(nGram, level);
        }

    }
}