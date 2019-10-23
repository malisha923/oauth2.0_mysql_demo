using System;
using System.Collections.Specialized;
using System.Reflection;

namespace oauth2._0_mysql_demo
{
    public static class ConfigurationExtensions
    {
        public static T LoadConfig<T>(this NameValueCollection nv, string prefix = null)
            where T : class, new()
        {
            if (nv == null)
                return null;

            T t = new T();

            foreach (PropertyInfo pi in typeof(T).GetProperties())
            {
                if (pi.CanWrite == false)
                    continue;

                var name = string.Format("{0}{1}", prefix, pi.Name);

                var value = nv[name];
                if (value == null)
                    continue;
                try
                {
                    var v = Convert.ChangeType(value, pi.PropertyType);
                    pi.SetValue(t, v);
                }
                catch { }

            }

            return t;
        }


        public static void LoadConfig_Static<T>(this NameValueCollection nv, string prefix = null)
            where T : class, new()
        {
            if (nv == null)
                return;

            foreach (PropertyInfo pi in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (pi.CanWrite == false)
                    continue;

                var name = string.Format("{0}{1}", prefix, pi.Name);

                var value = nv[name];
                if (value == null)
                    continue;
                try
                {
                    var v = Convert.ChangeType(value, pi.PropertyType);
                    pi.SetValue(null, v);
                }
                catch { }

            }
        }


        public static void LoadConfig_Static(this NameValueCollection nv, Type type, string prefix = null)
        {
            if (nv == null)
                return;

            foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (pi.CanWrite == false)
                    continue;

                var name = string.Format("{0}{1}", prefix, pi.Name);

                var value = nv[name];
                if (value == null)
                    continue;
                try
                {
                    var v = Convert.ChangeType(value, pi.PropertyType);
                    pi.SetValue(null, v);
                }
                catch { }

            }
        }


    }
}