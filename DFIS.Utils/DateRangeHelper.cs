using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFIS.Utils
{
    public class DateRange
    {
        public DateTime DateStart { get; private set; }
        public DateTime DateEnd { get; private set; }
        public object Tag { get; private set; }

        public DateRange(DateTime start, DateTime end, object tag = null)
        {
            DateStart = start;
            DateEnd = end;
            Tag = tag;
            if (DateEnd != null && DateEnd < DateStart)
                throw new Exception("End date cannot be lower than start date!");
        }

        public DateRangeIntersect IntersectWith(DateRange range)
        {
            // full independence
            if ((DateStart > range.DateEnd && DateEnd > range.DateEnd)
                ||
                (DateStart < range.DateStart && DateEnd < range.DateStart))
                return new DateRangeIntersect(this, range, DateRangeIntersectType.None);
            DateRangeIntersectType rng = DateRangeIntersectType.None;
            if (DateStart >= range.DateStart && DateStart <= range.DateEnd) rng |= DateRangeIntersectType.Head;
            if (DateEnd >= range.DateStart && DateEnd <= range.DateEnd) rng |= DateRangeIntersectType.Tail;
            if (DateStart <= range.DateStart && DateEnd >= range.DateEnd) rng |= DateRangeIntersectType.Outer;
            if (DateStart >= range.DateStart && DateEnd <= range.DateEnd) rng |= DateRangeIntersectType.Inner;
            return new DateRangeIntersect(this, range, rng);
        }
    }

    public struct DateRangeIntersect
    {
        public DateRange SourceRange;
        public DateRange ComparedRange;
        public DateRangeIntersectType IntersectType;

        public DateRangeIntersect(DateRange sourceRange, DateRange comparedRange, DateRangeIntersectType intersectType)
        {
            SourceRange = sourceRange;
            ComparedRange = comparedRange;
            IntersectType = intersectType;
        }
    }

    public class DateRangeHelper : ICollection<DateRange>
    {
        List<DateRange> _ranges;
        DateRange[] Ranges { get { return _ranges.ToArray(); } }

        public int Count { get { return _ranges.Count; } }

        public bool IsReadOnly { get { return false; } }

        public DateRangeHelper()
        {
            _ranges = new List<DateRange>();
        }

        public ICollection<DateRangeIntersect> IntersectWith(DateRange range)
        {
            var col = new List<DateRangeIntersect>();
            foreach(var rng in _ranges)
            {
                var re = rng.IntersectWith(range);
                if (re.IntersectType == DateRangeIntersectType.None) continue;
                col.Add(re);
            }
            return col;
        }

        public void Add(DateRange item)
        {
            var ix = IntersectWith(item).FirstOrDefault();
            if (ix.IntersectType == DateRangeIntersectType.None)
                _ranges.Add(item);
        }

        public void Clear()
        {
            _ranges.Clear();
        }

        public bool Contains(DateRange item)
        {
            return _ranges.Contains(item);
        }

        public void CopyTo(DateRange[] array, int arrayIndex)
        {
            _ranges.CopyTo(array, arrayIndex);
        }

        public bool Remove(DateRange item)
        {
            return _ranges.Remove(item);
        }

        public IEnumerator<DateRange> GetEnumerator()
        {
            return _ranges.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _ranges.GetEnumerator();
        }
    }

    public enum DateRangeIntersectType
    {
        None = 0,

        Head = 1,
        Tail = 2,

        Inner = 4,
        Outer = 8,

        Complete = 15
    }
}
