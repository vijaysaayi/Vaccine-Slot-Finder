using CheckVaccineSlotConsoleApp.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace CheckVaccineSlotConsoleApp
{
    public class CheckVaccineSlotService : ICheckVaccineSlotService
    {
        private readonly HttpClient _http;
        private readonly ILogger<CheckVaccineSlotService> _logger;

        public CheckVaccineSlotService(ILogger<CheckVaccineSlotService> logger)
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.Add("User-Agent", Guid.NewGuid().ToString());
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // States Information
        public async Task ShowAvailableStatesWithCodes()
        {
            var getStatesInformationResponse = await GetStatesInformationResponse();

            if (getStatesInformationResponse != null)
            {
                foreach (var state in getStatesInformationResponse.states)
                {
                    _logger.LogInformation($"{state.state_name} - {state.state_id} ");
                }
            }
        }

        public async Task<int> GetStateCode(string stateName)
        {
            var getStatesInformationResponse = await GetStatesInformationResponse();
            var state = getStatesInformationResponse.states.Where(s => s.state_name == stateName).FirstOrDefault();
            if (state != null)
            {
                return state.state_id;
            }
            return 0;
        }

        private async Task<GetStatesInformationResponse> GetStatesInformationResponse()
        {
            var response = await _http.GetAsync("https://cdn-api.co-vin.in/api/v2/admin/location/states");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var getStatesInformationResponse = JsonConvert.DeserializeObject<GetStatesInformationResponse>(content);
                return getStatesInformationResponse;
            }
            else
            {
                _logger.LogError($"Request failed with status code : {response.StatusCode} , reason : {content}");
                return null;
            }
        }

        // District Information
        public async Task ShowAllAvailableDistrictsWithCode(string stateName)
        {
            var stateCode = await GetStateCode(stateName);
            var getDistrictsInformationResponse = await GetDistrictInformationResponse(stateCode);
            if (getDistrictsInformationResponse != null)
            {
                foreach (var district in getDistrictsInformationResponse.districts)
                {
                    _logger.LogInformation($"{district.district_name} - {district.district_id} ");
                }
            }
        }

        public async Task<int> GetDistrictCodeForState(int stateCode, string districtName)
        {
            var getDistrictsInformationResponse = await GetDistrictInformationResponse(stateCode);
            var district = getDistrictsInformationResponse.districts.Where(s => s.district_name == districtName).FirstOrDefault();
            if (district != null)
            {
                return district.district_id;
            }
            return 0;
        }

        private async Task<GetDistrictsInformationResponse> GetDistrictInformationResponse(int stateCode)
        {
            var response = await _http.GetAsync($"https://cdn-api.co-vin.in/api/v2/admin/location/districts/{stateCode}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var getStatesInformationResponse = JsonConvert.DeserializeObject<GetDistrictsInformationResponse>(content);
                return getStatesInformationResponse;
            }
            else
            {
                _logger.LogError($"Request failed with status code : {response.StatusCode} , reason : {content}");
                return null;
            }
        }

        public async Task<GetAvailableSlotsResponse> GetAvailableSlots(int districtCode)
        {
            var response = await _http.GetAsync($"https://cdn-api.co-vin.in/api/v2/appointment/sessions/public/calendarByDistrict?district_id={districtCode}&date={DateTime.Now.ToString("dd-MM-yyyy")}");
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                var getAvailableSlotsResponse = JsonConvert.DeserializeObject<GetAvailableSlotsResponse>(content);
                return getAvailableSlotsResponse;
            }
            else
            {
                _logger.LogError($"Request failed with status code : {response.StatusCode} , reason : {content}");
                return null;
            }
        }
    }
}