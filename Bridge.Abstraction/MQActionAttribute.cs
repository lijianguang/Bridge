namespace Bridge
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class MQActionAttribute : Attribute
    {
        private readonly string _action;
        public MQActionAttribute(string action)
        {
            if (string.IsNullOrEmpty(action)) throw new ArgumentException("queueName must not be null or empty.");

            _action = action;
        }
        public string Action { get { return _action; } }
    }
}
