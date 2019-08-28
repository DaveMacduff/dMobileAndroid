/// <summary>
/// This is the abstract activity item class factory.
/// </summary>
/// <remarks></remarks>
namespace dMobile.Controller
{
  public abstract class ActItemFactory
	{
		public abstract ActItem GetActItem(Activity activity);
	}
}
