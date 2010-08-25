using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ANH_WCF_Interface
{
    public class HashCalc
    {
        public static string GetSHA(string source)
        {
            return BitConverter.ToString(new SHA1CryptoServiceProvider().ComputeHash(
             Encoding.Default.GetBytes(source))).Replace("-", String.Empty);
        }
    }
}
