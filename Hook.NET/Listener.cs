using System;
using System.Threading.Tasks;

namespace Hook.NET
{
    internal class Listener
    {
        internal readonly int Priority;
        internal readonly Func<object, object> Func;
        internal readonly Func<object, Task<object>> AsyncFunc;
        internal readonly bool IsAsync;
        
        internal Listener(int priority, Func<object, object> func)
        {
            Priority = priority;
            Func = func;
            IsAsync = false;
        }
        
        internal Listener(int priority, Func<object, Task<object>> func)
        {
            Priority = priority;
            AsyncFunc = func;
            IsAsync = true;
        }
    }
}