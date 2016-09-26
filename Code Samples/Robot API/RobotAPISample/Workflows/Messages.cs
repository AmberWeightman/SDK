using System;
using System.Linq;

namespace RobotAPISample.Workflows
{
    public enum MessageType
    {
        Error,
        Warning
    }

    public class Message
    {
        public MessageType Type { get; }

        public string Code { get; }

        private string _text;

        private object[] _args;

        public string Text => ToString();

        private Message(MessageType type, string code, string text, object[] args)
        {
            Type = type;
            Code = code;
            _text = text;
            _args = args;
        }
        
        public static Message Create(MessageType type, string code, object[] args = null)
        {
            switch (code)
            {
                case "ERR001":
                    return new Message(type, code, "No records were found in Landonline which matched the selected CT {0}.", args);
                case "ERR002":
                    return new Message(type, code, "Could not find image {0} with minimum accuracy {1}.", args);
                case "WAR001":
                    return new Message(type, code, "Citrix client is unavailable.", args);
                case "WAR002":
                    return new Message(type, code, "Event '{0}' completed with accuracy {1}.", args);
                default:
                    throw new ApplicationException($"Error code {code} is not recognised.");
            }
        }

        public override string ToString()
        {
            return (_args == null || !_args.Any()) ? _text : string.Format(_text, _args);
        }
    }
    
}
