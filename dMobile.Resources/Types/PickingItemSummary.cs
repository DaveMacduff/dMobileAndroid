namespace dMobile.Resources.Types
{
    public class PickingItemSummary
    {
        private decimal ordered;
        private decimal weightOrdered;
        private decimal shipped;
        private decimal weightShipped;
        private string wtUnitCode;

        public decimal Ordered { get => ordered; set => ordered = value; }
        public decimal WeightOrdered { get => weightOrdered; set => weightOrdered = value; }
        public decimal Shipped { get => shipped; set => shipped = value; }
        public decimal WeightShipped { get => weightShipped; set => weightShipped = value; }
        public string WtUnitCode { get => wtUnitCode; set => wtUnitCode = value; }
    }
}
