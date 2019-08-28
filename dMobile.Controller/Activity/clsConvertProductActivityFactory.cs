namespace dMobile.Controller
{
    public class ConvertProductActivityFactory : ActivityFactory
    {
        public override Activity GetActivity()
        {
            return new ConvertProductActivity();
        }
    }
}
