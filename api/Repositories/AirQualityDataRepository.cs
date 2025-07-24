using System;
using System.Security.Cryptography.X509Certificates;
using api.Database;
using api.Models.Dto;
using dotenv.net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace api.Repositories;

public interface IAirQualityDataRepository
{
    Task<AirQualityDataSetDto> GetDataByUID(int uid);
}

public class AirQualityDataRepository : IAirQualityDataRepository
{
    private readonly AirPollutionDbContext _context;

    public AirQualityDataRepository(AirPollutionDbContext context)
    {
        _context = context;
    }

    public async Task<AirQualityDataSetDto> GetDataByUID(int uid)
    {
        var envVars = DotEnv.Read();
        var client = new RestClient();
        var request = new RestRequest(
            $"http://api.waqi.info/feed/@{uid}/?token={envVars["AIR_POLLUTION_API_KEY"]}",
            Method.Get
        );

        Console.WriteLine(request);
        var response = await client.ExecuteAsync<RestResponse>(request);
        // var content = await client.GetAsync<AirQualityDataSetDto>(request);

        var jsonResult = response.Content;

        if (jsonResult != null)
        {
            JObject parsedJsonResult = JObject.Parse(jsonResult);
            AirQualityDataSetDto? uniqueAirQualityData =
                parsedJsonResult.ToObject<AirQualityDataSetDto>();
            var uniqueAirQuaility = new AirQualityDataSetDto();

            if (uniqueAirQualityData != null)
            {
                Console.WriteLine(uniqueAirQualityData.Data?.City?.Name);

                return uniqueAirQualityData;
            }
        }

        throw new Exception($"No Station found with UID {uid}");
    }
}
