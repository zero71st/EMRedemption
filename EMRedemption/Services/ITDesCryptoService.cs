using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRedemption.Services
{
    public interface ITDesCryptoService
    {
        string Encrypt(string data);
        string Decrypt(string data);
    }
}
