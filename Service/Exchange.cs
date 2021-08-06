using BancSystem.Currencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Service
{
    public class Exchange : IExchange
    {
        public double ConverterCurrency(int countMoney, CurrencyType firstCurrency, CurrencyType secondCurrency)
        {
            return (countMoney * firstCurrency.PriceCurrency) / secondCurrency.PriceCurrency;
        }
    }
}