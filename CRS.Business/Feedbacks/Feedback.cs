namespace CRS.Business.Feedbacks
{
    /// <summary>
    /// Generic version of Feedback class that wraps a single return value of a method
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Feedback<T> : Feedback
    {
        /// <summary>
        /// Gets return value of the method
        /// </summary>
        public T Data { get; private set; }

        public Feedback(bool success, string message = null, T data = default(T))
            : base(success, message)
        {
            Data = data;
        }
    }

    /// <summary>
    /// Serves as return value for methods that need to return success flag and necessary message. Should be inherited for method-specific purpose.
    /// </summary>
    public class Feedback
    {
        /// <summary>
        /// Gets or sets the flag indicating whether the call was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message that the caller should know
        /// </summary>
        public string Message { get; set; }

        public Feedback(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }
    }
}