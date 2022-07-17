using System.Globalization;

namespace ATMApp.UI
{
    internal class CulturInfo
    {
        public static implicit operator CulturInfo(CultureInfo v)
        {
            throw new NotImplementedException();
        }
    }
}