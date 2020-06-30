namespace dMobile.Controller
{
    public class ReleaseByOrderActivityFactory : ActivityFactory
    {
        public override Activity GetActivity()
        {
            return new ReleaseByOrderActivity();
        }
    }
}
