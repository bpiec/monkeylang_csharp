using System;

namespace Monkey.Object
{
    public struct HashPair : ICloneable
    {
        public IObject Key { get; set; }
        public IObject Value { get; set; }

        public object Clone()
        {
            return new HashPair
            {
                Value = Value.Clone() as IObject,
                Key = Key.Clone() as IObject
            };
        }
    }
}