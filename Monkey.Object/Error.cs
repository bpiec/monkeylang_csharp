namespace Monkey.Object
{
    public struct Error : IObject
    {
        public ObjectType Type => ObjectType.ERROR;

        public string Inspect()
        {
            return "ERROR: " + Message;
        }

        public string Message { get; set; }

        public object Clone()
        {
            return new Error
            {
                Message = Message
            };
        }
    }
}