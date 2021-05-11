using System;
namespace dMobile.Resources.Types
{
    public class TotalQuantity
    {
        private decimal ordered;
        private decimal received;

        public decimal Ordered { get => ordered; set => ordered = value; }
        public decimal Received { get => received; set => received = value; }
    }
}
