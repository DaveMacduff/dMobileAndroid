namespace dMobile.Controller
{
    public class InvInquiryActivityFactory : ActivityFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Activity GetActivity()
        {
            return new InvInquiryActivity();
        }
    }
}
