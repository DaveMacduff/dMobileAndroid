// <copyright file="Product.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class Product
    {
        private string productCode;

        public int ProductRefno { get; set; }
        public string ProductDescription { get; set; }
        public string ProductCode { get => this.productCode; set => this.productCode = value; }
    }
}
