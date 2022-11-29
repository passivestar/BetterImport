using System;
using System.Collections.Generic;
using System.Linq;

namespace BetterImport
{
    public abstract class Hint
    {
        public static List<Type> GetHintTypes()
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => t.IsSubclassOf(typeof(Hint)) && !t.IsAbstract)
                .ToList();
        }

        public static List<T> MakeInstancesOfAll<T>() where T : Hint
        {
            return GetHintTypes()
                .Where(t => t.IsSubclassOf(typeof(T)))
                .Select(t => (T)System.Activator.CreateInstance(t)).ToList();
        }

        public abstract string Text { get; }
    }
}