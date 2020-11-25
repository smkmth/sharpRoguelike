﻿using sharpRoguelike.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace sharpRoguelike.Core.Systems
{
    public class SchedulingSystem
    {
        private int time;
        private readonly SortedDictionary<int, List<IScheduleable>> scheduleables;

        public SchedulingSystem()
        {
            time = 0;
            scheduleables = new SortedDictionary<int, List<IScheduleable>>();
        }

        public void Add(IScheduleable scheduleable)
        {
            int key = time + scheduleable.Time;
            if (!scheduleables.ContainsKey(key))
            {
                scheduleables.Add(key, new List<IScheduleable>());
            }
            scheduleables[key].Add(scheduleable);


        }


        // Remove a specific object from the schedule.
        // Useful for when an monster is killed to remove it before it's action comes up again.
        public void Remove(IScheduleable scheduleable)
        {
            KeyValuePair<int, List<IScheduleable>> scheduleableListFound = new KeyValuePair<int, List<IScheduleable>>(-1, null);

            foreach (var scheduleablesList in scheduleables)
            {
                if (scheduleablesList.Value.Contains(scheduleable))
                {
                    scheduleableListFound = scheduleablesList;
                    break;
                }
            }
            if (scheduleableListFound.Value != null)
            {
                scheduleableListFound.Value.Remove(scheduleable);
                if (scheduleableListFound.Value.Count <= 0)
                {
                    scheduleables.Remove(scheduleableListFound.Key);
                }
            }
        }

        public IScheduleable Get()
        {
            var firstScheduleableGroup = scheduleables.First();
            var firstScheduleable = firstScheduleableGroup.Value.First();
            Remove(firstScheduleable);
            time = firstScheduleableGroup.Key;
            return firstScheduleable;

        }

        public int GetTime()
        {
            return time;
        }

        public void Clear()
        {
            time = 0;
            scheduleables.Clear();
        }
    }
}
