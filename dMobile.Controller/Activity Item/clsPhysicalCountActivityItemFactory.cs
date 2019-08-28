namespace dMobile.Controller
{
  public class PhysicalActItemFactory : ActItemFactory
	{
		public override ActItem GetActItem(Activity activity)
		{
			return new PhysicalActItem(activity);
		}
	}
}
