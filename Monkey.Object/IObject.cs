using System;

namespace Monkey.Object
{
    public interface IObject : ICloneable
    {
        ObjectType Type { get; }

        string Inspect();
    }
}