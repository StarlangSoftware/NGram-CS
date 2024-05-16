using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataStructure;

namespace NGram
{
    public class NGram<TSymbol>
    {
        public readonly NGramNode<TSymbol> rootNode;
        private int _n;
        private double _lambda1, _lambda2;
        private bool _interpolated = false;
        private HashSet<TSymbol> _vocabulary;
        private double[] _probabilityOfUnseen;

        /**
         * <summary>Constructor of {@link NGram} class which takes a {@link ArrayList} corpus and {@link Integer} size of ngram as input.
         * It adds all sentences of corpus as ngrams.</summary>
         *
         * <param name="corpus">{@link ArrayList} list of sentences whose ngrams are added.</param>
         * <param name="n">size of ngram.</param>
         */
        public NGram(List<List<TSymbol>> corpus, int n)
        {
            int i;
            this._n = n;
            this._vocabulary = new HashSet<TSymbol>();
            _probabilityOfUnseen = new double[n];
            rootNode = new NGramNode<TSymbol>(default(TSymbol));
            for (i = 0; i < corpus.Count; i++)
                AddNGramSentence(corpus[i].ToArray());
        }

        /**
         * <summary>Constructor of {@link NGram} class which takes {@link Integer} size of ngram.</summary>
         *
         * <param name="n">size of ngram.</param>
         */
        public NGram(int n)
        {
            this._n = n;
            this._vocabulary = new HashSet<TSymbol>();
            this._probabilityOfUnseen = new double[n];
            rootNode = new NGramNode<TSymbol>(default(TSymbol));
        }

        /// <summary>
        /// Reads the header from the input file.
        /// </summary>
        /// <param name="br">Input file</param>
        public void ReadHeader(StreamReader br)
        {
            var line = br.ReadLine();
            var items = line.Split(" ");
            this._n = int.Parse(items[0]);
            this._lambda1 = double.Parse(items[1]);
            this._lambda2 = double.Parse(items[2]);
            this._probabilityOfUnseen = new double[_n];
            line = br.ReadLine();
            items = line.Split(" ");
            for (var i = 0; i < _n; i++)
            {
                this._probabilityOfUnseen[i] = double.Parse(items[i]);
            }

            this._vocabulary = new HashSet<TSymbol>();
            var vocabularySize = int.Parse(br.ReadLine());
            for (var i = 0; i < vocabularySize; i++)
            {
                this._vocabulary.Add((TSymbol) Convert.ChangeType(br.ReadLine(), typeof(TSymbol)));
            }
        }

        /**
         * <summary>Constructor of {@link NGram} class which takes filename to read from text file.</summary>
         *
         * <param name="fileName">name of the text file where NGram is saved.</param>
         */
        public NGram(string fileName)
        {
            var br = new StreamReader(fileName);
            ReadHeader(br);
            rootNode = new NGramNode<TSymbol>(true, br);
            br.Close();
        }

        /// <summary>
        /// Constructor of NGram class which takes a list of files to read.
        /// </summary>
        /// <param name="fileNameList">List of the files where NGram is saved.</param>
        public NGram(params string[] fileNameList)
        {
            var multipleFile = new MultipleFile(fileNameList);
            ReadHeader(multipleFile.GetStreamReader());
            rootNode = new NGramNode<TSymbol>(true, multipleFile);
            multipleFile.Close();
        }
        
        /// <summary>
        /// Merges current NGram with the given NGram. If N of the two NGram's are not same, it does not
        /// merge. Merges first the vocabulary, then the NGram trees.
        /// </summary>
        /// <param name="toBeMerged">NGram to be merged with.</param>
        public void Merge(NGram<TSymbol> toBeMerged){
            if (_n != toBeMerged.GetN()){
                return;
            }
            _vocabulary.UnionWith(toBeMerged._vocabulary);
            rootNode.Merge(toBeMerged.rootNode);
        }

        /**
         * <returns>size of ngram.</returns>
         */
        public int GetN()
        {
            return _n;
        }

        /**
         * <summary>Set size of ngram.</summary>
         * <param name="n">size of ngram</param>
         */
        public void SetN(int n)
        {
            this._n = n;
        }

        /**
         * <summary>Adds {@link Symbol[]} given array of symbols to {@link HashSet} the vocabulary and to {@link NGramNode} the rootNode</summary>
         *
         * <param name="symbols">{@link Symbol[]} ngram added.</param>
         */
        public void AddNGram(TSymbol[] symbols)
        {
            foreach (var symbol in symbols)
            {
                _vocabulary.Add(symbol);
            }

            rootNode.AddNGram(symbols, 0, _n);
        }

        /**
         * <summary>Adds given sentence count times to {@link HashSet} the vocabulary and create and add ngrams of the sentence to {@link NGramNode} the rootNode</summary>
         *
         * <param name="symbols">{@link Symbol[]} sentence whose ngrams are added.</param>
         */
        public void AddNGramSentence(TSymbol[] symbols, int count = 1)
        {
            foreach (var symbol in symbols)
            {
                _vocabulary.Add(symbol);
            }

            for (var j = 0; j < symbols.Length - _n + 1; j++)
            {
                rootNode.AddNGram(symbols, j, _n, count);
            }
        }

        /**
         * <returns>vocabulary size.</returns>
         */
        public double VocabularySize()
        {
            return _vocabulary.Count;
        }

        /**
         * <summary>Sets lambda, interpolation ratio, for bigram and unigram probabilities.
         * ie. lambda1 * bigramProbability + (1 - lambda1) * unigramProbability</summary>
         *
         * <param name="lambda1">interpolation ratio for bigram probabilities</param>
         */
        public void SetLambda(double lambda1)
        {
            if (_n == 2)
            {
                _interpolated = true;
                this._lambda1 = lambda1;
            }
        }

        /**
         * <summary>Sets lambdas, interpolation ratios, for trigram, bigram and unigram probabilities.
         * ie. lambda1 * trigramProbability + lambda2 * bigramProbability  + (1 - lambda1 - lambda2) * unigramProbability</summary>
         *
         * <param name="lambda1">interpolation ratio for trigram probabilities</param>
         * <param name="lambda2">interpolation ratio for bigram probabilities</param>
         */
        public void SetLambda(double lambda1, double lambda2)
        {
            if (_n == 3)
            {
                _interpolated = true;
                this._lambda1 = lambda1;
                this._lambda2 = lambda2;
            }
        }

        /**
         * <summary>Calculates NGram probabilities using {@link ArrayList} given corpus and {@link TrainedSmoothing} smoothing method.</summary>
         *
         * <param name="corpus">corpus for calculating NGram probabilities.</param>
         * <param name="trainedSmoothing">instance of smoothing method for calculating ngram probabilities.</param>
         */
        public void CalculateNGramProbabilities(List<List<TSymbol>> corpus,
            TrainedSmoothing<TSymbol> trainedSmoothing)
        {
            trainedSmoothing.Train(corpus, this);
        }

        /**
         * <summary>Calculates NGram probabilities using {@link SimpleSmoothing} simple smoothing.</summary>
         *
         * <param name="simpleSmoothing">{@link SimpleSmoothing}</param>
         */
        public void CalculateNGramProbabilities(SimpleSmoothing<TSymbol> simpleSmoothing)
        {
            simpleSmoothing.SetProbabilities(this);
        }

        /**
         * <summary>Calculates NGram probabilities given {@link SimpleSmoothing} simple smoothing and level.</summary>
         *
         * <param name="simpleSmoothing">{@link SimpleSmoothing}</param>
         * <param name="level">Level for which N-Gram probabilities will be set.</param>
         *
         */
        public void CalculateNGramProbabilities(SimpleSmoothing<TSymbol> simpleSmoothing, int level)
        {
            simpleSmoothing.SetProbabilities(this, level);
        }

        /**
         * <summary>Replaces words not in {@link HashSet} given dictionary.</summary>
         *
         * <param name="dictionary">dictionary of known words.</param>
         */
        public void ReplaceUnknownWords(HashSet<TSymbol> dictionary)
        {
            rootNode.ReplaceUnknownWords(dictionary);
        }

        /**
         * <summary>Constructs a dictionary of non rare words with given N-Gram level and probability threshold.</summary>
         *
         * <param name="level">Level for counting words. Counts for different levels of the N-Gram can be set. If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *              N-Gram is treated as Bigram, etc.
         * <param name="probability">probability threshold for non rare words.</param>
         * <returns>{@link HashSet} non rare words.</returns>
         */
        public HashSet<TSymbol> ConstructDictionaryWithNonRareWords(int level, double probability)
        {
            var result = new HashSet<TSymbol>();
            var wordCounter = new CounterHashMap<TSymbol>();
            rootNode.CountWords(wordCounter, level);
            var sum = wordCounter.SumOfCounts();
            foreach (var symbol in wordCounter.Keys)
            {
                if (wordCounter[symbol] / (sum + 0.0) > probability)
                {
                    result.Add(symbol);
                }
            }

            return result;
        }

        /**
         * <summary>Calculates unigram perplexity of given corpus. First sums negative log likelihoods of all unigrams in corpus.
         * Then returns exp of average negative log likelihood.</summary>
         *
         * <param name="corpus">corpus whose unigram perplexity is calculated.</param>
         *
         * <returns>unigram perplexity of corpus.</returns>
         */
        private double GetUniGramPerplexity(List<List<TSymbol>> corpus)
        {
            double sum = 0;
            var count = 0;
            foreach (var symbols in corpus)
            {
                foreach (var symbol in symbols)
                {
                    var p = GetProbability(symbol);
                    sum -= System.Math.Log(p);
                    count++;
                }
            }

            return System.Math.Exp(sum / count);
        }

        /**
         * <summary>Calculates bigram perplexity of given corpus. First sums negative log likelihoods of all bigrams in corpus.
         * Then returns exp of average negative log likelihood.</summary>
         *
         * <param name="corpus">corpus whose bigram perplexity is calculated.</param>
         *
         * <returns>bigram perplexity of given corpus.</returns>
         */
        private double GetBiGramPerplexity(List<List<TSymbol>> corpus)
        {
            double sum = 0;
            var count = 0;
            foreach (var symbols in corpus)
            {
                for (var j = 0; j < symbols.Count - 1; j++)
                {
                    var p = GetProbability(symbols[j], symbols[j + 1]);
                    sum -= System.Math.Log(p);
                    count++;
                }
            }

            return System.Math.Exp(sum / count);
        }

        /**
         * <summary>Calculates trigram perplexity of given corpus. First sums negative log likelihoods of all trigrams in corpus.
         * Then returns exp of average negative log likelihood.</summary>
         *
         * <param name="corpus">corpus whose trigram perplexity is calculated.</param>
         * <returns>trigram perplexity of given corpus.</returns>
         */
        private double GetTriGramPerplexity(List<List<TSymbol>> corpus)
        {
            double sum = 0;
            var count = 0;
            foreach (var symbols in corpus)
            {
                for (var j = 0; j < symbols.Count - 2; j++)
                {
                    var p = GetProbability(symbols[j], symbols[j + 1], symbols[j + 2]);
                    sum -= System.Math.Log(p);
                    count++;
                }
            }

            return System.Math.Exp(sum / count);
        }

        /**
         * <summary>Calculates the perplexity of given corpus depending on N-Gram model (unigram, bigram, trigram, etc.)</summary>
         *
         * <param name="corpus">corpus whose perplexity is calculated.</param>
         * <returns>perplexity of given corpus</returns>
         */
        public double GetPerplexity(List<List<TSymbol>> corpus)
        {
            switch (_n)
            {
                case 1:
                    return GetUniGramPerplexity(corpus);
                case 2:
                    return GetBiGramPerplexity(corpus);
                case 3:
                    return GetTriGramPerplexity(corpus);
                default:
                    return 0;
            }
        }

        /**
         * <summary>Gets probability of sequence of symbols depending on N in N-Gram. If N is 1, returns unigram probability.
         * If N is 2, if interpolated is true, then returns interpolated bigram and unigram probability, otherwise returns only bigram probability.
         * If N is 3, if interpolated is true, then returns interpolated trigram, bigram and unigram probability, otherwise returns only trigram probability.</summary>
         * <param name="symbols">sequence of symbol.</param>
         * <returns>probability of given sequence.</returns>
         */
        public double GetProbability(params TSymbol[] symbols)
        {
            switch (_n)
            {
                case 1:
                    return GetUniGramProbability(symbols[0]);
                case 2:
                    if (symbols.Length == 1)
                    {
                        return GetUniGramProbability(symbols[0]);
                    }

                    if (_interpolated)
                    {
                        return _lambda1 * GetBiGramProbability(symbols[0], symbols[1]) +
                               (1 - _lambda1) * GetUniGramProbability(symbols[1]);
                    }
                    else
                    {
                        return GetBiGramProbability(symbols[0], symbols[1]);
                    }
                case 3:
                    if (symbols.Length == 1)
                    {
                        return GetUniGramProbability(symbols[0]);
                    }
                    else
                    {
                        if (symbols.Length == 2)
                        {
                            return GetBiGramProbability(symbols[0], symbols[1]);
                        }
                    }

                    if (_interpolated)
                    {
                        return _lambda1 * GetTriGramProbability(symbols[0], symbols[1], symbols[2]) +
                               _lambda2 * GetBiGramProbability(symbols[1], symbols[2]) +
                               (1 - _lambda1 - _lambda2) * GetUniGramProbability(symbols[2]);
                    }
                    else
                    {
                        return GetTriGramProbability(symbols[0], symbols[1], symbols[2]);
                    }
            }

            return 0.0;
        }

        /**
         * <summary>Gets unigram probability of given symbol.</summary>
         * <param name="w1">a unigram symbol.</param>
         * <returns>probability of given unigram.</returns>
         */
        private double GetUniGramProbability(TSymbol w1)
        {
            return rootNode.GetUniGramProbability(w1);
        }

        /**
         * <summary>Gets bigram probability of given symbols.</summary>
         * <param name="w1">first gram of bigram</param>
         * <param name="w2">second gram of bigram</param>
         * <returns>probability of bigram formed by w1 and w2.</returns>
         */
        private double GetBiGramProbability(TSymbol w1, TSymbol w2)
        {
            try
            {
                return rootNode.GetBiGramProbability(w1, w2);
            }
            catch (UnseenCase unseenCase)
            {
                return _probabilityOfUnseen[1];
            }
        }

        /**
         * <summary>Gets trigram probability of given symbols.</summary>
         * <param name="w1">first gram of trigram</param>
         * <param name="w2">second gram of trigram</param>
         * <param name="w3">third gram of trigram</param>
         * <returns>probability of trigram formed by w1, w2, w3.</returns>
         */
        private double GetTriGramProbability(TSymbol w1, TSymbol w2, TSymbol w3)
        {
            try
            {
                return rootNode.GetTriGramProbability(w1, w2, w3);
            }
            catch (UnseenCase unseenCase)
            {
                return _probabilityOfUnseen[2];
            }
        }

        /**
         * <summary>Gets count of given sequence of symbol.</summary>
         * <param name="symbols">sequence of symbol.</param>
         * <returns>count of symbols.</returns>
         */
        public int GetCount(TSymbol[] symbols)
        {
            return rootNode.GetCount(symbols, 0);
        }

        /**
         * <summary>Sets probabilities by adding pseudocounts given height and pseudocount.</summary>
         * <param name="pseudoCount">pseudocount added to all N-Grams.</param>
         * <param name="height"> height for N-Gram. If height= 1, N-Gram is treated as UniGram, if height = 2,</param>
         *                N-Gram is treated as Bigram, etc.
         */
        public void SetProbabilityWithPseudoCount(double pseudoCount, int height)
        {
            double vocabularySize;
            if (pseudoCount != 0)
            {
                vocabularySize = VocabularySize() + 1;
            }
            else
            {
                vocabularySize = VocabularySize();
            }

            rootNode.SetProbabilityWithPseudoCount(pseudoCount, height, vocabularySize);
            _probabilityOfUnseen[height - 1] = 1.0 / vocabularySize;
        }

        /**
         * <summary>Find maximum occurrence in given height.</summary>
         * <param name="height">height for occurrences. If height = 1, N-Gram is treated as UniGram, if height = 2,</param>
         *               N-Gram is treated as Bigram, etc.
         * <returns>maximum occurrence in given height.</returns>
         */
        private int MaximumOccurrence(int height)
        {
            return rootNode.MaximumOccurrence(height);
        }

        /**
         * <summary>Update counts of counts of N-Grams with given counts of counts and given height.</summary>
         * <param name="countsOfCounts">updated counts of counts.</param>
         * <param name="height"> height for NGram. If height = 1, N-Gram is treated as UniGram, if height = 2,</param>
         *                N-Gram is treated as Bigram, etc.
         */
        private void UpdateCountsOfCounts(int[] countsOfCounts, int height)
        {
            rootNode.UpdateCountsOfCounts(countsOfCounts, height);
        }

        /**
         * <summary>Calculates counts of counts of NGrams.</summary>
         * <param name="height"> height for NGram. If height = 1, N-Gram is treated as UniGram, if height = 2,</param>
         *                N-Gram is treated as Bigram, etc.
         * <returns>counts of counts of NGrams.</returns>
         */
        public int[] CalculateCountsOfCounts(int height)
        {
            var maxCount = MaximumOccurrence(height);
            var countsOfCounts = new int[maxCount + 2];
            UpdateCountsOfCounts(countsOfCounts, height);
            return countsOfCounts;
        }

        /**
         * <summary>Sets probability with given counts of counts and pZero.</summary>
         * <param name="countsOfCounts">counts of counts of NGrams.</param>
         * <param name="height"> height for NGram. If height = 1, N-Gram is treated as UniGram, if height = 2,</param>
         *                N-Gram is treated as Bigram, etc.
         * <param name="pZero">probability of zero.</param>
         */
        public void SetAdjustedProbability(double[] countsOfCounts, int height, double pZero)
        {
            rootNode.SetAdjustedProbability(countsOfCounts, height, VocabularySize() + 1, pZero);
            _probabilityOfUnseen[height - 1] = 1.0 / (VocabularySize() + 1);
        }

        /// <summary>
        /// Prunes NGram according to the given threshold. All nodes having a probability less than the threshold will be
        /// pruned.
        /// </summary>
        /// <param name="threshold">Probability threshold used for pruning.</param>
        public void Prune(double threshold)
        {
            if (threshold > 0.0 && threshold <= 1.0)
            {
                rootNode.Prune(threshold, _n - 1);
            }
        }

        /**
         * <summary>Save this NGram to a text file.</summary>
         *
         * <param name="fileName">{@link String} name of file where NGram is saved.</param>
         */
        public void SaveAsText(string fileName)
        {
            var streamWriter = new StreamWriter(fileName);
            streamWriter.WriteLine(_n + " " + _lambda1 + " " + _lambda2);
            for (var i = 0; i < _n; i++)
            {
                streamWriter.Write(_probabilityOfUnseen[i] + " ");
            }

            streamWriter.WriteLine();
            streamWriter.WriteLine((int) VocabularySize());
            foreach (var symbol in _vocabulary)
            {
                streamWriter.WriteLine(symbol.ToString());
            }

            rootNode.SaveAsText(true, streamWriter, 0);
            streamWriter.Close();
        }
    }
}