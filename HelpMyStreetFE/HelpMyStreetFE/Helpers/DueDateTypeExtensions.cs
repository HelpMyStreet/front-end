using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;

namespace HelpMyStreetFE.Helpers
{
    public static class DueDateTypeExtensions
    {
        public static bool HasDate(this DueDateType dueDateType)
        {
            return dueDateType switch
            {
                DueDateType.ASAP => false,
                DueDateType.Before => false,
                DueDateType.On => true,
                DueDateType.SpecificStartTime => true,
                DueDateType.SpecificStartAndEndTimes => true,
                DueDateType.OpenUntil => true,
                _ => throw new ArgumentException($"Unexpected DueDateType value: {dueDateType}", nameof(dueDateType))
            };
        }

        public static bool HasStartTime(this DueDateType dueDateType)
        {
            return dueDateType switch
            {
                DueDateType.ASAP => false,
                DueDateType.Before => false,
                DueDateType.On => false,
                DueDateType.SpecificStartTime => true,
                DueDateType.SpecificStartAndEndTimes => true,
                DueDateType.OpenUntil => false,
                _ => throw new ArgumentException($"Unexpected DueDateType value: {dueDateType}", nameof(dueDateType))
            };
        }

        public static bool HasEndTime(this DueDateType dueDateType)
        {
            return dueDateType switch
            {
                DueDateType.ASAP => false,
                DueDateType.Before => false,
                DueDateType.On => false,
                DueDateType.SpecificStartTime => false,
                DueDateType.SpecificStartAndEndTimes => true,
                DueDateType.OpenUntil => true,
                _ => throw new ArgumentException($"Unexpected DueDateType value: {dueDateType}", nameof(dueDateType))
            };
        }
    }
}
