using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.WebDAV
{

    /// <summary>
    /// Result of the OPTIONS method
    /// </summary>
    public class OptionsResult
    {
        /// <summary>
        /// Reference of the resource
        /// </summary>
        public string ResourceRef { get; set; }

        /// <summary>
        /// List of compliance classes
        /// </summary>
        public List<DAVComplianceClass> ComplianceClasses { get; private set; } = new List<DAVComplianceClass>();

        /// <summary>
        /// List of methods allowed
        /// </summary>
        public List<string> Allow { get; private set; } = new List<string>();
    }

}
