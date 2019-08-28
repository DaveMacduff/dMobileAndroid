namespace dMobile.Controller
{
  public class CycleActItemFactory : ActItemFactory
	{
		public override ActItem GetActItem(Activity activity)
		{
			return new CycleActItem(activity);
		}
	}
}
