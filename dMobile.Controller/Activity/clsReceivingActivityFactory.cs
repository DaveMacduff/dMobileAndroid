namespace dMobile.Controller
{
    public class RecvFactory : ActivityFactory
    {
        public override Activity GetActivity()
        {
            return new RecvActivity();
        }
    }
}
