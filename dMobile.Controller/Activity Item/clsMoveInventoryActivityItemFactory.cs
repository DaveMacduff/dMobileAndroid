namespace dMobile.Controller
{
  public class MoveInvActItemFactory : ActItemFactory
	{
		public override ActItem GetActItem(Activity activity)
		{
			return new MoveInvActItem(activity);
		}
	}
}
