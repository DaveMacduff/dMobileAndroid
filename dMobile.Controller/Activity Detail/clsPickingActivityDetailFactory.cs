namespace dMobile.Controller
{
    public class PickingActDetailFactory : ActDetailFactory
    {
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new PickingActDetail(actItem);
        }
    }
}
