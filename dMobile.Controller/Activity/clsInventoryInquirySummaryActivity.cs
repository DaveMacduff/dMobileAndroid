// <copyright file="clsInventoryInquirySummaryActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;

    /// <summary>
    /// Defines the <see cref="InvInquirySummaryActivity" />.
    /// </summary>
    public class InvInquirySummaryActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvInquirySummaryActivity"/> class.
        /// </summary>
        public InvInquirySummaryActivity()
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
        public override string GetFKRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// The GetControlDescription.
        /// </summary>
        /// <returns>.</returns>
        public override string GetControlDescription()
        {
            return "Inventory Inquiry Summary";
        }

        /// <summary>
        /// The SelectActivity.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            dynamic inventoryInquiry = selectedActivity as dynamic;

            if (inventoryInquiry != null)
            {
                this.HdrFKRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
                this.HdrRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
            }
        }

        /// <summary>
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedItem">The selectedItem<see cref="object"/>.</param>
        /// <returns>.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            dynamic inventoryInquiry = selectedItem as dynamic;

            if (inventoryInquiry != null)
            {
                this.SelectedActivity = inventoryInquiry;
                this.HdrFKRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
                this.HdrRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
            }

            return this.HdrRefno;
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
        /// <param name="GridMainActivity">The GridMainActivity<see cref="Grid"/>.</param>
        /// <param name="ListViewMainActivity">The ListViewMainActivity<see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public override ObservableCollection<object> DesignGrid(ref Grid GridMainActivity, ref ListView ListViewMainActivity)
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

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The Delete.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The PrintLabel.
        /// </summary>
        /// <param name="xmlParam">The xmlParam<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The GetLabelQuantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The ChangeStatus.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns>The <see cref="Task{bool}"/>.</returns>
        public override Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }
    }
}
