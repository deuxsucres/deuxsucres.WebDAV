using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Type of values
    /// </summary>
    public enum ValueTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Binary,
        /// <summary>
        /// 
        /// </summary>
        Boolean,
        /// <summary>
        /// 
        /// </summary>
        Cal_Address,
        /// <summary>
        /// 
        /// </summary>
        Date,
        /// <summary>
        /// 
        /// </summary>
        Date_Time,
        /// <summary>
        /// 
        /// </summary>
        Duration,
        /// <summary>
        /// 
        /// </summary>
        Float,
        /// <summary>
        /// 
        /// </summary>
        Integer,
        /// <summary>
        /// 
        /// </summary>
        Period,
        /// <summary>
        /// 
        /// </summary>
        Recur,
        /// <summary>
        /// 
        /// </summary>
        Text,
        /// <summary>
        /// 
        /// </summary>
        Time,
        /// <summary>
        /// 
        /// </summary>
        Uri,
        /// <summary>
        /// 
        /// </summary>
        Utc_Offset
    }

    /// <summary>
    /// Classes
    /// </summary>
    public enum Classes
    {
        /// <summary>
        /// Confidential
        /// </summary>
        Confidential,
        /// <summary>
        /// Private
        /// </summary>
        Private,
        /// <summary>
        /// Public
        /// </summary>
        Public
    }

    /// <summary>
    /// Status for events
    /// </summary>
    public enum EventStatuses
    {
        /// <summary>
        /// CONFIRMED
        /// </summary>
        Confirmed,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled,
        /// <summary>
        /// Tentative
        /// </summary>
        Tentative
    }

    /// <summary>
    /// Status for todos
    /// </summary>
    public enum TodoStatuses
    {
        /// <summary>
        /// NEEDS-ACTION
        /// </summary>
        Needs_Action,
        /// <summary>
        /// CONFIRMED
        /// </summary>
        Confirmed,
        /// <summary>
        /// IN-PROCESS
        /// </summary>
        In_Process,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Status for journals
    /// </summary>
    public enum JournalStatuses
    {
        /// <summary>
        /// DRAFT
        /// </summary>
        Draft,
        /// <summary>
        /// FINAL
        /// </summary>
        Final,
        /// <summary>
        /// Canceled
        /// </summary>
        Canceled
    }

    /// <summary>
    /// Free/Busy types
    /// </summary>
    public enum FreeBusyTypes
    {
        /// <summary>
        /// Start
        /// </summary>
        Free,
        /// <summary>
        /// End
        /// </summary>
        Busy,
        /// <summary>
        /// 
        /// </summary>
        Busy_Unavailable,
        /// <summary>
        /// 
        /// </summary>
        Busy_Tentative
    }

    /// <summary>
    /// Transparent states
    /// </summary>
    public enum TransparentStates
    {
        /// <summary>
        /// Opaque
        /// </summary>
        Opaque,
        /// <summary>
        /// Transparent
        /// </summary>
        Transparent
    }

    /// <summary>
    /// Calendar user types
    /// </summary>
    public enum CalUserTypes
    {
        /// <summary>
        /// One individual
        /// </summary>
        Individual,
        /// <summary>
        /// A group
        /// </summary>
        Group,
        /// <summary>
        /// A physical resource
        /// </summary>
        Resource,
        /// <summary>
        /// A location resource
        /// </summary>
        Room,
        /// <summary>
        /// Unknwon
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Roles
    /// </summary>
    public enum Roles
    {
        /// <summary>
        /// 
        /// </summary>
        Chair,
        /// <summary>
        /// 
        /// </summary>
        Req_Participant,
        /// <summary>
        /// 
        /// </summary>
        Opt_Participant,
        /// <summary>
        /// 
        /// </summary>
        Non_Participant
    }

    /// <summary>
    /// Participation statues for events
    /// </summary>
    public enum EventPartStatuses
    {
        /// <summary>
        /// Needs action
        /// </summary>
        Needs_Action,
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted,
        /// <summary>
        /// Declined
        /// </summary>
        Declined,
        /// <summary>
        /// Tentative
        /// </summary>
        Tentative,
        /// <summary>
        /// Delegated
        /// </summary>
        Delegated
    }

    /// <summary>
    /// Participation statues for todos
    /// </summary>
    public enum TodoPartStatuses
    {
        /// <summary>
        /// Needs action
        /// </summary>
        Needs_Action,
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted,
        /// <summary>
        /// Declined
        /// </summary>
        Declined,
        /// <summary>
        /// Tentative
        /// </summary>
        Tentative,
        /// <summary>
        /// Delegated
        /// </summary>
        Delegated,
        /// <summary>
        /// Completed
        /// </summary>
        Completed,
        /// <summary>
        /// In process
        /// </summary>
        In_Process
    }

    /// <summary>
    /// Participation statues for journal
    /// </summary>
    public enum JournalPartStatuses
    {
        /// <summary>
        /// Needs action
        /// </summary>
        Needs_Action,
        /// <summary>
        /// Accepted
        /// </summary>
        Accepted,
        /// <summary>
        /// Declined
        /// </summary>
        Declined
    }

    /// <summary>
    /// Ranges
    /// </summary>
    public enum Ranges
    {
        /// <summary>
        /// 
        /// </summary>
        ThisAndPrior,
        /// <summary>
        /// 
        /// </summary>
        ThisAndFuture
    }

    /// <summary>
    /// Relation types
    /// </summary>
    public enum RelationTypes
    {
        /// <summary>
        /// 
        /// </summary>
        Parent,
        /// <summary>
        /// 
        /// </summary>
        Child,
        /// <summary>
        /// 
        /// </summary>
        Sibling
    }

    /// <summary>
    /// Trigger relation
    /// </summary>
    public enum Relateds
    {
        /// <summary>
        /// 
        /// </summary>
        Start,
        /// <summary>
        /// 
        /// </summary>
        End
    }

    /// <summary>
    /// Actions of an alarm
    /// </summary>
    public enum AlarmActions
    {
        /// <summary>
        /// Audio
        /// </summary>
        Audio,
        /// <summary>
        /// Display
        /// </summary>
        Display,
        /// <summary>
        /// Email
        /// </summary>
        Email,
        /// <summary>
        /// Procedure
        /// </summary>
        Procedure
    }
}
