using System;
using Akka.TestKit.Xunit2;
using Finance.Core.CardPayment;
using Xunit;

namespace Finance.Core.Test
{
    public class UnitTest1: TestKit
    {
        [Fact]
        public void Test1()
        {
            var message = new CardPaymentInit("id", "", DateTime.Now);
            var probe = CreateTestProbe();
            var parent = ActorOfAsTestActorRef<TransactionActor>(TransactionActor.Props(), probe);
            parent.Tell(message);
            probe.ExpectMsgFrom<CardPaymentInit>(probe);
        }
    }
}