using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// Options for a DAV resource
    /// </summary>
    public class DavOptions
    {

        /// <summary>
        /// Test if a compliant for a class
        /// </summary>
        /// <returns></returns>
        public bool IsCompliant(DavComplianceClass complianceClass)
        {
            return ComplianceClasses.Any(c => string.Equals(complianceClass.Value, c.Value, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Test if a method is allowed
        /// </summary>
        public bool IsAllowed(HttpMethod method)
        {
            return Allow.Any(m => string.Equals(method.Method, m, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// List of compliance classes
        /// </summary>
        public List<DavComplianceClass> ComplianceClasses { get; private set; } = new List<DavComplianceClass>();

        /// <summary>
        /// List of methods allowed
        /// </summary>
        public List<string> Allow { get; private set; } = new List<string>();

    }
}
