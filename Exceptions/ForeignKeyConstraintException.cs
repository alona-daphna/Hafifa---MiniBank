using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniBank.Exceptions
{
    internal class ForeignKeyConstraintException : Exception
    {
        public ForeignKeyConstraintException()
        {
        }

        public ForeignKeyConstraintException(string message)
            : base(message)
        {
        }

        public ForeignKeyConstraintException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
