using System.Collections.Generic;
using Sampling;

namespace NGram
{
    public class InterpolatedSmoothing<TSymbol> : TrainedSmoothing<TSymbol>
    {
        private double _lambda1, _lambda2;
        private readonly SimpleSmoothing<TSymbol> _simpleSmoothing;

        /**
         * <summary>No argument constructor of {@link InterpolatedSmoothing}</summary>
         */
        public InterpolatedSmoothing()
        {
            this._simpleSmoothing = new GoodTuringSmoothing<TSymbol>();
        }

        /**
         * <summary>Constructor of {@link InterpolatedSmoothing}</summary>
         * <param name="simpleSmoothing">smoothing method.</param>
         */
        public InterpolatedSmoothing(SimpleSmoothing<TSymbol> simpleSmoothing)
        {
            this._simpleSmoothing = simpleSmoothing;
        }

        /**
         * <summary>The algorithm tries to optimize the best lambda for a given corpus. The algorithm uses perplexity on the validation
         * set as the optimization criterion.</summary>
         *
         * <param name="nGrams">10 N-Grams learned for different folds of the corpus. nGrams[i] is the N-Gram trained with i'th train fold of the corpus.</param>
         * <param name="kFoldCrossValidation">Cross-validation data used in training and testing the N-grams.</param>
         * <param name="lowerBound">Initial lower bound for optimizing the best lambda.</param>
         * <returns> Best lambda optimized with k-fold crossvalidation.</returns>
         */
        private double LearnBestLambda(NGram<TSymbol>[] nGrams, KFoldCrossValidation<List<TSymbol>> kFoldCrossValidation,
            double lowerBound)
        {
            double bestPrevious = -1,
                upperBound = 0.999;
            var bestLambda = (lowerBound + upperBound) / 2;
            var numberOfParts = 5;
            var testFolds = new List<List<TSymbol>>[10];
            for (var i = 0; i < 10; i++)
            {
                testFolds[i] = kFoldCrossValidation.GetTestFold(i);
            }

            while (true)
            {
                var bestPerplexity = double.MaxValue;
                for (var value = lowerBound; value <= upperBound; value += (upperBound - lowerBound) / numberOfParts)
                {
                    double perplexity = 0;
                    for (var i = 0; i < 10; i++)
                    {
                        nGrams[i].SetLambda(value);
                        perplexity += nGrams[i].GetPerplexity(testFolds[i]);
                    }

                    if (perplexity < bestPerplexity)
                    {
                        bestPerplexity = perplexity;
                        bestLambda = value;
                    }
                }

                lowerBound = NewLowerBound(bestLambda, lowerBound, upperBound, numberOfParts);
                upperBound = NewUpperBound(bestLambda, lowerBound, upperBound, numberOfParts);
                if (bestPrevious != -1)
                {
                    if (System.Math.Abs(bestPrevious - bestPerplexity) / bestPerplexity < 0.001)
                    {
                        break;
                    }
                }

                bestPrevious = bestPerplexity;
            }

            return bestLambda;
        }

        /**
         * <summary>The algorithm tries to optimize the best lambdas (lambda1, lambda2) for a given corpus. The algorithm uses perplexity on the validation
         * set as the optimization criterion.</summary>
         *
         * <param name="nGrams">10 N-Grams learned for different folds of the corpus. nGrams[i] is the N-Gram trained with i'th train fold of the corpus.</param>
         * <param name="kFoldCrossValidation">Cross-validation data used in training and testing the N-grams.</param>
         * <param name="lowerBound1">Initial lower bound for optimizing the best lambda1.</param>
         * <param name="lowerBound2">Initial lower bound for optimizing the best lambda2.</param>
         */
        private double[] LearnBestLambdas(NGram<TSymbol>[] nGrams,
            KFoldCrossValidation<List<TSymbol>> kFoldCrossValidation,
            double lowerBound1, double lowerBound2)
        {
            double upperBound1 = 0.999,
                upperBound2 = 0.999,
                bestPrevious = -1;
            double bestLambda1 = (lowerBound1 + upperBound1) / 2,
                bestLambda2 = (lowerBound2 + upperBound2) / 2;
            var testFolds = new List<List<TSymbol>>[10];
            const int numberOfParts = 5;
            for (var i = 0; i < 10; i++)
            {
                testFolds[i] = kFoldCrossValidation.GetTestFold(i);
            }

            while (true)
            {
                var bestPerplexity = double.MaxValue;
                for (var value1 = lowerBound1;
                    value1 <= upperBound1;
                    value1 += (upperBound1 - lowerBound1) / numberOfParts)
                {
                    for (var value2 = lowerBound2;
                        value2 <= upperBound2 && value1 + value2 < 1;
                        value2 += (upperBound2 - lowerBound2) / numberOfParts)
                    {
                        double perplexity = 0;
                        for (var i = 0; i < 10; i++)
                        {
                            nGrams[i].SetLambda(value1, value2);
                            perplexity += nGrams[i].GetPerplexity(testFolds[i]);
                        }

                        if (perplexity < bestPerplexity)
                        {
                            bestPerplexity = perplexity;
                            bestLambda1 = value1;
                            bestLambda2 = value2;
                        }
                    }
                }

                lowerBound1 = NewLowerBound(bestLambda1, lowerBound1, upperBound1, numberOfParts);
                upperBound1 = NewUpperBound(bestLambda1, lowerBound1, upperBound1, numberOfParts);
                lowerBound2 = NewLowerBound(bestLambda2, lowerBound2, upperBound2, numberOfParts);
                upperBound2 = NewUpperBound(bestLambda2, lowerBound2, upperBound2, numberOfParts);
                if (bestPrevious != -1)
                {
                    if (System.Math.Abs(bestPrevious - bestPerplexity) / bestPerplexity < 0.001)
                    {
                        break;
                    }
                }

                bestPrevious = bestPerplexity;
            }

            return new[] {bestLambda1, bestLambda2};
        }

        /**
         * <summary>Wrapper function to learn the parameters (lambda1 and lambda2) in interpolated smoothing. The function first creates K NGrams
         * with the train folds of the corpus. Then optimizes lambdas with respect to the test folds of the corpus depending on given N.</summary>
         * <param name="corpus">Train corpus used to optimize lambda parameters</param>
         * <param name="n">N in N-Gram.</param>
         */
        protected override void LearnParameters(List<List<TSymbol>> corpus, int n)
        {
            if (n <= 1)
            {
                return;
            }

            var K = 10;
            var nGrams = new NGram<TSymbol>[K];
            var kFoldCrossValidation = new KFoldCrossValidation<List<TSymbol>>(corpus, K, 0);
            for (var i = 0; i < K; i++)
            {
                nGrams[i] = new NGram<TSymbol>(kFoldCrossValidation.GetTrainFold(i), n);
                for (var j = 2; j <= n; j++)
                {
                    nGrams[i].CalculateNGramProbabilities(_simpleSmoothing, j);
                }

                nGrams[i].CalculateNGramProbabilities(_simpleSmoothing, 1);
            }

            if (n == 2)
            {
                _lambda1 = LearnBestLambda(nGrams, kFoldCrossValidation, 0.1);
            }
            else
            {
                if (n == 3)
                {
                    var bestLambdas = LearnBestLambdas(nGrams, kFoldCrossValidation, 0.1, 0.1);
                    _lambda1 = bestLambdas[0];
                    _lambda2 = bestLambdas[1];
                }
            }
        }

        /**
         * <summary>Wrapper function to set the N-gram probabilities with interpolated smoothing.</summary>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         *
         */
        public override void SetProbabilities(NGram<TSymbol> nGram, int level)
        {
            for (var j = 2; j <= nGram.GetN(); j++)
            {
                nGram.CalculateNGramProbabilities(_simpleSmoothing, j);
            }

            nGram.CalculateNGramProbabilities(_simpleSmoothing, 1);
            switch (nGram.GetN())
            {
                case 2:
                    nGram.SetLambda(_lambda1);
                    break;
                case 3:
                    nGram.SetLambda(_lambda1, _lambda2);
                    break;
            }
        }
    }
}