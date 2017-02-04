using System.Collections.Generic;

namespace Assets.Scripts.Metrics.Model
{
    public class MetricsTable
    {
        public float FreeTime { get; private set; }
        public float Exercises { get; private set; }
        public List<RangeMetricsInfo> RangeMetricsInfos { get; private set; }

        public MetricsTable(float freeTime, float exercises, List<RangeMetricsInfo> rangeMetricsInfos)
        {
            FreeTime = freeTime;
            this.Exercises = exercises;
            RangeMetricsInfos = rangeMetricsInfos;
        }
    }

    public class RangeMetricsInfo
    {
        public Range Range1 { get; private set; }
        public float MinErrors { get; private set; }
        public float MaxErrors { get; private set; }
        public float MaxTime { get; private set; }
        public float MaxTimeToBonus { get; private set; }

        public RangeMetricsInfo(Range range, float minErrors, float maxErrors, float maxTime, float maxTimeToBonus)
        {
            Range1 = range;
            MinErrors = minErrors;
            MaxErrors = maxErrors;
            MaxTime = maxTime;
            MaxTimeToBonus = maxTimeToBonus;
        }
    }
}