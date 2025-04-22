// <copyright file="InvInquiryActivitySingle.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;

    /// <summary>
    /// Represents a inventory inquiry activity single.
    /// </summary>
    public class InvInquiryActivitySingle : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvInquiryActivitySingle"/> class.
        /// </summary>
        public InvInquiryActivitySingle()
            : base()
        {
            this.FullClassName = typeof(AdjActivity).FullName;
        }

        /// <summary>
        /// The Purge.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetFKRefnoIdx.
        /// </summary>
        /// <returns>.</returns>
        public override string GetFkRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Inventory Inquiry Single";
        }

        /// <summary>
        /// The SelectActivity.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is InvInquiryActivitySingle inquiry)
            {
                this.SelectedActivity = inquiry;
                this.HdrFkRefno = inquiry.HdrRefno;
                this.HdrRefno = inquiry.HdrRefno;
                this.LocationMapRefno = inquiry.LocationMapRefno;
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        public override void AssignRefnoFromActivity(object selectedItem)
        {
            if (selectedItem is InvInquirySummaryActivity inquiry)
            {
                this.SelectedActivity = inquiry;
                this.HdrFkRefno = Convert.ToInt32(inquiry.HdrRefno);
                this.HdrRefno = Convert.ToInt32(inquiry.HdrRefno);
            }
        }

        /// <summary>
        /// The GetActivityList.
        /// </summary>
        /// <param name="isSubmitBatch">.</param>
        /// <returns>.</returns>
        public override Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Search.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns>.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The DesignGrid.
        /// </summary>
        /// <param name="gridMainActivity">The gridMainActivity<see cref="Grid"/>.</param>
        /// <param name="listViewMainActivity">The listViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// ///// </summary>
        ///// <remarks></remarks>
        // public override void DesignDataGrid(DataGrid dataGridMain)
        // {
        //    throw new NotImplementedException();
        // }

        /// <summary>
        ///.
        /// </summary>
        /// <param name="filterData">.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task Refresh(XDocument filterData)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
            }
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        /// <param name="filterData">.</param>
        public override void GetFilter(XDocument filterData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetFilter.
        /// </summary>
        public override void GetFilter()
        {
        }
    }
}
