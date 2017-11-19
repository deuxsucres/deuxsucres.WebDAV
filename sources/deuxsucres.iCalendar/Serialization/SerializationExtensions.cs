using deuxsucres.iCalendar.Parser;
using deuxsucres.iCalendar.Serialization;
using deuxsucres.iCalendar.Structure;
using System;
using System.Collections.Generic;
using System.Text;

namespace deuxsucres.iCalendar
{
    /// <summary>
    /// Extensions for serialization
    /// </summary>
    public static class SerializationExtensions
    {

        /// <summary>
        /// Check error in a strict mode
        /// </summary>
        public static void CheckStrict(this ICalReader reader, Func<bool> check, Action action)
        {
            if (check?.Invoke() == false && reader?.StrictMode == true)
                action?.Invoke();
        }

        /// <summary>
        /// Check a syntax error
        /// </summary>
        public static void CheckSyntaxError(this ICalReader reader, Func<bool> check, string message)
        {
            reader.CheckStrict(check, () => { throw new CalSyntaxError(message); });
        }

        /// <summary>
        /// Check a syntax error
        /// </summary>
        public static void CheckSyntaxError(this ICalReader reader, Func<bool> check, Func<string> message)
        {
            reader.CheckStrict(check, () => { throw new CalSyntaxError(message.Invoke()); });
        }

        /// <summary>
        /// Make a default property parameter
        /// </summary>
        public static ICalPropertyParameter MakePropertyParameter(this ICalReader reader, string name, string value)
        {
            if (reader == null) return null;
            var prm = reader.CreateDefaultParameter(name);
            if (prm == null) return null;
            if (prm.Deserialize(reader, name, value))
                return prm;
            return null;
        }

        /// <summary>
        /// Make a typed property parameter
        /// </summary>
        public static T MakePropertyParameter<T>(this ICalReader reader, string name, string value) where T : class, ICalPropertyParameter, new()
        {
            var prm = new T();
            if (prm.Deserialize(reader, name, value))
                return prm;
            return null;
        }

    }
}
