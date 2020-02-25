namespace dMobile.GroundWork
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using System.Collections.Generic;

    public class TableLocationMapWarehouseProductScheme : TableBase
    {
        public TableLocationMapWarehouseProductScheme()
            : base()
        {
            this.FullClassName = GetType().FullName;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Request()
        {
            //System.Diagnostics.Stopwatch swTotal = new System.Diagnostics.Stopwatch();
            //System.Diagnostics.Stopwatch swPerRun = new System.Diagnostics.Stopwatch();
            //int runs = 0;
            //swTotal.Start();

            List<string> families = new List<string>();

            try
            {
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes.", true);

                using (SQLiteCommand cmndFamily = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndFamily.CommandText = "SELECT tFamily.* FROM tFamily";
                    cmndFamily.CommandType = CommandType.Text;
                    //cmndFamily.IndexName = "UQ_FamilyOnFamilyCode";

                    using (SQLiteDataReader drFamily = cmndFamily.ExecuteReader())
                    {
                        while (drFamily.Read())
                        {
                            families.Add(drFamily["FamilyCode"].ToString());
                        }
                    }
                }

                for (int i = 0; i < families.Count; ++i)
                {
                    string family = families[i];
                    string reqData = string.Empty;

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("For family {0}", family), true);

                    reqData = this.SendRequest(family);

                    if (0 < reqData.Length)
                    {
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            XmlDocument reqErr = new XmlDocument();
                            reqErr.LoadXml(reqData);
                            throw new Exception(reqErr.DocumentElement["detail"].InnerText);
                        }

                        this.Save(reqData);
                    }
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes complete.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Seeding warehouse product schemes failed.", ex);
            }
            catch (Exception ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Seeding warehouse product schemes failed.", true);
                throw new Exception("Seeding warehouse product schemes failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override int DataStatus()
        {
            int rowsAffected = 0;

            try
            {
                using (SQLiteCommand cmndStatus = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndStatus.CommandText = "SELECT COUNT(*) FROM tLocationMapWarehouseProductScheme";
                    cmndStatus.CommandType = CommandType.Text;
                    rowsAffected = Convert.ToInt32(cmndStatus.ExecuteScalar());

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("{0} warehouse product schemes.", rowsAffected), true);
                }
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                throw new Exception("Database exception occurred. Warehouse product schemes status failed.", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
        {
            try
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes.", true);

                using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
                {
                    cmndPurge.CommandText = "DELETE FROM tLocationMapWarehouseProductScheme";
                    cmndPurge.CommandType = CommandType.Text;
                    cmndPurge.ExecuteNonQuery();
                }

                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
            }
            catch (SQLiteException ex)
            {
                // TODO: Resource strings
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
                this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging warehouse product schemes failed.", true);
                throw new Exception("Database exception occurred. Purge warehouse product schemes failed.", ex);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="reqData"></param>
        /// <remarks></remarks>
        protected override void Save(string reqData)
        {
            SQLiteConnection conn = DatabaseConnection.GetInstance().SQLConn;
            string errMsg = string.Empty;

            using (SQLiteCommand cmndLocMapWhs = conn.CreateCommand())
            using (SQLiteCommand cmndProduct = conn.CreateCommand())
            using (SQLiteCommand cmndScheme = conn.CreateCommand())
            using (SQLiteCommand cmndLocMapProdScheme = conn.CreateCommand())
            using (SQLiteTransaction transaction = conn.BeginTransaction())
            {
                try
                {
                    using (DataSet ds = new DataSet())
                    {
                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Reading received data.", false);

                        ds.ReadXml(SysCommon.GetXmlReader(ref reqData), XmlReadMode.Auto);

                        // TODO: Resource strings
                        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes.", false);

                        if (ds.Tables.Count != 0)
                        {
                            int progMax = Convert.ToInt32(ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count);

                            // TODO: Resource strings
                            this.DataAccessStatusOutHandler(this, EventArgs.Empty, string.Format("Records {0}", ds.Tables["tLocationMapWarehouseProductScheme"].Rows.Count), false);

                            int progValue = 0;
                            int productFKRefno = 0;
                            int locMapWhsFKRefno = 0;
                            int schemeRefno = 0;
                            int locMapWhsRefno = 0;
                            int productRefno = 0;
                            string schemeCode = string.Empty;
                            bool isWhsProdSchemeFound = false;

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, 0, 0, progMax);

                            foreach (DataRow drow in ds.Tables["tLocationMapWarehouseProductScheme"].Rows)
                            {
                                schemeCode = drow["F2"].ToString();
                                errMsg = "Scheme code \'" + schemeCode + "\'";

                                locMapWhsFKRefno = Convert.ToInt32(drow["F3"]);
                                errMsg = "Warehouse FK Refno \'" + locMapWhsFKRefno.ToString() + "\'";

                                productFKRefno = Convert.ToInt32(drow["F1"]);
                                errMsg = "Product FK Refno \'" + productFKRefno.ToString() + "\'";

                                // Find the scheme
                                cmndScheme.CommandText = "SELECT * FROM tProductScheme WHERE ProductSchemeCode = @productSchemeCode";
                                cmndScheme.CommandType = CommandType.Text;
                                //cmndScheme.IndexName = "UQ_ProductSchemeOnProductSchemeCode";
                                cmndScheme.Parameters.Add(new SQLiteParameter("@productSchemeCode", schemeCode));

                                using (SQLiteDataReader drScheme = cmndScheme.ExecuteReader())
                                {
                                    //if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { schemeCode }))
                                    //{
                                    if (drScheme.Read())
                                    {
                                        schemeRefno = Convert.ToInt32(drScheme.GetValue(
                                            (int)SysCommon.ProdSchemeFields.Refno));
                                    }
                                    else
                                    {
                                        throw new Exception("drScheme.Read() failed. Unable to locate the scheme refno using the scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    throw new Exception("Unable to locate the scheme refno using the scheme code in tProductScheme." + Environment.NewLine + errMsg);
                                    //}
                                }

                                // Find the product
                                cmndProduct.CommandText = "SELECT * FROM tProduct WHERE ProductFKRefno = @productFKRefno";
                                cmndProduct.CommandType = CommandType.Text;
                                //cmndProduct.IndexName = "UQ_ProductOnProductFKRefno";
                                cmndProduct.Parameters.Add(new SQLiteParameter("@productFKRefno", Convert.ToInt32(productFKRefno)));

                                using (SQLiteDataReader drProduct = cmndProduct.ExecuteReader())
                                {
                                    //if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { productFKRefno }))
                                    //{
                                    if (drProduct.Read())
                                    {
                                        productRefno = Convert.ToInt32(drProduct.GetValue((int)SysCommon.ProductFields.Refno));
                                    }
                                    else
                                    {
                                        // TODO: Resource strings
                                        throw new Exception("drProduct.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the product refno using the product FK refno in tProduct." + Environment.NewLine + errMsg);
                                    //}
                                }

                                // Find the warehouse
                                cmndLocMapWhs.CommandText = "SELECT * FROM tLocationMapWarehouse WHERE LocationMapWarehouseFKRefno = @locationMapWarehouseFKRefno";
                                cmndLocMapWhs.CommandType = CommandType.Text;
                                //cmndLocMapWhs.IndexName = "UQ_LocationMapWarehouseOnLocationMapWarehouseFKRefno";
                                cmndLocMapWhs.Parameters.Add(new SQLiteParameter("@locationMapWarehouseFKRefno", Convert.ToInt32(locMapWhsFKRefno)));

                                using (SQLiteDataReader drLocMapWhs = cmndLocMapWhs.ExecuteReader())
                                {
                                    //if (drLocMapWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
                                    //{
                                    if (drLocMapWhs.Read())
                                    {
                                        locMapWhsRefno = Convert.ToInt32(drLocMapWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
                                    }
                                    else
                                    {
                                        throw new Exception("drLocMapWhs.Read() failed." + Environment.NewLine + errMsg);
                                    }
                                    //}
                                    //else
                                    //{
                                    //    // TODO: Resource strings
                                    //    throw new Exception("Unable to locate the warehouse refno using the warehouse FK refno in tLocationMapWarehouse." + Environment.NewLine + errMsg);
                                    //}
                                }

                                isWhsProdSchemeFound = false;

                                if (SysEnv.RefreshOpt.RefreshDataChangesOnly)
                                {
                                    cmndLocMapProdScheme.CommandText = "SELECT * FROM tLocationMapWarehouseProductScheme WHERE WarehouseLocationMapRefno = @warehouseLocationMapRefno AND ProductRefno = @productRefno AND ProductSchemeRefno = @productSchemeRefno";
                                    cmndLocMapProdScheme.CommandType = CommandType.Text;
                                    //cmndLocMapProdScheme.IndexName = "UQ_WhsProdSchemeOnWhsRefnoAndProductRefnoAndSchemeRefno";
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(locMapWhsRefno)));
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(productRefno)));
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@productSchemeRefno", Convert.ToInt32(schemeRefno)));

                                    using (SQLiteDataReader drLocMapProdScheme = cmndLocMapProdScheme.ExecuteReader())
                                    {
                                        //if (drScheme.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsRefno, productRefno, schemeRefno }))
                                        if (drLocMapProdScheme.Read())
                                        {
                                            isWhsProdSchemeFound = true;
                                            // Nothing to update because all the fields in the table make up the lookup key.
                                        }
                                    }
                                }

                                if (!isWhsProdSchemeFound)
                                {
                                    cmndLocMapProdScheme.CommandText = "INSERT INTO tLocationMapWarehouseProductScheme (WarehouseLocationMapRefno, ProductRefno, ProductSchemeRefno) VALUES (@warehouseLocationMapRefno, @productRefno, @productSchemeRefno)";
                                    cmndLocMapProdScheme.CommandType = CommandType.Text;
                                    //cmndLocMapProdScheme.IndexName = "UQ_WhsProdSchemeOnWhsRefnoAndProductRefnoAndSchemeRefno";
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@warehouseLocationMapRefno", Convert.ToInt32(locMapWhsRefno)));
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@productRefno", Convert.ToInt32(productRefno)));
                                    cmndLocMapProdScheme.Parameters.Add(new SQLiteParameter("@productSchemeRefno", Convert.ToInt32(schemeRefno)));

                                    cmndLocMapProdScheme.ExecuteNonQuery();

                                    //rec.SetInt32((int)SysCommon.WhsLocMapProdSchemeFields.WhsLocMapRefno, locMapWhsRefno);
                                    //rec.SetInt32((int)SysCommon.WhsLocMapProdSchemeFields.ProdRefno, productRefno);
                                    //rec.SetInt32((int)SysCommon.WhsLocMapProdSchemeFields.ProdSchemeRefno, schemeRefno);
                                    //rs.Insert(rec);
                                }

                                progValue++;

                                if (progValue % 100 == 0)
                                {
                                    this.DataAccessProgressOutHandler(this, EventArgs.Empty, progValue, 0, progMax);
                                }
                            }

                            transaction.Commit();

                            this.DataAccessProgressOutHandler(this, EventArgs.Empty, progMax, 0, progMax);
                        }
                    }

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes complete.", false);
                }
                catch (SQLiteException ex)
                {
                    transaction.Rollback();

                    // TODO: Resource strings
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", false);
                    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Saving warehouse product schemes failed.", false);

                    //// Duplicate key violations
                    //if (ex.NativeError == 25016)
                    //{
                    //    this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Uniqueness violation.", false);
                    //    throw new Exception("Uniqueness violation." + Environment.NewLine + errMsg, ex);
                    //}

                    // TODO: Resource strings
                    throw new Exception("Database exception occurred. Saving warehouse product schemes failed." + Environment.NewLine + errMsg, ex);
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected override string CreateRequest(string strFamilyCode)
        {
            StringBuilder xmlString = new StringBuilder();
            XmlWriter xml = XmlWriter.Create(xmlString);

            XDocument request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>REQUEST WAREHOUSE PRODUCT SCHEMES</action>"
                + SysCommon.GetRequestDefaults(true)
                + ("<family_code>" + strFamilyCode + "</family_code>")
                + "</command>"
                + "</imclient>");

            request.Save(xml);
            xml.Flush();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familyCode"></param>
        /// <returns></returns>
        protected override string SendRequest(string familyCode)
        {
            return this.SendComm(this.CreateRequest(familyCode));
        }
    }
}