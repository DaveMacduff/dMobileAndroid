namespace dMobile.Controller
{
    using System;
    using System.Data;
    using System.Windows.Forms;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class ConvertProductActivity : Activity
    {
        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public ConvertProductActivity()
            : base()
        {
            this.FullClassName = typeof(ConvertProductActivity).FullName;
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
#if MULTILANGUAGE
      return Translator.GetInstance().Translate(TranslatorVocabulary.ConvertUnitsz);
#else
            return "Convert Products";
#endif
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="i"></param>
        /// <param name="lst"></param>
        /// <remarks></remarks>
        public override void SelectHdr(int i)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rowIdx"></param>
        /// <returns></returns>
        public override int GetRefnoFromRowIdx(int rowIdx)
        {
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
        public override DataView Search(string searchText)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataGridMain"></param>
        public override void DesignDataGrid(DataGrid dataGridMain)
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
            return string.Empty;
        }

        #endregion
    }
}
