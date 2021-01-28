namespace dMobile.Controller
{
    public class POWorkSheetActDetailFactory : ActDetailFactory
    {
        public override ActDetail GetActDetail(ActItem actItem)
        {
            return new POWorkSheetActDetail(actItem);
        }
    }
}
