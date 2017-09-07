using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.WebDAV
{
    /// <summary>
    /// DAV Compliance class
    /// </summary>
    public class DavComplianceClass
    {

        /// <summary>
        /// Class 1
        /// </summary>
        public static readonly DavComplianceClass Class1 = new DavComplianceClass("1");
        /// <summary>
        /// Class 2
        /// </summary>
        public static readonly DavComplianceClass Class2 = new DavComplianceClass("2");
        /// <summary>
        /// Class 3
        /// </summary>
        public static readonly DavComplianceClass Class3 = new DavComplianceClass("3");

        /// <summary>
        /// Create a new compliance class
        /// </summary>
        public DavComplianceClass(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException(Locales.SR.Err_InvalidValue, nameof(value));
            if (value == "1")
            {
                ClassType = DAVComplianceClassType.Class1;
                Value = value;
            }
            else if (value == "2")
            {
                ClassType = DAVComplianceClassType.Class2;
                Value = value;
            }
            else if (value == "3")
            {
                ClassType = DAVComplianceClassType.Class3;
                Value = value;
            }
            else if (value.StartsWith("<") && value.EndsWith(">"))
            {
                ClassType = DAVComplianceClassType.CodedUrl;
                Value = value.Substring(1, value.Length - 2);
            }
            else if (ParseHelpers.IsToken(value))
            {
                ClassType = DAVComplianceClassType.Token;
                Value = value;
            }
            else
            {
                ClassType = DAVComplianceClassType.Other;
                Value = value;
            }
        }

        /// <summary>
        /// Type of the class
        /// </summary>
        public DAVComplianceClassType ClassType { get; private set; }

        /// <summary>
        /// Value of the class
        /// </summary>
        public string Value { get; private set; }

    }

    /// <summary>
    /// Type of compliance class
    /// </summary>
    public enum DAVComplianceClassType
    {
        /// <summary>
        /// Class 1
        /// </summary>
        Class1,
        /// <summary>
        /// Class 2
        /// </summary>
        Class2,
        /// <summary>
        /// Class 3
        /// </summary>
        Class3,
        /// <summary>
        /// Token
        /// </summary>
        Token,
        /// <summary>
        /// Code URL
        /// </summary>
        CodedUrl,
        /// <summary>
        /// Non standard class
        /// </summary>
        Other
    }

}
