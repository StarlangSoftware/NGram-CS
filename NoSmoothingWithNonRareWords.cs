using System.Collections.Generic;

namespace NGram
{
    public class NoSmoothingWithNonRareWords<Symbol> : NoSmoothing<Symbol>
    {
        private HashSet<Symbol> _dictionary;
        private readonly double _probability;

        /**
         * <summary>Constructor of {@link NoSmoothingWithNonRareWords}</summary>
         *
         * @param probability
         */
        public NoSmoothingWithNonRareWords(double probability){
            this._probability = probability;
        }

        /**
         * <summary>Wrapper function to set the N-gram probabilities with no smoothing and replacing unknown words not found in non rare words.</summary>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         *
         */
        public override void SetProbabilities(NGram<Symbol> nGram, int level) {
            _dictionary = nGram.ConstructDictionaryWithNonRareWords(level, _probability);
            nGram.ReplaceUnknownWords(_dictionary);
            base.SetProbabilities(nGram, level);
        }

    }
}