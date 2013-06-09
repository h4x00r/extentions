using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquareSnail.Infrastructure
{
    public class BoostMeException : Exception
    {
        public BoostMeException(string message)
            : base(message)
        {
        }

        public int StatusCode
        {
            get;
            set;
        }
    }
}
