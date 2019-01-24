namespace Finance.Common
{
    public class CompleteTransaction
    {
        public CompleteTransaction(string id)
        {
            Id = id;
        }

        public string Id { get; }
    }
}