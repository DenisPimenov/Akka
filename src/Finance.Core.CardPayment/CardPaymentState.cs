using System;
using Finance.External.Ecom.Authorize;

namespace Finance.Core.CardPayment
{
    public enum CardPaymentState
    {
        Authorize,
        _3Ds,
        Payment
    }

    public class MyState
    {
        public string OperationId { get; private set; }

        public CardPaymentState CurrentState { get; private set; }

        public object LastCommand { get; private set; }

        public void ChangeState(object command)
        {
            if (command == LastCommand)
                return;
            switch (command)
            {
                case CardPaymentInit m:
                    OperationId = m.Id;
                    CurrentState = CardPaymentState.Authorize;
                    break;
                case SuccessAuthorize m:
                    CurrentState = m.Need3Ds
                        ? CardPaymentState._3Ds
                        : CardPaymentState.Payment;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(command));
            }

            LastCommand = command;
        }
    }
}