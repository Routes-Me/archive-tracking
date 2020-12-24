using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ArchiveTrackService.Helper
{
    public class Common
    {
        public static dynamic ThrowException(string message, int statusCode)
        {
            var ex = new Exception();
            ex.Data.Add(message, statusCode);
            throw ex;
        }
    }
}
