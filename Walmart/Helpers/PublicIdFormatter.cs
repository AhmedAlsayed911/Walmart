using System.Security.Cryptography;
using System.Text;

namespace Walmart.Helpers
{
    public static class PublicIdFormatter
    {
        public static string ToPublicGuid(int id)
        {
            return ToPublicGuid($"int:{id}");
        }

        public static string ToPublicGuid(string? rawValue)
        {
            var normalized = string.IsNullOrWhiteSpace(rawValue)
                ? "empty"
                : rawValue.Trim().ToLowerInvariant();

            var bytes = MD5.HashData(Encoding.UTF8.GetBytes($"walmart-public-ref:{normalized}"));
            return new Guid(bytes).ToString();
        }
    }
}