namespace CheckVaccineSlotConsoleApp.Models
{
    public class GetDistrictsInformationResponse
    {
        public District[] districts { get; set; }
        public int ttl { get; set; }
    }

    public class District
    {
        public int district_id { get; set; }
        public string district_name { get; set; }
    }
}