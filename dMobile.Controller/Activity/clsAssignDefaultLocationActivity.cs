namespace dMobile.Controller
{
    using System;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using Xamarin.Forms;

    /// <summary>
    /// 
    /// </summary>
    public class AssignDefaultLocActivity : Activity
    {
        #region Constructors

        public AssignDefaultLocActivity()
          : base()
        {
            this.FullClassName = typeof(AssignDefaultLocActivity).FullName;
        }

        #endregion

        #region Public Members

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
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.AssignDefaultLocationz);
#else
            return "Assign Default Location";
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idx"></param>
        public override void SelectHdr(int idx)
        {
            this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
            this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override int GetRefnoFromRowIdx(int idx)
        {
            this.FKRefno = (int)this.DataViewHdrs[idx]["ActivityRefno"];
            this.Refno = (int)this.DataViewHdrs[idx]["ActivityRefno"];

            return this.Refno;
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
        ///// <remarks></remarks>
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
            if (!this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.Purge();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="xmlParams"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public override string GetFilter(XDocument data)
        {
            return string.Empty;
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
