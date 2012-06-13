using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using System.Reflection;

namespace Ns2Docs.Generator.Static.ViewModel
{

    public class ViewModel<T> : IIndexable, ILiquidizable
    {
        public ViewModel(T obj)
        {
            this.obj = obj;
            registered = new Dictionary<object, Dell>();
        }
        private T obj;
        public delegate object Dell();
        public void Register(string str, Dell dell)
        {
            registered[str] = dell;
        }
        private Dictionary<object, Dell> registered;

        protected virtual void BindProperty(string name)
        {
            PropertyInfo property = obj.GetType().GetProperty(name);
            MethodInfo getter = property.GetGetMethod();
            registered[name] = delegate() { return getter.Invoke(obj, null); };
        }

        protected virtual void BindMethod(string name)
        {
            MethodInfo method = obj.GetType().GetMethod(name);
            registered[name] = delegate() { return method.Invoke(obj, null); };
        }

        public object this[object str]
        {
            get
            {
                return registered[str]();
            }
        }

        public bool ContainsKey(object key)
        {
            return registered.ContainsKey(key);
        }

        public virtual object ToLiquid()
        {
            return this;
        }
    }

    public class StringViewModel : ViewModel<string>
    {
        public StringViewModel(string str)
            : base(str)
        {
            BindProperty("Length");
            Register("Hash", delegate() { return str.GetHashCode(); });
        }


    }
}
