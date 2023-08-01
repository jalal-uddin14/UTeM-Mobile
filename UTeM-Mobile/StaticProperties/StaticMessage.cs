using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UTeM_Mobile.StaticProperties
{
    public static class StaticMessage
    {
        public static bool HasNFCMessage { get; set; } = false;
        public static string NFCMessage { get; set; }
    }
}
