namespace dMobile.Controller
{
    public class RecvActDetailFactory : ActDetailFactory
    {
        public override ActDetail GetActDetail(ActItem activityItem)
        {
            return new RecvActDetail(activityItem);
        }
    }
}
