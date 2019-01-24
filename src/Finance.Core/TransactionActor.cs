using System.Collections.Generic;
using Akka.Actor;
using Akka.Persistence;
using Akka.Util.Internal;
using Finance.Common;
using Finance.Core.CardPayment;

namespace Finance.Core
{
    public class TransactionActor : UntypedPersistentActor
    {
        private const string InternalId = "transactor-0ae71adfc9264ee7907cce537541d490";
        private Dictionary<string, IInitTransaction> openedOperations = new Dictionary<string, IInitTransaction>();

        public override string PersistenceId { get; } = InternalId;

        public static string Name => InternalId;

        public static Props Props() => Akka.Actor.Props.Create<TransactionActor>();

        protected override void OnCommand(object message)
        {
            Persist(message, o => { HandleMessage(message); });
        }

        protected override void OnRecover(object message)
        {
            HandleMessage(message);
        }

        private void HandleMessage(object message)
        {
            switch (message)
            {
                case IInitTransaction transaction:
                    openedOperations.TryAdd(transaction.Id, transaction);
                    if (IsRecoveryFinished)
                    {
                        StartTransactions(transaction);
                    }

                    break;
                case CompleteTransaction complete:
                    openedOperations.Remove(complete.Id);
                    if (IsRecoveryFinished)
                    {
                        SaveSnapshot(openedOperations);
                    }
                    Context.Parent.Tell(complete);
                    break;
                case SnapshotOffer offeredSnapshot:
                    openedOperations = offeredSnapshot.Snapshot as Dictionary<string, IInitTransaction>;
                    break;
                case RecoveryCompleted _:
                    openedOperations.ForEach(o => StartTransactions(o.Value));
                    break;
            }
        }

        private static void StartTransactions(IInitTransaction message)
        {
            switch (message)
            {
                case CardPaymentInit init:
                    var actor = Context.ActorOf(CardPaymentActor.Props(), CardPaymentActor.GetNewName(init.Id));
                    actor.Forward(init);
                    break;
            }
        }
    }
}