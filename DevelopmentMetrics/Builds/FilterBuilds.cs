﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace DevelopmentMetrics.Builds
{
    public class FilterBuilds
    {
        private readonly List<Build> _builds;

        public FilterBuilds(List<Build> builds)
        {
            _builds = builds;
        }

        public List<Build> Filter(BuildFilter buildFilter)
        {
            var temp = _builds.Where(b =>
                    b.AgentName.Equals(buildFilter.BuildAgent, StringComparison.InvariantCultureIgnoreCase)
                    || buildFilter.BuildAgent.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            temp = temp.Where(b =>
                    b.BuildTypeId.StartsWith(buildFilter.BuildTypeId, StringComparison.InvariantCultureIgnoreCase)
                    || buildFilter.BuildTypeId.Equals("All", StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            return temp;
        }

        public List<Build> GetBuildsForOneWeekFrom(DateTime startDate)
        {
            var endDate = startDate.AddDays(7);

            return _builds
                .Where(b =>
                    b.StartDateTime >= startDate
                    && b.StartDateTime < endDate)
                .ToList();
        }

        public List<Build> GetBuildsFor(BuildGroup buildGroup)
        {
            return _builds
                .Where(b => b.BuildTypeId.StartsWith(buildGroup.BuildTypeGroup))
                .ToList();
        }
    }
}