using BancSystem.Currencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Service
{
    public class Exchange : IExchange
    {
        public double ConverterCurrency<T>(int countMoney, T firstCurrency, T secondCurrency) where T : CurrencyType
        { 
            return (countMoney * firstCurrency.PriceCurrency) / secondCurrency.PriceCurrency;
        }
    }
}