namespace dMobile.BarCodeScanner
{
    class BarCodeParseCode_2_of_5Factory : BarCodeParseFactory
    {
        public override BarCodeParse GetBarCodeParse()
        {
            return new BarCodeParseCode_2_of_5();
        }
    }
}
