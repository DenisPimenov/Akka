using System;
using System.Net.Http;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Pattern;
using Akka.Persistence;

namespace Finance.External.Ecom.Authorize
{
    public static class FakeCounter
    {
        public static int I;
    }

    public class EcomAuthorizeActor : ReceivePersistentActor
    {
        private readonly string id;
        private readonly string card;
        private readonly string url;
        private object lastState;

        public override string PersistenceId => nameof(EcomAuthorizeActor) + id;

        public EcomAuthorizeActor(string id, string card)
        {
            this.id = id;
            this.card = card;
            url = Context.System.Settings.Config.GetString("external.api.ecomUrl");
            Log.Info("Start ECOm");
            
            Command<Status.Failure>(failure => throw failure.Cause);
            Command<SuccessAuthorize>(authorize => { Persist(lastState, s => Context.Parent.Tell(s)); });
            Recover<SuccessAuthorize>(m => lastState = m);
            Recover<RecoveryCompleted>(m => { Task.Run(ProcessAuthorize).PipeTo(Self); });
        }

        private async Task<SuccessAuthorize> ProcessAuthorize()
        {
            if (lastState is SuccessAuthorize success)
            {
                return success;
            }

            var result = await SendToHttp();
            lastState = result;
            return result;
        }

        private Task<SuccessAuthorize> SendToHttp()
        {
            Log.Info($"Request to ecom url {url} with card {card}");
            if (FakeCounter.I < 3)
            {
                FakeCounter.I++;
                throw new HttpRequestException();
            }

            FakeCounter.I = 0;
            return Task.FromResult(new SuccessAuthorize(true));
        }

        public static Props Props(string id, string card)
        {
            return BackoffSupervisor.Props(
                Backoff.OnFailure(
                    Akka.Actor.Props.Create<EcomAuthorizeActor>(id, card),
                    nameof(EcomAuthorizeActor) + id,
                    TimeSpan.FromSeconds(3),
                    TimeSpan.FromSeconds(20),
                    0.2));
        }
    }
}