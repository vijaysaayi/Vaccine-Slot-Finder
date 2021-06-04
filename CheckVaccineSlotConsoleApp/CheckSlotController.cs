using CommandDotNet;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace CheckVaccineSlotConsoleApp
{
    public class CheckSlotController
    {
        private readonly ICheckVaccineSlotService _slotFinder;
        private readonly ILogger<CheckSlotController> _logger;

        public CheckSlotController(ICheckVaccineSlotService slotFinder, ILogger<CheckSlotController> logger)
        {
            _slotFinder = slotFinder ?? throw new System.ArgumentNullException(nameof(slotFinder));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        [Command(Name = "showAvailableStates",
                 Usage = "showAvailableStates",
                 Description = "Show available states which have vaccine")]
        public async Task ShowAvailableStates()
        {
            await _slotFinder.ShowAvailableStatesWithCodes();
        }

        [Command(Name = "getStateCode",
                 Usage = "getStateCode -s <stateName> ",
                 Description = "Fetch the state code")]
        public async Task GetStateCode(
        [Option(LongName = "stateName", ShortName = "s",
                Description = "StateName")] string stateName
        )
        {
            var stateCode = await _slotFinder.GetStateCode(stateName);
            _logger.LogInformation($"{stateName} : {stateCode}");
        }

        [Command(Name = "showAvailableDistricts",
                 Usage = "ShowAvailableDistricts  -s <stateName> ",
                 Description = "Show available districts in state which have vaccine")]
        public async Task ShowAvailableDistricts(
            [Option(LongName = "stateName", ShortName = "s",
                Description = "StateName")] string stateName
        )
        {
            await _slotFinder.ShowAllAvailableDistrictsWithCode(stateName);
        }

        [Command(Name = "getDistrictCode",
                 Usage = "getDistrictCode -s <stateName> -d <districtName> ",
                 Description = "Gets the district code")]
        public async Task GetDistrictCodeForState(
        [Option(LongName = "stateName", ShortName = "s",
                Description = "StateName")] string stateName
        ,
        [Option(LongName = "districtName", ShortName = "d",
                Description = "DistrictName")]
        string districtName
        )
        {
            var stateCode = await _slotFinder.GetStateCode(stateName);
            var districtCode = await _slotFinder.GetDistrictCodeForState(stateCode, districtName);
            _logger.LogInformation($"{districtName} : {districtCode}");
        }

        [Command(Name = "findSlots",
                 Usage = "findSlots  -s <stateName> -d <districtName> ",
                 Description = "Find Available slots")]
        public async Task FindSlots(
        [Option(LongName = "stateName", ShortName = "s",
                Description = "StateName")] string stateName
        ,
        [Option(LongName = "districtName", ShortName = "d",
                Description = "DistrictName")]
        string districtName,
        [Option(LongName = "retryAfter", ShortName = "n",
                Description = "Retry after n minutes")]
        int retryAfterMinutes
        )
        {
            var stateCode = await _slotFinder.GetStateCode(stateName);
            var districtCode = await _slotFinder.GetDistrictCodeForState(stateCode, districtName);

            while (true)
            {
                await FindSlotsHelper(stateName, districtName, districtCode);
                Thread.Sleep(retryAfterMinutes * 60 * 1000);
            }
        }

        private async Task FindSlotsHelper(string stateName, string districtName, int districtCode)
        {
            var slots = await _slotFinder.GetAvailableSlots(districtCode);
            var numberOfSessionsAvailable = 0;
            if (slots != null)
            {
                foreach (var center in slots.centers)
                {
                    foreach (var session in center.sessions)
                    {
                        if (session.available_capacity > 0 && session.min_age_limit == 18)
                        {
                            _logger.LogInformation($"{center.name}");
                            _logger.LogInformation($"------------------------");
                            _logger.LogInformation($"Available doses : {session.available_capacity}");
                            _logger.LogInformation($"Vaccine : {session.vaccine}");
                            _logger.LogInformation($"Dose 1 capacity : {session.available_capacity_dose1}");
                            _logger.LogInformation($"Dose 2 capacity : {session.available_capacity_dose2}");
                            _logger.LogInformation("");
                            _logger.LogInformation("");

                            numberOfSessionsAvailable++;
                        }
                    }
                }
            }

            if (numberOfSessionsAvailable == 0)
            {
                _logger.LogError($"No Slots available in {stateName}, {districtName}");
            }
            else
            {
                _logger.LogInformation($"Yeyy !! Found {numberOfSessionsAvailable} slots.");
                _logger.LogInformation($"Book your slots by navigating to  https://selfregistration.cowin.gov.in/ .");
                _logger.LogInformation("");
                _logger.LogInformation("");
            }
        }
    }
}