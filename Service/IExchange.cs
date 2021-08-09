using BancSystem.Currencies;
using System;
using System.Collections.Generic;
using System.Text;

namespace BancSystem.Service
{
    public interface IExchange
    {
        public double ConverterCurrency<T>(int countMoney, T firstCurrency, T secondCurrency) where T : CurrencyType;
    }
}
