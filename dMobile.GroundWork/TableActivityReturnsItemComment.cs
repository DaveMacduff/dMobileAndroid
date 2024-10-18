// <copyright file="TableActivityReturnsItemComment.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.GroundWork
{
    using System;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Bedrock;
    using SQLite;

    public class TableActivityReturnsItemComment : TableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableActivityReturnsItemComment"/> class.
        /// </summary>
        public TableActivityReturnsItemComment()
            : base()
        {
            this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItemComment";
            this.CurrentSubType = SysCommon.ActSubTypes.Returns;
            this.LowerCaseName = "sales order return";
            this.ProperCaseName = "Sales order return";
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        /// <inheritdoc/>
        public override async Task PurgeData()
        {
            try
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging sales return item comments.",
                    true);

                await DatabaseConnection.GetInstance()
                    .SqlConn.ExecuteAsync("DELETE FROM tActivityReturnsItemComment");

                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging sales return item comments complete.",
                    true);
            }
            catch (SQLiteException ex)
            {
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Database exception occurred.",
                    true);
                this.DataAccessStatusOutHandler(
                    this,
                    EventArgs.Empty,
                    "Purging sales return item comments failed.",
                    true);

                throw new Exception(
                    "Database exception occurred. Purging sales return item comments failed.",
                    ex);
            }
        }
    }
}