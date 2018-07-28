using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hook.NET
{
    public static class Hook
    {
        private static readonly ConcurrentDictionary<string, EventManager> HookManager = new ConcurrentDictionary<string, EventManager>();
        
        public static void Add(string eventName, object identifier, int priority, Func<object, object> func)
        {
            var eventManager = HookManager.GetOrAdd(eventName, CreateNewEventManager);
            eventManager.Add(identifier, priority, func);
        }
        
        public static void Add(string eventName, object identifier, int priority, Func<object, Task<object>> func)
        {
            var eventManager = HookManager.GetOrAdd(eventName, CreateNewEventManager);
            eventManager.Add(identifier, priority, func);
        }

        public static List<object> Call(string eventName, object arg)
        {
            var eventManager = HookManager.GetOrAdd(eventName, CreateNewEventManager);
            return eventManager.Call(arg);
        }
        
        public static async Task<List<object>> CallAsync(string eventName, object arg)
        {
            var eventManager = HookManager.GetOrAdd(eventName, CreateNewEventManager);
            return await eventManager.CallAsync(arg);
        }

        public static void Remove(string eventName, object identifier)
        {
            var eventManager = HookManager.GetOrAdd(eventName, CreateNewEventManager);
            eventManager.Remove(identifier);
        }

        private static EventManager CreateNewEventManager(string eventName)
        {
            return new EventManager(eventName);
        }
    }
}