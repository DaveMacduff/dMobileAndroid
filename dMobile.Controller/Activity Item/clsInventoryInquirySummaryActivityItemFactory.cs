namespace dMobile.Controller
{
    public class InvInquirySummaryActItemFactory : ActItemFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActItem.
        /// </summary>
        /// <param name="activity">Activity to use.</param>
        /// <returns>New InvInquirySummaryActItem</returns>
        public override ActItem GetActItem(Activity activity)
        {
            return new InvInquirySummaryActItem(activity);
        }
    }
}
