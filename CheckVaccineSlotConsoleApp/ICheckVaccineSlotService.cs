using CheckVaccineSlotConsoleApp.Models;
using System.Threading.Tasks;

namespace CheckVaccineSlotConsoleApp
{
    public interface ICheckVaccineSlotService
    {
        Task<int> GetDistrictCodeForState(int stateCode, string districtName);

        Task<int> GetStateCode(string stateName);

        Task ShowAllAvailableDistrictsWithCode(string stateName);

        Task ShowAvailableStatesWithCodes();

        Task<GetAvailableSlotsResponse> GetAvailableSlots(int districtCode);
    }
}