namespace Finance.External.Ecom.Authorize
{
    public class SuccessAuthorize
    {
        public SuccessAuthorize(bool need3Ds)
        {
            Need3Ds = need3Ds;
        }

        public bool Need3Ds { get; }
    }
}