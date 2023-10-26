﻿using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TourneyHub.Feature.Tournament.Fields
{
    public class TournamentFields
    {
        public static readonly ID MainTemplateID = new ID("{E0E7EF3E-C3AF-4F45-A8AD-5EDEE6FD40CA}");
        public static readonly ID TournamentParentPageId = new ID("{FE5FAE09-293E-4F42-89AC-A12A314A83E5}");

        public static class TournamentItemNames
        {
            public static readonly string Participants = "Participants";
            public static readonly string TournamentMatch = "Tournament Matches";

        }
        public static class TournamentData
        {
            public static class TournamentDataFolder
            {
                public static readonly ID TournamentFormatFolderId = new ID("{DC5F8F33-6FA8-4F74-B8C5-C1F7492D39DB}");
                public static readonly ID TournamentTypeFolderId = new ID("{AE687964-6861-4591-9A6A-346C883DC44B}");

            }
            public static class TournamentFormat
            {
                public static readonly ID ID = new ID("{1BF1DE3E-503A-4EE8-8F9F-2003331DE272}");
                public static readonly ID TournamentFormatFieldID = new ID("{D6990C04-C69C-46A3-810C-A9101CB53681}");

            }
            public static class TournamentType
            {
                public static readonly ID ID = new ID("{025075C1-A538-451E-9A4F-37E3E47080A6}");
                public static readonly ID TournamentTypeFieldID = new ID("{FB69D4BC-4912-468F-8A57-22341ABF3AED}");

            }
        }



        public static class Templates
        {
            public static class TournamentInfo
            {
                public static class Fields
                {
                    public static readonly ID TournamentNameFieldId = new ID("{26BD58D5-37C8-4273-8B6C-CB6925B41A69}");
                    public static readonly ID SportNameFieldId = new ID("{22A4333F-BB9B-44EE-883E-CC71CC8E99BE}");
                    public static readonly ID TournamentFormatFieldId = new ID("{22595ABE-A3BD-424A-B20C-20D381ECB360}");
                    public static readonly ID TournamentTypeFieldId = new ID("{F2A9144E-6E8B-4878-B05B-B24EA9C6D46A}");
                    public static readonly ID TournamentStartDateFieldId = new ID("{339336AD-4F93-4586-B667-9EC3411DD344}");
                    public static readonly ID TournamentEndDateFieldId = new ID("{E5E1B865-B835-40D5-81A3-3F2A35779E61}");
                    public static readonly ID CreatedByUserFieldId = new ID("{7513ED32-1E5E-4275-8FFF-7361BEE2DA46}");
                }
            }

            public static class TournamentTeam
            {
                public static readonly ID ID = new ID("{BC6E9B3E-8144-4F3E-8430-6EA8AF57F072}");

                public static class Fields
                {
                    public static readonly ID TeamNameFieldId = new ID("{4FFEB6DF-8DF8-44DB-9D45-622890D4C66F}");
                    public static readonly ID TeamLogoFieldId = new ID("{ED9AC297-577F-44FE-A3F5-F9B1AEBAE1D1}");
                    public static readonly ID TeamDescriptionFieldId = new ID("{7BAE3977-FF56-4A7D-AAA7-FEED3532ABEC}");

                }
            }
            public static class Participants
            {
                public static readonly ID ID = new ID("{7BFEE681-F5A8-4C80-BEEE-C93A87D80D27}");

            }
            public static class Participant
            {
                public static readonly ID ID = new ID("{D1146312-3DBA-4CE3-92CB-A5972C2322EC}");

                public static class Fields
                {
                    public static readonly ID NameFieldId = new ID("{A3761E5E-6104-4AC4-B4DC-66B755B57277}");
                    public static readonly ID SurnameFieldId = new ID("{394CFFA9-ADF0-432D-A4CE-54B6FB974251}");
                    public static readonly ID InformationFieldId = new ID("{A5598208-72DF-4624-9600-AEDD2075AD2C}");
                    public static readonly ID AgeFieldId = new ID("{67F1064F-6DD6-4381-AE66-CCEEE8CC931D}");

                }
            }
            public static class TournamentMatches
            {
                public static readonly ID ID = new ID("{F5843129-BB5F-4C87-86E0-2EA1234698A7}");
            }
            public static class TournamentMatch
            {
                public static readonly ID ID = new ID("{3D8E18D8-CDC5-4104-A92B-DF9414AB707B}");

                public static class Fields
                {
                    public static readonly ID FirstParticipantFieldId = new ID("{96650D6F-1B45-4DB9-97E9-245183DDFBE1}");
                    public static readonly ID SecondParticipantFieldId = new ID("{D999F66B-2C9D-4010-A30C-993A53242566}");
                    public static readonly ID ScoreFieldId = new ID("{467C08DC-FA21-499A-A23F-17643EDC20A7}");
                    public static readonly ID WinnerFieldId = new ID("{6D77FD4E-CB7A-4904-9B5F-240237F2F1B0}");
                    public static readonly ID DateOfMatchFieldId = new ID("{19D91E5D-7C96-41E1-B185-C1CCB764D49C}");

                }
            }
        }


    }
}