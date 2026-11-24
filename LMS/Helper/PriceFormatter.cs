using System.Globalization;

namespace LMS.Helper
{
    public static class PriceFormatter
    {
        public const string FreeLabel = "Free";

        /// <summary>
        /// Single rendering rule for a course price, so the home page, the course page and the admin
        /// tables cannot disagree about the same value.
        /// </summary>
        public static string Format(int price)
        {
            return price <= 0
                ? FreeLabel
                : price.ToString(CultureInfo.InvariantCulture);
        }
    }
}
