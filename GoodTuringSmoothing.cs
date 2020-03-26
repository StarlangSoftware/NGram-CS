using System.Collections.Generic;
using Math;

namespace NGram
{
    public class GoodTuringSmoothing<TSymbol> : SimpleSmoothing<TSymbol>
    {
        /**
         * <summary>Given counts of counts, this function will calculate the estimated counts of counts c$^*$ with
         * Good-Turing smoothing. First, the algorithm filters the non-zero counts from counts of counts array and constructs
         * c and r arrays. Then it constructs Z_n array with Z_n = (2C_n / (r_{n+1} - r_{n-1})). The algorithm then uses
         * simple linear regression on Z_n values to estimate w_1 and w_0, where log(N[i]) = w_1log(i) + w_0</summary>
         * <param name="countsOfCounts">Counts of counts. countsOfCounts[1] is the number of words occurred once in the corpus.</param>
         *                       countsOfCounts[i] is the number of words occurred i times in the corpus.
         * <returns>Estimated counts of counts array. N[1] is the estimated count for out of vocabulary words.</returns>
         */
        private double[] LinearRegressionOnCountsOfCounts(int[] countsOfCounts)
        {
            var n = new double[countsOfCounts.Length];
            var r = new List<int>();
            var c = new List<int>();
            for (var i = 1; i < countsOfCounts.Length; i++)
            {
                if (countsOfCounts[i] != 0)
                {
                    r.Add(i);
                    c.Add(countsOfCounts[i]);
                }
            }

            var a = new Matrix(2, 2);
            var y = new Vector(2, 0);
            for (var i = 0; i < r.Count; i++)
            {
                var xt = System.Math.Log(r[i]);
                double rt;
                if (i == 0)
                {
                    rt = System.Math.Log(c[i]);
                }
                else
                {
                    if (i == r.Count - 1)
                    {
                        rt = System.Math.Log(1.0 * c[i] / (r[i] - r[i - 1]));
                    }
                    else
                    {
                        rt = System.Math.Log(2.0 * c[i] / (r[i + 1] - r[i - 1]));
                    }
                }

                a.AddValue(0, 0, 1.0);
                a.AddValue(0, 1, xt);
                a.AddValue(1, 0, xt);
                a.AddValue(1, 1, xt * xt);
                y.AddValue(0, rt);
                y.AddValue(1, rt * xt);
            }

            a.Inverse();
            var w = a.MultiplyWithVectorFromRight(y);
            var w0 = w.GetValue(0);
            var w1 = w.GetValue(1);
            for (var i = 1; i < countsOfCounts.Length; i++)
            {
                n[i] = System.Math.Exp(System.Math.Log(i) * w1 + w0);
            }

            return n;
        }

        /**
         * <summary>Wrapper function to set the N-gram probabilities with Good-Turing smoothing. N[1] / \sum_{i=1}^infty N_i is
         * the out of vocabulary probability.</summary>
         * <param name="nGram">N-Gram for which the probabilities will be set.</param>
         * <param name="level">Level for which N-Gram probabilities will be set. Probabilities for different levels of the</param>
         *              N-gram can be set with this function. If level = 1, N-Gram is treated as UniGram, if level = 2,
         *              N-Gram is treated as Bigram, etc.
         */
        public override void SetProbabilities(NGram<TSymbol> nGram, int level)
        {
            var countsOfCounts = nGram.CalculateCountsOfCounts(level);
            var n = LinearRegressionOnCountsOfCounts(countsOfCounts);
            double sum = 0;
            for (var r = 1; r < countsOfCounts.Length; r++)
            {
                sum += countsOfCounts[r] * r;
            }

            nGram.SetAdjustedProbability(n, level, n[1] / sum);
        }
    }
}