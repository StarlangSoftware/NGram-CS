using System;

namespace NGram
{
    public class UnseenCase : Exception
    {
        public new string ToString(){
            return "Unseen Case";
        }
        
    }
}