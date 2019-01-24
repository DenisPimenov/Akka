using System;
using Finance.Common;

namespace Finance.Core.CardPayment
{
    public class CardPaymentInit : IInitTransaction
    {
        public CardPaymentInit(string id, string card, DateTime dateTime)
        {
            Id = id;
            Card = card;
            DateTime = dateTime;
        }

        public string Id { get; }

        public string Card { get; }

        public DateTime DateTime { get; set; }
    }
}