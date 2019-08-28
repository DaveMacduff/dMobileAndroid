namespace dMobile.Controller
{
  public class TicketByProdDetailFactory : ActDetailFactory
	{
		public override ActDetail GetActDetail(ActItem actItem)
		{
			return new TicketByProdActDetail(actItem);
		}
	}
}
