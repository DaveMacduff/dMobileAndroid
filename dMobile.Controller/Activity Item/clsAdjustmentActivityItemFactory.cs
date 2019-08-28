namespace dMobile.Controller
{
  public class AdjActItemFactory : ActItemFactory
	{
		public override ActItem GetActItem(Activity activity)
		{
			return new AdjActItem(activity);
		}
	}
}
