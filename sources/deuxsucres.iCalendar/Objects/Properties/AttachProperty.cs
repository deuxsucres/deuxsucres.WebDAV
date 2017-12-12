using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// ATTACH property
    /// </summary>
    public class AttachProperty : CalProperty
    {

        /// <summary>
        /// New property
        /// </summary>
        public AttachProperty()
        {
            Name = Constants.ATTACH;
        }

        /// <summary>
        /// Check the parameters from the context
        /// </summary>
        void CheckParameters()
        {
            if (IsBinary)
            {
                SetParameter(new TextParameter { Name = Constants.ENCODING, Value = "Base64" });
                SetParameter(new EnumParameter<ValueTypes> { Name = Constants.VALUE, Value = ValueTypes.Binary });
            }
            else
            {
                if (FindParameter<TextParameter>(Constants.ENCODING)?.Value.IsEqual("Base64") == true)
                    RemoveParameter(Constants.ENCODING);
                if (FindParameter<EnumParameter<ValueTypes>>(Constants.VALUE)?.Value == ValueTypes.Binary)
                    RemoveParameter(Constants.VALUE);
            }
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            CheckParameters();
            return writer.Parser.EncodeBinary(BinaryValue) ?? writer.Parser.EncodeUri(UriValue, false);
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            if (line.GetParam(Constants.ENCODING).IsEqual("Base64") && line.GetParam(Constants.VALUE).IsEqual("Binary"))
            {
                BinaryValue = reader.Parser.ParseBinary(line.Value);
            }
            else
            {
                UriValue = reader.Parser.ParseUri(line.Value, false);
            }
            return BinaryValue != null || UriValue != null;
        }

        #endregion

        /// <summary>
        /// Cast from Uri
        /// </summary>
        public static implicit operator AttachProperty(Uri value) { return value != null ? new AttachProperty { UriValue = value } : null; }

        /// <summary>
        /// Cast from bytes
        /// </summary>
        public static implicit operator AttachProperty(byte[] value) { return value != null ? new AttachProperty { BinaryValue = value } : null; }

        /// <summary>
        /// Uri value
        /// </summary>
        public Uri UriValue
        {
            get { return _uriValue; }
            set
            {
                _uriValue = value;
                if (_uriValue != null)
                {
                    _binaryValue = null;
                    IsBinary = false;
                }
                CheckParameters();
            }
        }
        private Uri _uriValue;

        /// <summary>
        /// Binary value
        /// </summary>
        public byte[] BinaryValue
        {
            get { return _binaryValue; }
            set
            {
                _binaryValue = value;
                if (_binaryValue != null)
                {
                    _uriValue = null;
                    IsBinary = true;
                }
                CheckParameters();
            }
        }
        private byte[] _binaryValue;

        /// <summary>
        /// Indicates if the value is binaray
        /// </summary>
        public bool IsBinary { get; private set; }

        /// <summary>
        /// Format type
        /// </summary>
        public TextParameter FormatType
        {
            get { return FindParameter<TextParameter>(Constants.FMTTYPE); }
            set { SetParameter(value, Constants.FMTTYPE); }
        }

    }
}
