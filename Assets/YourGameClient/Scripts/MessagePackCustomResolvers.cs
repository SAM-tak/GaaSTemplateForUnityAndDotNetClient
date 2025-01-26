using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Formatters;
using YourGameServer.Interface;

namespace YourGameClient.MessagePack
{
    public sealed class CustomResolver : IFormatterResolver
    {
        /// <summary>
        /// The singleton instance that can be used.
        /// </summary>
        public static readonly CustomResolver Instance = new();

        private CustomResolver()
        {
        }

        public IMessagePackFormatter<T> GetFormatter<T>()
        {
            return FormatterCache<T>.Formatter;
        }

        private static class FormatterCache<T>
        {
            public static readonly IMessagePackFormatter<T> Formatter;

            static FormatterCache()
            {
                // Reduce IL2CPP code generate size(don't write long code in <T>)
                Formatter = (IMessagePackFormatter<T>)CustomResolverGetFormatterHelper.GetFormatter(typeof(T));
            }
        }
    }

    internal static class CustomResolverGetFormatterHelper
    {
        private static readonly Dictionary<Type, object> FormatterMap = new() {
            { typeof(IEnumerable<FormalPlayerAccount>), new InterfaceEnumerableFormatter<FormalPlayerAccount>() },
            { typeof(IEnumerable<MaskedPlayerAccount>), new InterfaceEnumerableFormatter<MaskedPlayerAccount>() },
            { typeof(IEnumerable<FormalPlayerProfile>), new InterfaceEnumerableFormatter<FormalPlayerProfile>() },
            { typeof(IEnumerable<MaskedPlayerProfile>), new InterfaceEnumerableFormatter<MaskedPlayerProfile>() },
        };

        internal static object GetFormatter(Type t)
        {
            if(FormatterMap.TryGetValue(t, out var formatter)) return formatter;
            return null;
        }
    }
}
