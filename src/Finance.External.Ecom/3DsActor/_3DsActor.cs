using System;
using System.Transactions;
using Akka.Actor;
using Akka.Event;

namespace Finance.External.Ecom._3DsActor
{
    public class _3DsActor : ReceiveActor
    {
        public _3DsActor(string id)
        {
            Context.System.EventStream.Subscribe<_3DsSuccess>(Self);
            var scheduler = Context.System.Scheduler.ScheduleTellOnceCancelable(TimeSpan.FromMinutes(2), Self,
                new Status.Failure(new Exception()), Self);
            Receive<Status.Failure>(failure => throw new TransactionException());
            Receive<_3DsSuccess>(success =>
            {
                if (success.Id != id)
                    return;
                
                scheduler.Cancel();
                Context.Parent.Tell(success);
            });
        }

        public static Props Props(string id) =>
            Akka.Actor.Props.Create(() => new _3DsActor(id));
    }
    
    internal class _3DsSuccess
    {
        public _3DsSuccess(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}