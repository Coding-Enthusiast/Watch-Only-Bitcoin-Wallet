namespace WatchOnlyBitcoinWallet.Services
{
    public class Response<T> : Response
    {
        public T Result { get; set; }
    }


    public class Response
    {
        private readonly ErrorCollection errors = new ErrorCollection();

        public ErrorCollection Errors
        {
            get { return errors; }
        }
    }
}
