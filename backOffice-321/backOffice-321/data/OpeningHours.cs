using System;
using System.Collections.Generic;
using System.Linq;

namespace backOffice_321.data
{
    public class DaySchedule
    {
        public DayOfWeek Day { get; set; }

        public bool IsOpen { get; set; }

        public TimeSpan OpenTime { get; set; }

        public TimeSpan CloseTime { get; set; }

        // Une plage horaire n'est valable que si la fermeture suit l'ouverture dans la même journée
        public bool HasValidRange => IsOpen && CloseTime > OpenTime;
    }

    public class WeeklySchedule
    {
        public List<DaySchedule> Days { get; set; } = new List<DaySchedule>();

        public static WeeklySchedule CreateDefault()
        {
            var schedule = new WeeklySchedule();

            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                bool isSaturday = day == DayOfWeek.Saturday;

                schedule.Days.Add(new DaySchedule
                {
                    Day = day,
                    IsOpen = day != DayOfWeek.Sunday,
                    OpenTime = new TimeSpan(8, 0, 0),
                    CloseTime = isSaturday ? new TimeSpan(12, 0, 0) : new TimeSpan(18, 0, 0)
                });
            }

            return schedule;
        }

        public DaySchedule? GetDay(DayOfWeek day)
        {
            return Days.FirstOrDefault(d => d.Day == day);
        }

        /// <summary>
        /// Indique si le commerce est ouvert à cet instant.
        /// stateText vaut "OUVERT" ou "FERMÉ" (état prêt à afficher),
        /// detailsText donne l'heure de fermeture ou la prochaine ouverture.
        /// </summary>
        public bool IsOpenAt(DateTime moment, out string stateText, out string detailsText)
        {
            DaySchedule? today = GetDay(moment.DayOfWeek);

            if (today != null && today.HasValidRange)
            {
                if (moment.TimeOfDay >= today.OpenTime && moment.TimeOfDay < today.CloseTime)
                {
                    stateText = "OUVERT";
                    detailsText = $"Fermeture à {Format(today.CloseTime)}";
                    return true;
                }

                if (moment.TimeOfDay < today.OpenTime)
                {
                    stateText = "FERMÉ";
                    detailsText = $"Ouverture aujourd'hui à {Format(today.OpenTime)}";
                    return false;
                }
            }

            for (int offset = 1; offset <= 7; offset++)
            {
                DateTime day = moment.Date.AddDays(offset);
                DaySchedule? next = GetDay(day.DayOfWeek);

                if (next != null && next.HasValidRange)
                {
                    stateText = "FERMÉ";
                    detailsText = $"Ouverture {GetDayName(next.Day)} à {Format(next.OpenTime)}";
                    return false;
                }
            }

            stateText = "FERMÉ";
            detailsText = "Aucune ouverture programmée";
            return false;
        }

        public static string GetDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return "lundi";
                case DayOfWeek.Tuesday: return "mardi";
                case DayOfWeek.Wednesday: return "mercredi";
                case DayOfWeek.Thursday: return "jeudi";
                case DayOfWeek.Friday: return "vendredi";
                case DayOfWeek.Saturday: return "samedi";
                default: return "dimanche";
            }
        }

        public static string GetShortDayName(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return "Lun.";
                case DayOfWeek.Tuesday: return "Mar.";
                case DayOfWeek.Wednesday: return "Mer.";
                case DayOfWeek.Thursday: return "Jeu.";
                case DayOfWeek.Friday: return "Ven.";
                case DayOfWeek.Saturday: return "Sam.";
                default: return "Dim.";
            }
        }

        private static string Format(TimeSpan time)
        {
            return time.ToString(@"hh\:mm");
        }
    }
}
