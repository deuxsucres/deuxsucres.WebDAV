using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace deuxsucres.iCalendar.Tests
{
    class Utils
    {
        public static Stream OpenResourceStream(string name)
        {
            return typeof(Utils).Assembly.GetManifestResourceStream(
                string.Join(".",
                    nameof(deuxsucres),
                    nameof(deuxsucres.iCalendar),
                    nameof(deuxsucres.iCalendar.Tests),
                    "samples",
                    name
                ));
        }

        public static string ReadResource(string name)
        {
            using (var str = OpenResourceStream(name))
            using (var rdr = new StreamReader(str))
                return rdr.ReadToEnd();
        }
    }
}
