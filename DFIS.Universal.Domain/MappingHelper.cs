using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Universal
{

    public static class MappingHelper
    {
        public static IEnumerable<TA> Map<TA>(IEnumerable<object> source) where TA : class
        {
            if (source == null) return null;
            List<TA> _ret = new List<TA>();
            foreach (var obj in source)
                _ret.Add(Map<TA>(obj));
            return _ret;
        }
        public static List<TA> Map<TB, TA>(List<TB> source) where TA : class where TB : class
        {
            if (source == null) return null;
            List<TA> _ret = new List<TA>();
            foreach (var obj in source)
                _ret.Add(Map<TA>(obj));
            return _ret;
        }
        public static void Map<TA, TB>(TA source, TB destination) where TA : class where TB : class
        {
            if (source == null || destination == null) return;
            var typA = source.GetType() ?? typeof(TA);
            var typB = destination.GetType() ?? typeof(TB);
            var pid = typB.GetProperties();
            var fid = typB.GetFields();

            foreach (var pi in pid)
            {
                try
                {
                    var p = typA.GetProperty(pi.Name);
                    if (p != null && pi.CanWrite)
                        pi.SetValue(destination, p.GetValue(source));
                }
                catch { }
            }
            foreach (var fi in fid)
            {
                try
                {
                    var f = typA.GetProperty(fi.Name);
                    if (f != null && fi.IsPublic)
                        fi.SetValue(destination, f.GetValue(source));
                }
                catch { }
            }
        }
        public static TD Map<TD>(object source) where TD : class
        {
            var dest = Activator.CreateInstance<TD>();
            Map(source, dest);
            return dest;
        }
        public static TB Map<TA, TB>(TA source) where TB : class where TA : class
        {
            var dest = Activator.CreateInstance<TB>();
            Map(source, dest);
            return dest;
        }
        public static void Map<TB>(Dictionary<string, object> values, TB destination) where TB : class
        {
            var pbType = typeof(TB);
            foreach (var kv in values)
            {
                var pi = pbType.GetProperty(kv.Key);
                if (pi != null && pi.CanWrite)
                {
                    pi.SetValue(destination, kv.Value);
                }
                var fi = pbType.GetField(kv.Key);
                if (fi != null && fi.IsPublic)
                {
                    fi.SetValue(destination, kv.Value);
                }
            }
        }
        public static TB Map<TB>(Dictionary<string, object> values) where TB : class
        {
            var res = Activator.CreateInstance<TB>();
            Map(values, res);
            return res;
        }
        public static void Map<TA, TB>(IList<TA> source, IList<TB> destination) where TA : class where TB : class
        {
            if (source == null || destination == null) return;
            foreach (var s in source)
            {
                destination.Add(Map<TB>(s));
            }
        }
        public static IList<TB> Map<TA, TB>(IList<TA> source)
        {
            var lst = new List<TB>();
            Map(source, lst);
            return lst;
        }
    }
}
