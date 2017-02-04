using System;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Metrics.Model
{

    public class RangeInfoAttribute : Attribute
    {
        internal RangeInfoAttribute(int minScore, int maxScore)
        {
            this.MinScore = minScore;
            this.MaxScore = maxScore;
        }

        public int MinScore { get; private set; }
        public int MaxScore { get; private set; }
    }

    [Serializable]
    public enum Range
    {
        [RangeInfo(9000, 10000)]
        Master,
        [RangeInfo(6000, 9000)]
        Experimented,
        [RangeInfo(4000, 6000)]
        Beginner,
        [RangeInfo(500, 4000)]
        Rookie
    }

    public static class RangeExtensions
    {
        public static int GetMinScore(this Range u)
        {
            return u.GetAttribute<RangeInfoAttribute>().MinScore;
        }

        public static int GetName(this Range u)
        {
            return u.GetAttribute<RangeInfoAttribute>().MaxScore;
        }
        public static int GetDelta(this Range u)
        {
            RangeInfoAttribute attribute = u.GetAttribute<RangeInfoAttribute>();
            return attribute.MaxScore - attribute.MinScore;
        }
    }

    public static class EnumExtensions
    {
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name)
                .GetCustomAttributes(false)
                .OfType<TAttribute>()
                .SingleOrDefault();
        }
    }
}