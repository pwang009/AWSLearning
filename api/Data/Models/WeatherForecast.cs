#nullable disable
using Amazon.DynamoDBv2.DataModel;

namespace api.Data.Models;

[DynamoDBTable("WeatherForecast")]
public class WeatherForecast
{
    [DynamoDBHashKey]
    public string City { get; set; }
     [DynamoDBRangeKey]
    public string Date { get; set; }
    public string Condition { get; set; }
    public int Temperature { get; set; }
}
