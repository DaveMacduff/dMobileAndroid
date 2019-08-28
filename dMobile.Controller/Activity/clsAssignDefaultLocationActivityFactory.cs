namespace dMobile.Controller
{
  public class AssignDefaultLocActivityFactory : ActivityFactory
	{
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		/// <remarks></remarks>
		public override Activity GetActivity()
		{
			return new AssignDefaultLocActivity();
		}
	}
}
