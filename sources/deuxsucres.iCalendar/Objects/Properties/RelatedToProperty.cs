using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// RELATED-TO
    /// </summary>
    public class RelatedToProperty : TextProperty
    {
        /// <summary>
        /// New property
        /// </summary>
        public RelatedToProperty()
        {
            Name = Constants.RELATED_TO;
        }

        /// <summary>
        /// Cast vers un string
        /// </summary>
        public static implicit operator string(RelatedToProperty prop) { return prop?.Value; }

        /// <summary>
        /// Cast depuis un string
        /// </summary>
        public static implicit operator RelatedToProperty(string value) { return value != null ? new RelatedToProperty { Value = value } : null; }

        /// <summary>
        /// Relation type
        /// </summary>
        public EnumParameter<RelationTypes> RelationType
        {
            get { return FindParameter<EnumParameter<RelationTypes>>(Constants.RELTYPE); }
            set { SetParameter(value, Constants.RELTYPE); }
        }
    }
}
