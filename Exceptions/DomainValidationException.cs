using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCAP.Api.Exceptions
{
    public class DomainValidationException : Exception
    {
        public DomainValidationException()
        {
        }
        public DomainValidationException(string message) : base(message)
        {
        }
    }
}
