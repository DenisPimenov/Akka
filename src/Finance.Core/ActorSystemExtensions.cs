using Akka.Actor;

namespace Finance.Core
{
    public static class ActorSystemExtensions
    {
        private static IActorRef transactionActor;

        public static IActorRef GetTransactionActor(this ActorSystem system)
        {
            return transactionActor ??
                   (transactionActor = system.ActorOf(TransactionActor.Props(), TransactionActor.Name));
        }
    }
}