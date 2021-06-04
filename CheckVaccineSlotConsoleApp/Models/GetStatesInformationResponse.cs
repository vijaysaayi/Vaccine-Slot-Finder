using System;
using System.Collections.Generic;
using System.Text;

namespace CheckVaccineSlotConsoleApp.Models
{

    public class GetStatesInformationResponse
    {
        public State[] states { get; set; }
        public int ttl { get; set; }
    }

    public class State
    {
        public int state_id { get; set; }
        public string state_name { get; set; }
    }

}
