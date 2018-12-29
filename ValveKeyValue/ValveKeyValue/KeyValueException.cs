using System;

namespace ValveKeyValue
{
    /// <inheritdoc />
    /// <summary>
    /// General KeyValue exception type.
    /// </summary>
    public class KeyValueException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ValveKeyValue.KeyValueException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public KeyValueException(string message)
            : base(message)
        {
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ValveKeyValue.KeyValueException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public KeyValueException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
