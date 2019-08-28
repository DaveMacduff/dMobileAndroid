namespace dMobile.Controller
{
    public class InvInquirySummaryActivityFactory : ActivityFactory
    {
        /// <summary>
        /// Constructs and returns a new InvInquirySummaryActivity.
        /// </summary>
        /// <returns>New InvInquirySummaryActivity.</returns>
        public override Activity GetActivity()
        {
            return new InvInquirySummaryActivity();
        }
    }
}
