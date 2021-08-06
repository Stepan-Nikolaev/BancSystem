using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Currencies
{
    public class Ruble : CurrencyType
    {
        public Ruble()
        {
            PriceCurrency = 0.014f;
            NameCurrency = "Рубль";
        }
    }
}
