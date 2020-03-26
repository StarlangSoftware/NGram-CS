namespace NGram
{
    public abstract class SimpleSmoothing<TSymbol>
    {
        public abstract void SetProbabilities(NGram<TSymbol> nGram, int level);

        public void SetProbabilities(NGram<TSymbol> nGram){
            SetProbabilities(nGram, nGram.GetN());
        }
        
    }
}