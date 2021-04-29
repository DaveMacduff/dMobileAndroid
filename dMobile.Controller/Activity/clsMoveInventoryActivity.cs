namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;

    public class MoveInvActivity : Activity
    {
        #region Constructors

        public MoveInvActivity()
          : base()
        {
            this.FullClassName = typeof(MoveInvActivity).FullName;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFKRefnoIdx()
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetControlDescription()
        {
            // TODO: Resource strings
            return "Move Inventory";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        public override Task SelectActivity(object selectedActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            throw new NotImplementedException();
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
        //  throw new NotImplementedException();
        //}

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override Task Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

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
        public override string GetFilter()
        {
            return string.Empty;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
