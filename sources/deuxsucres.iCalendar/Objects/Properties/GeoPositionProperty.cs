using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// GEO property
    /// </summary>
    public class GeoPositionProperty : CalProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public GeoPositionProperty()
        {
            Name = Constants.GEO;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public override void Reset()
        {
            base.Reset();
            Latitude = 0;
            Longitude = 0;
        }

        #region Serialization

        /// <summary>
        /// Serialize the value
        /// </summary>
        protected override string SerializeValue(ICalWriter writer, ContentLine line)
        {
            return $"{writer.Parser.EncodeFloat(Latitude)};{writer.Parser.EncodeFloat(Longitude)}";
        }

        /// <summary>
        /// Deserialize the value
        /// </summary>
        protected override bool DeserializeValue(ICalReader reader, ContentLine line)
        {
            if (string.IsNullOrWhiteSpace(line.Value)) return false;
            var parts = line.Value.Split(';');
            if (parts.Length != 2) return false;
            var vla = reader.Parser.ParseFloat(parts[0]);
            var vlo = reader.Parser.ParseFloat(parts[1]);
            if (!vla.HasValue || !vlo.HasValue)
                return false;
            Latitude = vla.Value;
            Longitude = vlo.Value;
            return true;
        }

        #endregion

        /// <summary>
        /// Latitude
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public double Longitude { get; set; }
    }
}
