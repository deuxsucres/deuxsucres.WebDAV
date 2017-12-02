using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{

    /// <summary>
    /// REQUEST-STATUS property
    /// </summary>
    public class RequestStatusProperty : CalProperty
    {

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            StatusCode = null;
            StatusDescription = null;
            ExtraData = null;
        }

        /// <summary>
        /// New property
        /// </summary>
        public RequestStatusProperty()
        {
            Name = Constants.REQUEST_STATUS;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return string.IsNullOrWhiteSpace(ExtraData)
                ? $"{StatusCode};{StatusDescription}"
                : $"{StatusCode};{StatusDescription};{ExtraData}";
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            if (line.Value == null) return false;
            var parts = line.Value.Split(new char[] { ';' }, 3);
            if (parts.Length > 0)
                StatusCode = parts[0];
            if (parts.Length > 1)
                StatusDescription = parts[1];
            if (parts.Length > 2)
                ExtraData = parts[2];
            return true;
        }

        #endregion

        /// <summary>
        /// Status code
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Status description
        /// </summary>
        public string StatusDescription { get; set; }

        /// <summary>
        /// Extra data
        /// </summary>
        public string ExtraData { get; set; }

        /// <summary>
        /// Language
        /// </summary>
        public TextParameter Language
        {
            get { return FindParameter<TextParameter>(Constants.LANGUAGE); }
            set { SetParameter(value, Constants.LANGUAGE); }
        }

    }
}
