namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;

    public class PutAwaySkidActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public PutAwaySkidActivity()
          : base()
        {
            this.FullClassName = typeof(PutAwaySkidActivity).FullName;
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
            return "PutAway Skid";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectHdr(int idx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(int idx)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="isSubmitBatch"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override DataView GetDataView(bool isSubmitBatch)
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="dataGridMain"></param>
        //public override void DesignDataGrid(DataGrid dataGridMain)
        //{
        //  throw new NotImplementedException();
        //}

        public override ObservableCollection<object> DesignGrid(ref Grid dataGridMainActivityHeader, ref ListView dataGridMainActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Refresh(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <remarks></remarks>
        public override void Purge()
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
