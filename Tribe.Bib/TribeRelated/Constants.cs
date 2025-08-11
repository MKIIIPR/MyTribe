using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tribe.Bib.Models.TribeRelated
{
    public static class Constants
    {
        public static class ProfileTypes
        {
            public const string Regular = "Regular";
            public const string Creator = "Creator";
            public const string Admin = "Admin";
        }

        public static class RaffleStatus
        {
            public const string Draft = "Draft";
            public const string Active = "Active";
            public const string Ended = "Ended";
            public const string Drawn = "Drawn";
            public const string Cancelled = "Cancelled";
        }

        public static class RaffleTypes
        {
            public const string Standard = "Standard";        // 1 Gewinner, einfache Verlosung
            public const string Multiple = "Multiple";        // Mehrere Gewinner
            public const string TwoStage = "TwoStage";        // Erst Qualifikation, dann Final
            public const string ThreeStage = "ThreeStage";    // Qualification -> Semi -> Final
            public const string Instant = "Instant";          // Sofortige Gewinnchance
            public const string Progressive = "Progressive";   // Jackpot w�chst �ber Zeit
        }

        public static class RequirementTypes
        {
            public const string Free = "Free";               // Kostenlose Teilnahme
            public const string Token = "Token";             // Token erforderlich
            public const string Follow = "Follow";           // Follower sein erforderlich
            public const string Both = "Both";               // Token UND Follow
            public const string Either = "Either";           // Token ODER Follow
        }

        public static class EntryTypes
        {
            public const string Free = "Free";               // Kostenlose Entry
            public const string Token = "Token";             // Mit Token bezahlt
            public const string Follow = "Follow";           // �ber Follow qualifiziert
            public const string Sponsored = "Sponsored";     // Gesponserte Entry
        }

        public static class RaffleStages
        {
            public const string Primary = "Primary";         // Erste Stufe
            public const string Secondary = "Secondary";     // Zweite Stufe
            public const string Final = "Final";             // Finale Stufe
            public const string Qualification = "Qualification"; // Qualifikationsrunde
        }

    }
}
