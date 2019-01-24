using System;
using System.Net.Http;
using Akka.Actor;
using Akka.Persistence;
using Finance.Common;
using Finance.External.Ecom.Authorize;
using Finance.External.Ecom._3DsActor;

namespace Finance.Core.CardPayment
{
    public class CardPaymentActor : ReceivePersistentActor
    {
        private readonly MyState state;
        private IActorRef initiator;

        public CardPaymentActor()
        {
            state = new MyState();
            Become(WaitInit);
            ConfigureRecover();
        }

        public override string PersistenceId => Self.Path.Name;

        public static string GetNewName(string opId) => nameof(CardPaymentActor) + opId;

        public static Props Props() => Akka.Actor.Props.Create<CardPaymentActor>();

        private void WaitInit()
        {
            Command<CardPaymentInit>(init =>
            {
                Persist(init, Init);
            });
        }

        private void WaitAuthorizationCompleted()
        {
            Command<SuccessAuthorize>(authorize => { Persist(authorize, SuccessAuthorization); });
        }

        private void WaitPayment()
        {
        }

        private void Wait3Ds()
        {
            Context.ActorOf(_3DsActor.Props(state.OperationId));
        }

        private void Init(CardPaymentInit m)
        {
            state.ChangeState(m);
            Log.Info($"TRANSACTION CARDPAYMENT STARTED WITH ID {m.Id}");
            Context.ActorOf(EcomAuthorizeActor.Props(m.Id,m.Card));
            Become(WaitAuthorizationCompleted);
        }

        private void SuccessAuthorization(SuccessAuthorize obj)
        {
            state.ChangeState(obj);
            Log.Info($"Success Ecom"); 
            
            initiator?.Tell(obj);
            if (state.CurrentState == CardPaymentState._3Ds)
            {
                Become(Wait3Ds);
                
            }

            if (state.CurrentState == CardPaymentState.Payment)
            {
                Become(WaitPayment);
            }
            
            Context.Parent.Tell(new CompleteTransaction(state.OperationId)); //todo remove
        }

        public void ConfigureRecover()
        {
            Recover<CardPaymentInit>(init => { Init(init); });
            Recover<SuccessAuthorize>(completed => { SuccessAuthorization(completed); });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(5,null, exception =>
            {
                if (exception is TimeoutException || exception is HttpRequestException)
                {
                    return Directive.Restart;
                }

                return Directive.Escalate;
            });
        }
    }

    
}