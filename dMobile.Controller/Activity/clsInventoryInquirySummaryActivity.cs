namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    public class InvInquirySummaryActivity : Activity
    {
        #region Constructors

        public InvInquirySummaryActivity()
            : base()
        {
            this.FullClassName = typeof(AdjActivity).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override Task Purge()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Inventory Inquiry Summary";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override async Task SelectActivity(object selectedActivity)
        {
            dynamic inventoryInquiry = selectedActivity as dynamic;

            if (inventoryInquiry != null)
            {
                this.FKRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
                this.Refno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            dynamic inventoryInquiry = selectedItem as dynamic;

            if (inventoryInquiry != null)
            {
                this.SelectedActivity = inventoryInquiry;
                this.FKRefno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
                this.Refno = Convert.ToInt32(inventoryInquiry.ActivityRefno);
            }

            return this.Refno;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override ObservableCollection<object> Search(string searchText)
        {
            throw new NotImplementedException();
        }

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        ///// <summary>
        /////
        ///// </summary>
        ///// <remarks></remarks>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterData"></param>
        public override async Task Refresh(XDocument filterData)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                await this.Purge();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterData"></param>
        /// <returns></returns>
        public override string GetFilter(XDocument filterData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter()
        {
            return string.Empty;
        }

        #endregion
    }
}
