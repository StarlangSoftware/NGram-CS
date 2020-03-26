using System;
using System.Collections.Generic;
using Sampling;

namespace NGram
{
    public class AdditiveSmoothing<TSymbol> : TrainedSmoothing<TSymbol>
    {
        /**
         * <summary>Additive pseudocount parameter used in Additive Smoothing. The parameter will be learned using 10-fold cross
         * validation.</summary>
         */
        private double _delta;

        /**
         * <summary>The algorithm tries to optimize the best delta for a given corpus. The algorithm uses perplexity on the validation
         * set as the optimization criterion.</summary>
         * <param name="nGrams">10 N-Grams learned for different folds of the corpus. nGrams[i] is the N-Gram trained with i'th train</param>
         *               fold of the corpus.
         * <param name="kFoldCrossValidation">Cross-validation data used in training and testing the N-grams.</param>
         * <param name="lowerBound">Initial lower bound for optimizing the best delta.</param>
         * <returns>Best delta optimized with k-fold crossvalidation.</returns>
         */
        private double LearnBestDelta(NGram<TSymbol>[] nGrams, KFoldCrossValidation<List<TSymbol>> kFoldCrossValidation,
            double lowerBound)
        {
            double bestPrevious = -1, upperBound = 1;
            var bestDelta = (lowerBound + upperBound) / 2;
            const int numberOfParts = 5;
            while (true)
            {
                var bestPerplexity = Double.MaxValue;
                for (var value = lowerBound; value <= upperBound; value += (upperBound - lowerBound) / numberOfParts)
                {
                    double perplexity = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        nGrams[i].SetProbabilityWithPseudoCount(value, nGrams[i].GetN());
                        perplexity += nGrams[i].GetPerplexity(kFoldCrossValidation.GetTestFold(i));
                    }

                    if (perplexity < bestPerplexity)
                    {
                        bestPerplexity = perplexity;
                        bestDelta = value;
                    }
                }

                lowerBound = NewLowerBound(bestDelta, lowerBound, upperBound, numberOfParts);
                upperBound = NewUpperBound(bestDelta, lowerBound, upperBound, numberOfParts);
                if (bestPrevious != -1)
                {
                    if (System.Math.Abs(bestPrevious - bestPerplexity) / bestPerplexity < 0.001)
                    {
                        break;
                    }
                }

                bestPrevious = bestPerplexity;
            }

            return bestDelta;
        }

        /**
         * <summary>Wrapper function to learn the parameter (delta) in additive smoothing. The function first creates K NGrams
         * with the train folds of the corpus. Then optimizes delta with respect to the test folds of the corpus</summary>
         * <param name="corpus">Train corpus used to optimize delta parameter</param>
         * <param name="n">N in N-Gram.</param>
         */
        protected override void LearnParameters(List<List<TSymbol>> corpus, int n)
        {
            const int k = 10;
            var nGrams = new NGram<TSymbol>[k];
            var kFoldCrossValidation =
                new KFoldCrossValidation<List<TSymbol>>(corpus, k, 0);
            for (var i = 0; i < k; i++)
            {
                nGrams[i] = new NGram<TSymbol>(kFoldCrossValidation.GetTrainFold(i), n);
            }

            _delta = LearnBestDelta(nGrams, kFoldCrossValidation, 0.1);
        }
        
        /**
         * <summary>Wrapper function to set the N-gram probabilities with additive smoothing.</summary>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         */
        public override void SetProbabilities(NGram<TSymbol> nGram, int level) {
            nGram.SetProbabilityWithPseudoCount(_delta, level);
        }

    }
}