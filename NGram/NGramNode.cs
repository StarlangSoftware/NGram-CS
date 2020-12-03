using System;
using System.Collections.Generic;
using System.IO;
using DataStructure;

namespace NGram
{
    public class NGramNode<TSymbol>
    {
        private Dictionary<TSymbol, NGramNode<TSymbol>> _children;
        private TSymbol _symbol;
        private int _count;
        private double _probability;
        private double _probabilityOfUnseen;
        private NGramNode<TSymbol> _unknown;

        /**
         * <summary>Constructor of {@link NGramNode}</summary>
         *
         * <param name="symbol">symbol to be kept in this node.</param>
         */
        public NGramNode(TSymbol symbol)
        {
            this._symbol = symbol;
            _count = 0;
        }

        /**
         * <summary>Constructor of {@link NGramNode}</summary>
         *
         * <param name="isRootNode">True if this node is root node, false otherwise.</param>
         * <param name="streamReader">        File to be read.</param>
         */
        public NGramNode(bool isRootNode, StreamReader streamReader)
        {
            if (!isRootNode)
            {
                this._symbol = (TSymbol) Convert.ChangeType(streamReader.ReadLine().Trim(), typeof(TSymbol));
            }

            var line = streamReader.ReadLine();
            var items = line.Split(" ");
            this._count = int.Parse(items[0]);
            this._probability = double.Parse(items[1]);
            this._probabilityOfUnseen = double.Parse(items[2]);
            var numberOfChildren = int.Parse(items[3]);
            if (numberOfChildren > 0)
            {
                _children = new Dictionary<TSymbol, NGramNode<TSymbol>>();
                for (var i = 0; i < numberOfChildren; i++)
                {
                    var childNode = new NGramNode<TSymbol>(false, streamReader);
                    _children.Add(childNode._symbol, childNode);
                }
            }
        }

        public NGramNode(bool isRootNode, MultipleFile multipleFile)
        {
            if (!isRootNode)
            {
                this._symbol = (TSymbol) Convert.ChangeType(multipleFile.ReadLine().Trim(), typeof(TSymbol));
            }

            var line = multipleFile.ReadLine();
            var items = line.Split(" ");
            this._count = int.Parse(items[0]);
            this._probability = double.Parse(items[1]);
            this._probabilityOfUnseen = double.Parse(items[2]);
            var numberOfChildren = int.Parse(items[3]);
            if (numberOfChildren > 0)
            {
                _children = new Dictionary<TSymbol, NGramNode<TSymbol>>();
                for (var i = 0; i < numberOfChildren; i++)
                {
                    var childNode = new NGramNode<TSymbol>(false, multipleFile);
                    _children.Add(childNode._symbol, childNode);
                }
            }
        }

        /**
         * <summary>Gets count of this node.</summary>
         *
         * <returns>count of this node.</returns>
         */
        public int GetCount()
        {
            return _count;
        }

        /**
         * <summary>Gets the size of children of this node.</summary>
         *
         * <returns>size of children of {@link NGramNode} this node.</returns>
         */
        public int Size()
        {
            return _children.Count;
        }

        /**
         * <summary>Finds maximum occurrence. If height is 0, returns the count of this node.
         * Otherwise, traverses this nodes' children recursively and returns maximum occurrence.</summary>
         *
         * <param name="height">height for NGram.</param>
         * <returns>maximum occurrence.</returns>
         */
        public int MaximumOccurrence(int height)
        {
            var max = 0;
            if (height == 0)
            {
                return _count;
            }

            foreach (var child in _children.Values)
            {
                var current = child.MaximumOccurrence(height - 1);
                if (current > max)
                {
                    max = current;
                }
            }

            return max;
        }

        /**
         * <returns>sum of counts of children nodes.</returns>
         */
        double ChildSum()
        {
            double sum = 0;
            foreach (var child in _children.Values)
            {
                sum += child._count;
            }

            if (_unknown != null)
            {
                sum += _unknown._count;
            }

            return sum;
        }

        /**
         * <summary>Traverses nodes and updates counts of counts for each node.</summary>
         *
         * <param name="countsOfCounts">counts of counts of NGrams.</param>
         * <param name="height">        height for NGram. if height = 1, If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *                       N-Gram is treated as Bigram, etc.
         */
        public void UpdateCountsOfCounts(int[] countsOfCounts, int height)
        {
            if (height == 0)
            {
                countsOfCounts[_count]++;
            }
            else
            {
                foreach (var child in _children.Values)
                {
                    child.UpdateCountsOfCounts(countsOfCounts, height - 1);
                }
            }
        }

        /**
         * <summary>Sets probabilities by traversing nodes and adding pseudocount for each NGram.</summary>
         *
         * <param name="pseudoCount">   pseudocount added to each NGram.</param>
         * <param name="height">        height for NGram. if height = 1, If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *                       N-Gram is treated as Bigram, etc.
         * <param name="vocabularySize">size of vocabulary</param>
         */
        public void SetProbabilityWithPseudoCount(double pseudoCount, int height, double vocabularySize)
        {
            if (height == 1)
            {
                var sum = ChildSum() + pseudoCount * vocabularySize;
                foreach (var child in _children.Values)
                {
                    child._probability = (child._count + pseudoCount) / sum;
                }

                if (_unknown != null)
                {
                    _unknown._probability = (_unknown._count + pseudoCount) / sum;
                }

                _probabilityOfUnseen = pseudoCount / sum;
            }
            else
            {
                foreach (var child in _children.Values)
                {
                    child.SetProbabilityWithPseudoCount(pseudoCount, height - 1, vocabularySize);
                }
            }
        }

        /**
         * <summary>Sets adjusted probabilities with counts of counts of NGrams.
         * For count less than 5, count is considered as ((r + 1) * N[r + 1]) / N[r]), otherwise, count is considered as it is.
         * Sum of children counts are computed. Then, probability of a child node is (1 - pZero) * (r / sum) if r > 5
         * otherwise, r is replaced with ((r + 1) * N[r + 1]) / N[r]) and calculated the same.</summary>
         *
         * <param name="N">             counts of counts of NGrams.</param>
         * <param name="height">        height for NGram. if height = 1, If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *                       N-Gram is treated as Bigram, etc.
         * <param name="vocabularySize">size of vocabulary.</param>
         * <param name="pZero">         probability of zero.</param>
         */
        public void SetAdjustedProbability(double[] N, int height, double vocabularySize, double pZero)
        {
            if (height == 1)
            {
                double sum = 0;
                foreach (var child in _children.Values)
                {
                    var r = child._count;
                    if (r <= 5)
                    {
                        var newR = ((r + 1) * N[r + 1]) / N[r];
                        sum += newR;
                    }
                    else
                    {
                        sum += r;
                    }
                }

                foreach (var child in _children.Values)
                {
                    var r = child._count;
                    if (r <= 5)
                    {
                        var newR = ((r + 1) * N[r + 1]) / N[r];
                        child._probability = (1 - pZero) * (newR / sum);
                    }
                    else
                    {
                        child._probability = (1 - pZero) * (r / sum);
                    }
                }

                _probabilityOfUnseen = pZero / (vocabularySize - _children.Count);
            }
            else
            {
                foreach (var child in _children.Values)
                {
                    child.SetAdjustedProbability(N, height - 1, vocabularySize, pZero);
                }
            }
        }

        /**
         * <summary>Adds count times NGram given as array of symbols to the node as a child.</summary>
         *
         * <param name="sentence">     array of symbols</param>
         * <param name="index"> start index of NGram</param>
         * <param name="height">height for NGram. if height = 1, If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *               N-Gram is treated as Bigram, etc.
         * <param name="count"> Number of times this NGram is added.</param>
         */
        public void AddNGram(TSymbol[] sentence, int index, int height, int count = 1)
        {
            NGramNode<TSymbol> child;
            if (height == 0)
            {
                return;
            }

            var s = sentence[index];
            if (_children != null && _children.ContainsKey(s))
            {
                child = _children[s];
            }
            else
            {
                child = new NGramNode<TSymbol>(s);
                if (_children == null)
                {
                    _children = new Dictionary<TSymbol, NGramNode<TSymbol>>();
                }

                _children.Add(s, child);
            }

            child._count += count;
            child.AddNGram(sentence, index + 1, height - 1, count);
        }

        /**
         * <summary>Gets unigram probability of given symbol.</summary>
         *
         * <param name="w1">unigram.</param>
         * <returns>unigram probability of given symbol.</returns>
         */
        public double GetUniGramProbability(TSymbol w1)
        {
            if (_children.ContainsKey(w1))
            {
                return _children[w1]._probability;
            }

            if (_unknown != null)
            {
                return _unknown._probability;
            }

            return _probabilityOfUnseen;
        }

        /**
         * <summary>Gets bigram probability of given symbols w1 and w2</summary>
         *
         * <param name="w1">first gram of bigram.</param>
         * <param name="w2">second gram of bigram.</param>
         * <returns>probability of given bigram</returns>
         */
        public double GetBiGramProbability(TSymbol w1, TSymbol w2)
        {
            if (_children.ContainsKey(w1))
            {
                var child = _children[w1];
                return child.GetUniGramProbability(w2);
            }

            if (_unknown != null)
            {
                return _unknown.GetUniGramProbability(w2);
            }

            throw new UnseenCase();
        }

        /**
         * <summary>Gets trigram probability of given symbols w1, w2 and w3.</summary>
         *
         * <param name="w1">first gram of trigram</param>
         * <param name="w2">second gram of trigram</param>
         * <param name="w3">third gram of trigram</param>
         * <returns>probability of given trigram.</returns>
         */
        public double GetTriGramProbability(TSymbol w1, TSymbol w2, TSymbol w3)
        {
            if (_children.ContainsKey(w1))
            {
                var child = _children[w1];
                return child.GetBiGramProbability(w2, w3);
            }

            if (_unknown != null)
            {
                return _unknown.GetBiGramProbability(w2, w3);
            }

            throw new UnseenCase();
        }

        /**
         * <summary>Counts words recursively given height and wordCounter.</summary>
         *
         * <param name="wordCounter">word counter keeping symbols and their counts.</param>
         * <param name="height">     height for NGram. if height = 1, If level = 1, N-Gram is treated as UniGram, if level = 2,</param>
         *                    N-Gram is treated as Bigram, etc.
         */
        public void CountWords(CounterHashMap<TSymbol> wordCounter, int height)
        {
            if (height == 0)
            {
                wordCounter.PutNTimes(_symbol, _count);
            }
            else
            {
                foreach (var child in _children.Values)
                {
                    child.CountWords(wordCounter, height - 1);
                }
            }
        }

        /**
         * <summary>Replace words not in given dictionary.
         * Deletes unknown words from children nodes and adds them to {@link NGramNode#unknown} unknown node as children recursively.</summary>
         *
         * <param name="dictionary">dictionary of known words.</param>
         */
        public void ReplaceUnknownWords(HashSet<TSymbol> dictionary)
        {
            if (_children != null)
            {
                var childList = new List<NGramNode<TSymbol>>();
                foreach (var s in _children.Keys)
                {
                    if (!dictionary.Contains(s))
                    {
                        childList.Add(_children[s]);
                    }
                }

                if (childList.Count > 0)
                {
                    _unknown = new NGramNode<TSymbol>(default(TSymbol));
                    _unknown._children = new Dictionary<TSymbol, NGramNode<TSymbol>>();
                    var sum = 0;
                    foreach (var child in childList)
                    {
                        if (child._children != null)
                        {
                            foreach (var (key, value) in child._children)
                            {
                                _unknown._children.Add(key, value);
                            }
                        }

                        sum += child._count;
                        _children.Remove(child._symbol);
                    }

                    _unknown._count = sum;
                    _unknown.ReplaceUnknownWords(dictionary);
                }

                foreach (var child in _children.Values)
                {
                    child.ReplaceUnknownWords(dictionary);
                }
            }
        }

        /**
         * <summary>Gets count of symbol given array of symbols and index of symbol in this array.</summary>
         *
         * <param name="s">    array of symbols</param>
         * <param name="index">index of symbol whose count is returned</param>
         * <returns>count of the symbol.</returns>
         */
        public int GetCount(TSymbol[] s, int index)
        {
            if (index < s.Length)
            {
                if (_children.ContainsKey(s[index]))
                {
                    return _children[s[index]].GetCount(s, index + 1);
                }

                return 0;
            }

            return GetCount();
        }

        /**
         * <summary>Generates next string for given list of symbol and index</summary>
         *
         * <param name="s">    list of symbol</param>
         * <param name="index">index index of generated string</param>
         * <returns>generated string.</returns>
         */
        public TSymbol GenerateNextString(List<TSymbol> s, int index)
        {
            var sum = 0.0;
            var random = new Random();
            if (index == s.Count)
            {
                var prob = random.NextDouble();
                foreach (var node in _children.Values)
                {
                    if (prob < node._probability + sum)
                    {
                        return node._symbol;
                    }

                    sum += node._probability;
                }
            }
            else
            {
                return _children[s[index]].GenerateNextString(s, index + 1);
            }

            return default(TSymbol);
        }

        public void Prune(double threshold, int N)
        {
            if (N == 0)
            {
                TSymbol maxElement = default;
                NGramNode<TSymbol> maxNode = null;
                var toBeDeleted = new List<TSymbol>();
                foreach (var symbol in _children.Keys)
                {
                    if (_children[symbol]._count / (_count + 0.0) < threshold)
                    {
                        toBeDeleted.Add(symbol);
                    }

                    if (maxNode == null || _children[symbol]._count > _children[maxElement]._count)
                    {
                        maxElement = symbol;
                        maxNode = _children[symbol];
                    }
                }

                foreach (var symbol in toBeDeleted)
                {
                    _children.Remove(symbol);
                }

                if (_children.Count == 0)
                {
                    _children[maxElement] = maxNode;
                }
            }
            else
            {
                foreach (var node in _children.Values)
                {
                    node.Prune(threshold, N - 1);
                }
            }
        }

        /**
         * <summary>Save this NGramNode to a text file.</summary>
         *
         * <param name="isRootNode">True if this not is a root node, false otherwise</param>
         * <param name="streamWriter">{@link BufferedWriter} file where NGram is saved.</param>
         * <param name="level">Level of this node.</param>
         */
        public void SaveAsText(bool isRootNode, StreamWriter streamWriter, int level)
        {
            if (!isRootNode)
            {
                for (var i = 0; i < level; i++)
                {
                    streamWriter.Write("\t");
                }

                streamWriter.WriteLine(_symbol.ToString());
            }

            for (var i = 0; i < level; i++)
            {
                streamWriter.Write("\t");
            }

            if (_children != null)
            {
                streamWriter.WriteLine(_count + " " + _probability + " " + _probabilityOfUnseen + " " + Size());
                foreach (var child in _children.Values)
                {
                    child.SaveAsText(false, streamWriter, level + 1);
                }
            }
            else
            {
                streamWriter.WriteLine(_count + " " + _probability + " " + _probabilityOfUnseen + " 0");
            }
        }
    }
}