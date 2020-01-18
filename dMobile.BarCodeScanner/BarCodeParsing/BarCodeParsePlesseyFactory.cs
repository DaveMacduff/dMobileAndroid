namespace dMobile.BarCodeScanner
{
    class BarCodeParsePlesseyFactory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParsePlessey();
        }
    }
}
