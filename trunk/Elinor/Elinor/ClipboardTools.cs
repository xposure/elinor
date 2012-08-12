using System;
using System.Globalization;
using System.Windows;

namespace Elinor
{
    class ClipboardTools
    {
        internal static void SetClipboardWrapper(double d)
        {
            if (d > .01)
            {
                Clipboard.SetText(Math.Round(d, 2).ToString(CultureInfo.InvariantCulture));
            }
        }

        internal static double GetSellPrice(double sell, Settings settings)
        {
            if (settings == null) return .0;

            double result = sell;

            if(settings.AdvancedStepSettings)
            {
                result -= (result*settings.SellPercentage > settings.SellThreshold)
                              ? settings.SellThreshold
                              : settings.SellPercentage * result;
            }
            else
            {
                result -= .01;
            }

            return result;
        }

        internal static double GetBuyPrice(double buy, Settings settings)
        {
            if (settings == null) return .0;

            double result = buy;

            if(settings.AdvancedStepSettings)
            {
                result += result*settings.BuyPercentage > settings.BuyThreshold
                              ? settings.BuyThreshold
                              : settings.BuyPercentage*result;

            }
            else
            {
                result += .01;
            }

            return result;
        }
    }
}
