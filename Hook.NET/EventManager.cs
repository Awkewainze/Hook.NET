using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hook.NET
{
    internal class EventManager
    {
        private readonly ConcurrentDictionary<object, Listener> _listeners = new ConcurrentDictionary<object, Listener>();

        private readonly string _eventName;
        
        internal EventManager(string eventName)
        {
            this._eventName = eventName;
        }

        internal void Add(object identifier, int priority, Func<object, object> func)
        {
            _listeners.AddOrUpdate(identifier,
                o => new Listener(priority, func),
                (o, listener) => new Listener(priority, func)
            );
        }
        
        internal void Add(object identifier, int priority, Func<object, Task<object>> func)
        {
            _listeners.AddOrUpdate(identifier,
                o => new Listener(priority, func),
                (o, listener) => new Listener(priority, func)
            );
        }
        
        
        /// <summary>
        /// Will still call the async hooks, but will not await their responses
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal List<object> Call(object arg)
        {
            var results = new Dictionary<Listener, object>();
            foreach (var listener in _listeners)
            {
                if (listener.Value.IsAsync)
                {
                    listener.Value.AsyncFunc(arg);
                    continue;
                }
                
                results.Add(listener.Value, listener.Value.Func(arg));
            }
            
            return results
                .OrderBy(x => x.Key.Priority)
                .Select(x => x.Value)
                .Where(x => x != null)
                .ToList();
        }
        
        internal async Task<List<object>> CallAsync(object arg)
        {
            var results = new Dictionary<Listener, object>();
            foreach (var listener in _listeners)
            {
                if (listener.Value.IsAsync)
                {
                    results.Add(listener.Value, await listener.Value.AsyncFunc(arg));
                    continue;
                }
                
                results.Add(listener.Value, listener.Value.Func(arg));
            }

            return results
                .OrderBy(x => x.Key.Priority)
                .Select(x => x.Value)
                .Where(x => x != null)
                .ToList();
        }

        internal void Remove(object identifier)
        {
            _listeners.TryRemove(identifier, out _);
        }
    }
}