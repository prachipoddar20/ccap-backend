using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CCAP.Api.Exceptions
{
    public class RoleAbsentException : Exception
    {
        public RoleAbsentException()
        {
        }
        public RoleAbsentException(string message) : base(message)
        {
        }
    }
}
