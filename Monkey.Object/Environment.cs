using System.Collections.Generic;

namespace Monkey.Object
{
    public class Environment
    {
        private Dictionary<string, IObject> _store;
        private Environment _outer;

        public Environment()
        {
            _store = new Dictionary<string, IObject>();
        }

        public IObject Get(string name)
        {
            return _store.ContainsKey(name) ? _store[name] : _outer?.Get(name);
        }

        public IObject Set(string name, IObject val)
        {
            _store[name] = val;
            return val;
        }

        public static Environment NewEnclosedEnvironment(Environment outer)
        {
            return new Environment
            {
                _store = new Dictionary<string, IObject>(),
                _outer = outer
            };
        }
    }
}