// <copyright file="ProductLocationMapType.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Resources.Types
{
    public class ProductLocationMapType
    {
        private string productBrandDescription;

        public string ProductCode { get; set; }
        public string LocationMapType { get; set; }
        public string ProductDescription { get; set; }
        public string ProductBrandDescription { get => this.productBrandDescription; set => this.productBrandDescription = value; }
    }
}
