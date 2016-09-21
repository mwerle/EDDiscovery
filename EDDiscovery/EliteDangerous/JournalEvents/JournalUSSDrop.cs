﻿using Newtonsoft.Json.Linq;
using System.Linq;

namespace EDDiscovery.EliteDangerous.JournalEvents
{
    //When written: when dropping from Supercruise at a USS
    //Parameters:
    //•	USSType: description of USS
    //•	USSThreat: threat level
    public class JournalUSSDrop : JournalEntry
    {
        public JournalUSSDrop(JObject evt, EDJournalReader reader) : base(evt, JournalTypeEnum.USSDrop, reader)
        {
            USSType = evt.Value<string>("USSType");
            USSThreat = evt.Value<int>("USSThreat");
        }
        public string USSType { get; set; }
        public int USSThreat { get; set; }
    }
}
