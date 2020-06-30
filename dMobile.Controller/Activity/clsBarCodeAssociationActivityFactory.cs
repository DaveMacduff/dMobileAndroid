namespace dMobile.Controller
{
    public class BarCodeAssocActivityFactory : ActivityFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Activity GetActivity()
        {
            return new BarCodeAssocActivity();
        }
    }
}
