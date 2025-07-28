
using GamHubApp.Core;
using System.Security.Cryptography;
using System.Text;

namespace GamHubApp.Services;

public class Csign
{
    public static string GenerateApiKey()
    {
        DateTime date = DateTime.Now;

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(AppConstant.ShaSalt));
        return Convert.ToHexStringLower(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{{token:{AppConstant.MonitoringKey},d:{date.Day},m:{date.Month - 1},y:{date.Year},h:{date.Hour}}}")));
    }
}
