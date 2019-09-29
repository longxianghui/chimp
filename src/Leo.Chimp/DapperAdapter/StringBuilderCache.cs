using System;
using System.Collections.Generic;
using System.Text;

namespace Leo.Chimp.DapperAdapter
{
    public static class StringBuilderCache
    {
        [ThreadStatic]
        private static StringBuilder _cache;

        public static StringBuilder Allocate()
        {
            var sb = _cache;
            if (sb == null)
                return new StringBuilder();

            sb.Length = 0;
            _cache = null;
            return sb;
        }

        public static void Free(StringBuilder sb)
        {
            _cache = sb;
        }

        public static string ReturnAndFree(StringBuilder sb)
        {
            var str = sb.ToString();
            _cache = sb;
            return str;
        }
    }
}
