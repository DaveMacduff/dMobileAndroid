namespace dMobile.Controller
{
    public class ConvertProductActDetailFactory : ActDetailFactory
    {
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new ConvertProductActDetail(actItem);
        }
    }
}
