using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class NoConnectException : Exception
    {
        public NoConnectException(string adress, string port)
            :base($"Can not open connection with adress \"{adress}\":\"{port}\".") { }

    }
}
