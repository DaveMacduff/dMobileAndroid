// <copyright file="ActDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using dMobile.Bedrock;
    using dMobile.GroundWork;
    using dMobile.Resources.Models;
    using dMobile.Resources.Types;
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    /// <see cref="ActDetail" /> Activity detail base class.
    /// </summary>
    public abstract class ActDetail : IDataLayerEventPublisher, IDataAccessEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActDetail"/> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        public ActDetail(ActItem activityItem)
        {
            this.ActItem = activityItem;
            this.ActSubType = this.ActItem.ActSubType;
        }

        /// <summary>
        /// On data layer status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        public delegate void OnDataLayerStatusOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        /// On data layer progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        public delegate void OnDataLayerProgressOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        /// On data layer status out event.
        /// </summary>
        public event OnDataLayerStatusOutEventHandler OnDataLayerStatusOut;

        /// <summary>
        /// On data layer progress out event.
        /// </summary>
        public event OnDataLayerProgressOutEventHandler OnDataLayerProgressOut;

        /// <summary>
        /// Gets or sets the new activity detail foreign key reference number.
        /// </summary>
        public int NewDetailFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new item foreign key reference number.
        /// </summary>
        public int NewItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets a value indicating whether is update the view.
        /// </summary>
        public bool IsUpdateView { get; set; } = false;

        /// <summary>
        /// Gets or sets the selected location map.
        /// </summary>
        public SLocMap SelectedLocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the data layer event arguments.
        /// </summary>
        public DataLayerEventArgs DataEventArgs { get; set; } = new DataLayerEventArgs();

        /// <summary>
        /// Gets or sets the extra activity detail.
        /// </summary>
        public ActDetailExtra ActExtra { get; set; } = new ActDetailExtra();

        /// <summary>
        /// Gets or sets the activity detail reference number.
        /// </summary>
        public int DetailRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity detail foreign key reference number.
        /// </summary>
        public int DetailFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the product scheme data.
        /// </summary>
        public SProductSchemeData SchemeData { get; set; } = new SProductSchemeData();

        /// <summary>
        /// Gets or sets the unique scheme id.
        /// </summary>
        public string UniqueSchemeID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the scheme id.
        /// </summary>
        public string SchemeID { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the alt quantity.
        /// </summary>
        public decimal Quantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the base pack quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity list details.
        /// </summary>
        public List<object> ListDetails { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the activity sub type.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets the system environment settings.
        /// </summary>
        public SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets or sets the location map.
        /// </summary>
        public LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        /// Gets or sets the ActItem.
        /// </summary>
        public ActItem ActItem { get; set; }

        /// <summary>
        /// Gets or sets the selected activity detail.
        /// </summary>
        public object SelectedActivityDetail { get; set; }

        /// <summary>
        /// The Insert.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Insert(XDocument data);

        /// <summary>
        /// Delete one or a set of activity detail.
        /// </summary>
        /// <returns><see cref="Task{int}"/> Previous non deleted detail foreign key reference number.</returns>
        public abstract Task<int> Delete();

        /// <summary>
        /// Refresh the activity detail.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Refresh();

        /// <summary>
        ///  Create submission XML.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Filter criteria.</param>
        /// <returns>The <see cref="Task{string}"/> Submission XML.</returns>
        public abstract Task<string> CreateSubmitXML(XDocument data);

        /// <summary>
        ///  Delete submission XML.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Filter criteria.</param>
        /// <returns>The <see cref="Task{string}"/> Deletion XML.</returns>
        public abstract Task<string> CreateDeleteXML(XDocument data);

        /// <summary>
        /// Submit the request to the comm server.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <param name="data"><see cref="XDocument"/> XML data to submit.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Submit(object selectedActivityDetail, XDocument data);

        /// <summary>
        /// Perform a search in the detail list for the search text.
        /// </summary>
        /// <param name="searchText"><see cref="string"/> Text to search for a match.</param>
        /// <returns><see cref="ObservableCollection{object}"/> Found activity detail or null.</returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// Get a list of activity details.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is this a batch submission.</param>
        /// <returns><see cref="Task{List{object}}"/>.</returns>
        public abstract Task<List<object>> GetActivityDetailList(bool isSubmitBatch);

        /// <summary>
        /// Design the activity detail grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/> List view of activity details.</param>
        /// <returns><see cref="ObservableCollection{object}"/> Constructed observable collection.</returns>
        public abstract ObservableCollection<object> DesignGrid(
            ref Grid gridMainActivity,
            ref ListView listViewMainActivity);

        /// <summary>
        /// Design activity grid template.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail"/> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{T}"/> Activity items list.</param>
        /// <returns><see cref="Grid"/> Created grid.</returns>
        public abstract DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems);

        /// <summary>
        /// Select an activity detail.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Activity detail to select.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        /// <remarks>
        /// Assigns this.SelectedActivityDetail.
        /// Handles verifying product default location.
        /// Current location is set to the selected skid location.
        /// Clears the selected skid if the line item product changed (Flag).
        /// </remarks>
        public virtual async Task SelectActivityDetail(object selectedActivityDetail)
        {
            if (selectedActivityDetail is ActivityDetail detail)
            {
                this.DetailRefno = detail.ActivityDetailRefno;
                this.DetailFKRefno = detail.ActivityDetailFKRefno;
                this.NewDetailFKRefno = 0;
                this.NewItemFKRefno = 0;

                this.Quantity = detail.ActivityDetailQuantity;
                this.BaseQuantity = detail.ActivityDetailBaseQuantity;
                this.WeightQuantity = detail.ActivityDetailWeightQuantity;

                // SelectedLocMap is used in the activity detail edit for label printing.
                // detail.LocationMapRefno could be a warehouse or even a slot.
                this.SelectedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(detail.LocationMapRefno);

                this.SchemeID = detail.ActivityDetailSchemeID;
                this.UniqueSchemeID = detail.ActivityDetailUniqueSchemeID;
                this.ActExtra.Text1 = detail.ActivityDetailExtra1Text;
                this.ActExtra.Text2 = detail.ActivityDetailExtra2Text;
                this.ActExtra.Text3 = detail.ActivityDetailExtra3Text;
                this.ActExtra.Text4 = detail.ActivityDetailExtra4Text;
                if (detail.ActivityDetailExtra5Date == null ||
                    detail.ActivityDetailExtra5Date <= SysCommon.EmptyDateTime)
                {
                    this.ActExtra.DateTime1 = SysCommon.EmptyDateTime;
                }
                else
                {
                    this.ActExtra.DateTime1 = detail.ActivityDetailExtra5Date;
                }
            }
            else
            {
                this.ClearProperties();
            }
        }

        /// <summary>
        /// Get the activity detail reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="int"/> Activity detail reference number.</returns>
        public abstract int GetRefno(object selectedActivityDetail);

        /// <summary>
        /// Get the activity detail foreign key reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <returns><see cref="int"/> Activity detail foreign key reference number.</returns>
        public abstract int GetFKRefno(object selectedActivityDetail);

        /// <summary>
        /// Insert a skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information to insert.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task InsertSkid(SysSkidInfo skidInfo);

        /// <summary>
        /// Delete a skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo"/> Skid information to delete.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task DeleteSkid(SysSkidInfo skidInfo);

        /// <summary>
        /// Get the activity detail description.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public abstract string GetControlDescription();

        /// <summary>
        /// Total all activity detail quantity from the data view.
        /// </summary>
        /// <returns><see cref="SFillQuantity"/> Total quantities.</returns>
        public virtual async Task<SFillQuantity> GetDataViewTotalQuantity()
        {
            SFillQuantity fillQty = new SFillQuantity();
            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (this.ListDetails != null && this.ListDetails.Count > 0)
                {
                    foreach (ActivityDetail detail in this.ListDetails)
                    {
                        fillQty.Quantity += Convert.ToDecimal(detail.ActivityDetailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(detail.ActivityDetailBaseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(detail.ActivityDetailWeightQuantity);
                    }
                }

                return Task.CompletedTask;
            });

            return fillQty;
        }

        /// <summary>
        /// Clear the activity properties.
        /// </summary>
        public virtual void ClearProperties()
        {
            this.DetailRefno = 0;
            this.DetailFKRefno = 0;
            this.NewItemFKRefno = 0;
            this.NewDetailFKRefno = 0;
            this.UniqueSchemeID = string.Empty;
            this.Quantity = 0;
            this.BaseQuantity = 0;
            this.WeightQuantity = 0;
            this.SchemeID = string.Empty;
            this.SchemeData = new SProductSchemeData();
            this.ActExtra.Text1 = string.Empty;
            this.ActExtra.Text2 = string.Empty;
            this.ActExtra.Text3 = string.Empty;
            this.ActExtra.Text4 = string.Empty;
            this.ActExtra.DateTime1 = SysCommon.EmptyDateTime;
            ////this.SelectedLocMap = new SLocMap();
            ////this.SelectedActivityDetail = null;
        }

        /// <summary>
        /// Get the activity detail information.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object"/> Selected activity detail.</param>
        /// <param name="isAdd"><see cref="bool"/> Is adding activity detail.</param>
        /// <returns><see cref="Task{ActDetail}"/> Activity detail.</returns>
        public virtual async Task<ActDetail> GetInfo(object selectedActivityDetail, bool isAdd)
        {
            selectedActivityDetail = selectedActivityDetail ??
                throw new ArgumentNullException(nameof(selectedActivityDetail));
            SProductSchemeData tmpSchemeData = this.SchemeData;

            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (isAdd)
                {
                    this.ClearProperties();
                    tmpSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
                    tmpSchemeData.LogisticUnitProductionDate = DateTime.Now;
                    tmpSchemeData.SingleTradeItemSerialNumber = string.Empty;
                    tmpSchemeData.LogisticUnitBatchLotNumber = string.Empty;
                    tmpSchemeData.VariantCode = string.Empty;
                    tmpSchemeData.VariantNumber = 0;
                }
                else
                {
                    ActivityDetail detail = (ActivityDetail)selectedActivityDetail;

                    this.Quantity = Convert.ToDecimal(detail.ActivityDetailQuantity);
                    this.BaseQuantity = Convert.ToDecimal(detail.ActivityDetailBaseQuantity);
                    this.WeightQuantity = Convert.ToDecimal(detail.ActivityDetailWeightQuantity);
                    this.DetailRefno = Convert.ToInt32(detail.ActivityDetailRefno);
                    this.SchemeID = Convert.ToString(detail.ActivityDetailSchemeID);
                    this.UniqueSchemeID = Convert.ToString(detail.ActivityDetailUniqueSchemeID);

                    // Check for DBNull because extra detail is in a child table, left join from the data query.
                    if (detail.ActivityDetailExtra1Text != null)
                    {
                        this.ActExtra.Text1 = Convert.ToString(detail.ActivityDetailExtra1Text);
                    }

                    if (detail.ActivityDetailExtra2Text != null)
                    {
                        this.ActExtra.Text2 = Convert.ToString(detail.ActivityDetailExtra2Text);
                    }

                    if (detail.ActivityDetailExtra3Text != null)
                    {
                        this.ActExtra.Text3 = Convert.ToString(detail.ActivityDetailExtra3Text);
                    }

                    if (detail.ActivityDetailExtra4Text != null)
                    {
                        this.ActExtra.Text4 = Convert.ToString(detail.ActivityDetailExtra4Text);
                    }

                    if (detail.ActivityDetailExtra5Date != null &&
                        !string.IsNullOrEmpty(Convert.ToString(detail.ActivityDetailExtra5Date)))
                    {
                        this.ActExtra.DateTime1 = Convert.ToDateTime(detail.ActivityDetailExtra5Date);
                    }

                    // Default best before date to today.
                    DateTime bestBefore = Convert.ToDateTime(detail.ActivityDetailLogisticUnitBestBeforeSellByDate);
                    tmpSchemeData.LogisticUnitBestBeforeSellByDate = SysCommon.IsEmptyDateTime(bestBefore) &&
                        this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTrackedDefault
                        ? DateTime.Now
                        : bestBefore;

                    // Default production date to today.
                    DateTime production = Convert.ToDateTime(detail.ActivityDetailLogisticUnitProductionDate);
                    tmpSchemeData.LogisticUnitProductionDate = SysCommon.IsEmptyDateTime(production) &&
                        this.ActItem.ProdInfo.Scheme.IsProductionDateTrackedDefault
                        ? DateTime.Now
                        : production;

                    tmpSchemeData.SingleTradeItemSerialNumber = Convert.ToString(detail.ActivityDetailSingleTradeItemSerialNumber);
                    tmpSchemeData.LogisticUnitBatchLotNumber = Convert.ToString(detail.ActivityDetailLogisticUnitBatchLotNumber);

                    // Check for DBNull because variants are in a child table, left join from the data query.
                    if (detail.ProductSchemeVariantCode != null)
                    {
                        tmpSchemeData.VariantCode = Convert.ToString(detail.ProductSchemeVariantCode);
                        tmpSchemeData.VariantNumber = Convert.ToInt32(detail.ProductSchemeVariantNo);
                    }
                }

                this.SchemeData = tmpSchemeData;
            });

            return this;
        }

        /// <summary>
        /// Check if the activity detail can be applied.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity detail.</param>
        /// <returns><see cref="bool"/> True if the detail is ready to be applied else false.</returns>
        public virtual async Task IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeID(this.UniqueSchemeID);

                    if (isDupSchemeId)
                    {
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeID);
                    }
                }
            }
        }

        /// <summary>
        /// Create a submit product label XML request.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Data to use to populate label.</param>
        /// <returns><see cref="Task{string}"/> Submission string.</returns>
        public virtual Task<string> CreateSubmitProductLabelXML(XDocument data)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get a string formatted xml of default detail values.
        /// </summary>
        /// <returns><see cref="string"/> Activity detail defaults XML string.</returns>
        public virtual string GetRequestDetailDefaults()
        {
            // Init the production and best before date as empty intentional.
            // We want to send a 'blank' date if it is not tracked rather than {01/01/1900} as the empty date.
            string bestBeforeDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                bestBeforeDate = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            string productionDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
            {
                productionDate = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
            }

            string def = "<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>"
                + "<is_bc>" + this.ActItem.ProdInfo.UnitIsBC + "</is_bc>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFKRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.CurrLocMap.SectFKRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFKRefno + "</slot_refno>"
                + "<refno>" + this.ActItem.ActHdr.HdrFKRefno + "</refno>"
                + "<itm_refno>" + this.ActItem.ItemFKRefno + "</itm_refno>"
                + "<ser_refno>" + this.DetailFKRefno + "</ser_refno>"
                + "<alt_qty>" + this.Quantity + "</alt_qty>"
                + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>"
                + "<base_qty>" + this.BaseQuantity + "</base_qty>"
                + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>"
                + "<wt_qty>" + this.WeightQuantity + "</wt_qty>"
                + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>"
                + "<serial>" + this.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<lot_no>" + this.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<pdate>" + productionDate + "</pdate>"
                + "<bdate>" + bestBeforeDate + "</bdate>"
                + "<variant_no>" + this.SchemeData.VariantNumber + "</variant_no>"
                + "<extra1>" + this.ActExtra.Text1 + "</extra1>"
                + "<extra2>" + this.ActExtra.Text2 + "</extra2>"
                + "<extra3>" + this.ActExtra.Text3 + "</extra3>"
                + "<extra4>" + this.ActExtra.Text4 + "</extra4>"
                + "<extra5>" + this.ActExtra.DateTime1 + "</extra5>"
                + "<is_disconnected>" + this.SysEnv.SysSettings.IsDisconnectedMode + "</is_disconnected>";

            return def;
        }

        /// <summary>
        /// Get activity detail XML data.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail"/> Activity detail.</param>
        /// <returns><see cref="XDocument"/> Activity detail XML.</returns>
        public XDocument GetXML(ActDetail actDetail)
        {
            return XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<params>"
                + "<refno></refno>"
                + "<ser_refno></ser_refno>"
                + "<serial>" + actDetail.SchemeData.SingleTradeItemSerialNumber + "</serial>"
                + "<pdate>" + actDetail.SchemeData.LogisticUnitProductionDate + "</pdate>"
                + "<lot_no>" + actDetail.SchemeData.LogisticUnitBatchLotNumber + "</lot_no>"
                + "<bdate>" + actDetail.SchemeData.LogisticUnitBestBeforeSellByDate + "</bdate>"
                + "<variant_code>" + actDetail.SchemeData.VariantCode + "</variant_code>"
                + "<variant_no>" + actDetail.SchemeData.VariantNumber + "</variant_no>"
                + "<alt_qty>" + actDetail.Quantity + "</alt_qty>"
                + "<wt_qty>" + actDetail.WeightQuantity + "</wt_qty>"
                + "<base_qty>" + actDetail.BaseQuantity + "</base_qty>"
                + "<skid_code>" + this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode + "</skid_code>"
                + "<is_batch_print_product_label>" + this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch
                + "</is_batch_print_product_label>"
                + "<extra1>" + actDetail.ActExtra.Text1 + "</extra1>"
                + "<extra2>" + actDetail.ActExtra.Text2 + "</extra2>"
                + "<extra3>" + actDetail.ActExtra.Text3 + "</extra3>"
                + "<extra4>" + actDetail.ActExtra.Text4 + "</extra4>"
                + "<extra5>" + actDetail.ActExtra.DateTime1 + "</extra5>"
                + "</params>");
        }

        /// <summary>
        /// Check if the inventory tracking scheme is complete.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem"/> Activity item.</param>
        /// <returns>.</returns>
        public bool IsSchemeComplete(ActItem actItem)
        {
            bool isComplete = true;

            if (actItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsLotTracked &&
                string.IsNullOrEmpty(this.SchemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsProductionDateTracked &&
                SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsSerialTracked &&
                string.IsNullOrEmpty(this.SchemeData.SingleTradeItemSerialNumber))
            {
                isComplete = false;
            }

            if (actItem.ProdInfo.Scheme.IsVariantTracked && this.SchemeData.VariantNumber == 0)
            {
                isComplete = false;
            }

            return isComplete;
        }

        /// <summary>
        /// Check if the product label scheme is complete.
        /// </summary>
        /// <returns><see cref="bool"/> True if the label scheme is complete else false.</returns>
        public bool IsLabelSchemeComplete()
        {
            return this.IsLabelSchemeComplete(this.SchemeData);
        }

        /// <summary>
        /// Check if the product label scheme is complete.
        /// </summary>
        /// <param name="schemeData"><see cref="SActItemSchemeData"/> Product scheme data.</param>
        /// <returns><see cref="bool"/> True if the label scheme is complete else false.</returns>
        public bool IsLabelSchemeComplete(SProductSchemeData schemeData)
        {
            bool isComplete = true;

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked &&
                SysCommon.IsEmptyDateTime(schemeData.LogisticUnitBestBeforeSellByDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsLotTracked &&
                string.IsNullOrEmpty(schemeData.LogisticUnitBatchLotNumber))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsProductionDateTracked &&
                SysCommon.IsEmptyDateTime(schemeData.LogisticUnitProductionDate))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsSerialTracked &&
                string.IsNullOrEmpty(schemeData.SingleTradeItemSerialNumber))
            {
                isComplete = false;
            }

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsVariantTracked &&
                schemeData.VariantNumber == 0)
            {
                isComplete = false;
            }

            return isComplete;
        }

        /// <summary>
        /// Get the scheme data variant information from the scheme data variant number.
        /// </summary>
        /// <param name="prodSchemeData"><see cref="SActItemSchemeData"/> Product scheme data.</param>
        /// <returns><see cref="Task{SProductSchemeData}"/> ProductSchemeData.</returns>
        public async Task<SProductSchemeData> FillSchemeDataVarInfoFromSchemeDataVarNo(
            SProductSchemeData prodSchemeData)
        {
            if (prodSchemeData.VariantNumber > 0)
            {
                SQLiteAsyncConnection conn = DatabaseConnection.GetInstance().SQLConn;
                List<tProductSchemeVariantModel> results = await conn.QueryAsync<tProductSchemeVariantModel>(
                    "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                    Convert.ToInt32(prodSchemeData.VariantNumber));

                if (results != null && results.Count > 0)
                {
                    prodSchemeData.VariantCode = results[0].ProductSchemeVariantCode;
                    prodSchemeData.VariantDescription = results[0].ProductSchemeVariantDescription;
                }
                else
                {
                    throw new Exception("FillSchemeDataVarInfoFromSchemeDataVarNo in" +
                        " clsActivityDetail returned no results");
                }
            }

            return prodSchemeData;
        }

        /// <summary>
        /// Fill the activity detail object from XML data.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity detail data.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task FillActDetailObjFromXML(XDocument data)
        {
            if (!string.IsNullOrEmpty(data.Root.Element("ser_refno").Value))
            {
                this.DetailFKRefno = int.Parse(data.Root.Element("ser_refno").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("refno").Value))
            {
                this.DetailRefno = int.Parse(data.Root.Element("refno").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("alt_qty").Value))
            {
                this.Quantity = decimal.Parse(data.Root.Element("alt_qty").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("base_qty").Value))
            {
                this.BaseQuantity = decimal.Parse(data.Root.Element("base_qty").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("wt_qty").Value))
            {
                this.WeightQuantity = decimal.Parse(data.Root.Element("wt_qty").Value);
            }

            SProductSchemeData tmpSchemeData = this.SchemeData;

            if (!string.IsNullOrEmpty(data.Root.Element("bdate").Value))
            {
                tmpSchemeData.LogisticUnitBestBeforeSellByDate =
                    Convert.ToDateTime(data.Root.Element("bdate").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("pdate").Value))
            {
                tmpSchemeData.LogisticUnitProductionDate =
                    Convert.ToDateTime(data.Root.Element("pdate").Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("serial").Value))
            {
                tmpSchemeData.SingleTradeItemSerialNumber = data.Root.Element("serial").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("lot_no").Value))
            {
                tmpSchemeData.LogisticUnitBatchLotNumber = data.Root.Element("lot_no").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_code").Value))
            {
                tmpSchemeData.VariantCode = data.Root.Element("variant_code").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_no").Value))
            {
                tmpSchemeData.VariantNumber = int.Parse(data.Root.Element("variant_no").Value);
            }

            tmpSchemeData = await this.FillSchemeDataVarInfoFromSchemeDataVarNo(tmpSchemeData);
            this.SchemeData = tmpSchemeData;

            this.SchemeID = ProductInfoTasks.GetActSchemeIDFromSProdSchemeData(
                this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme,
                this.SchemeData);
            this.UniqueSchemeID = ProductInfoTasks.GetUniqueSchemeIDFromSProdSchemeData(
                this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme,
                this.SchemeData);

            if (!string.IsNullOrEmpty(data.Root.Element("extra1").Value))
            {
                this.ActExtra.Text1 = data.Root.Element("extra1").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra2").Value))
            {
                this.ActExtra.Text2 = data.Root.Element("extra2").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra3").Value))
            {
                this.ActExtra.Text3 = data.Root.Element("extra3").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra4").Value))
            {
                this.ActExtra.Text4 = data.Root.Element("extra4").Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra5").Value))
            {
                this.ActExtra.DateTime1 = Convert.ToDateTime(data.Root.Element("extra5").Value);
            }
        }

        /// <summary>
        /// Insert data view detail.
        /// </summary>
        /// <param name="actDetailFKRefnoFldName"><see cref="string"/> Foreign key reference number field name.</param>
        /// <returns><see cref="ActivityDetail"/> Activity detail inserted.</returns>
        public abstract ActivityDetail InsertListDetail(string actDetailFKRefnoFldName);

        /// <summary>
        /// Insert the activity detail.
        /// </summary>
        /// <param name="sendReq"><see cref="string"/> Submission request.</param>
        /// <returns><see cref="bool"/> True if a new activity detail was added else false.</returns>
        public bool InsertDetail(string sendReq)
        {
            bool isNewDetail = false;
            string reqData = SysCommon.SubmitComm(sendReq);
            if (reqData.Length > 0)
            {
                XmlDocument xmlData = new XmlDocument();
                xmlData.LoadXml(reqData);

                if (reqData.Contains("<message>ERROR</message>"))
                {
                    string errorDetail = xmlData.DocumentElement["detail"].InnerText;

                    if (errorDetail.Contains("MSGBOX-"))
                    {
                        throw new MessageOnlyException(errorDetail);
                    }
                    else
                    {
                        throw new Exception(errorDetail);
                    }
                }

                this.IsUpdateView = true;
                this.NewItemFKRefno = int.Parse(xmlData.DocumentElement["itm_refno"].InnerText);
                this.NewDetailFKRefno = int.Parse(xmlData.DocumentElement["ser_refno"].InnerText);

                // Assign a temporary activity detail until a full activity detail refresh occurs.
                this.DetailRefno = this.NewDetailFKRefno * -1;

                if (this.NewItemFKRefno != this.ActItem.ItemFKRefno || this.NewDetailFKRefno != this.DetailFKRefno)
                {
                    this.ActItem.ItemFKRefno = this.NewItemFKRefno;
                    this.DetailFKRefno = this.NewDetailFKRefno;
                    isNewDetail = true;
                }
            }
            else
            {
                // NOTE: do we need to raise an error here? SubmitComm will raise errors.
                // When will the length be nothing?
                throw new NotImplementedException();
            }

            return isNewDetail;
        }

        /// <summary>
        /// Insert activity detail in disconnected mode.
        /// </summary>
        /// <returns><see cref="int"/> Inserted activity detail reference number.</returns>
        public async Task<int> InsertDetailDisconnected()
        {
            SQLiteAsyncConnection conn = DatabaseConnection.GetInstance().SQLConn;
            int actDetailRefno;
            try
            {
                if (string.Compare(this.UniqueSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeID(
                        this.UniqueSchemeID);
                    if (isDupSchemeId)
                    {
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine +
                            this.UniqueSchemeID);
                    }
                }

                string query = "INSERT INTO tActivityDetail (" +
                    "ActivityDetailSubType, " +
                    "ActivityDetailStatus, " +
                    "ActivityItemRefno, " +
                    "LocationMapRefno, " +
                    "ProductUnitRefno, " +
                    "ActivityDetailUniqueSchemeID, " +
                    "ActivityDetailSchemeID, " +
                    "ActivityDetailQuantity, " +
                    "ActivityDetailBaseQuantity, " +
                    "ActivityDetailWeightQuantity, " +
                    "ActivityDetailSingleTradeItemSerialNumber, " +
                    "ActivityDetailLogisticUnitBatchLotNumber, " +
                    "ActivityDetailLogisticUnitBestBeforeSellByDate, " +
                    "ActivityDetailLogisticUnitExpiryUseByDate, " +
                    "ActivityDetailLogisticUnitProductionDate, " +
                    "ActivityDetailLogisticUnitDueDate, " +
                    "ActivityDetailLogisticUnitPackagingDate, " +
                    "ActivityDetailLogisticUnitSkidNumber" +
                    ") VALUES (" +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?, " +
                    "?" +
                    ")";

                object[] parameters = new object[]
                {
                    Convert.ToInt32(this.ActSubType),
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(this.ActItem.ItemRefno),
                    Convert.ToInt32(this.LocMap.CurrLocMapRefno),
                    Convert.ToInt32(this.ActItem.ProdInfo.UnitRefno),
                    Convert.ToString(this.UniqueSchemeID),
                    Convert.ToString(this.SchemeID),
                    Convert.ToDecimal(this.Quantity),
                    Convert.ToDecimal(this.BaseQuantity),
                    Convert.ToDecimal(this.WeightQuantity),
                    Convert.ToString(this.SchemeData.SingleTradeItemSerialNumber),
                    Convert.ToString(this.SchemeData.LogisticUnitBatchLotNumber),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitExpiryUseByDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitProductionDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitDueDate),
                    Convert.ToDateTime(this.SchemeData.LogisticUnitPackagingDate),
                    Convert.ToString(this.SchemeData.LogisticUnitSkidNumber),
                };

                _ = await conn.ExecuteAsync(query, parameters);

                actDetailRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(
                    "SELECT last_insert_rowid() FROM tActivityDetail"));
                this.DetailFKRefno = actDetailRefno * -1;

                if (this.SchemeData.VariantNumber > 0)
                {
                    int schemeVarRefno = 0;
                    List<tProductSchemeVariantModel> results = await conn.QueryAsync<tProductSchemeVariantModel>(
                        "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                        Convert.ToInt32(this.SchemeData.VariantNumber));

                    if (results != null && results.Count > 0)
                    {
                        schemeVarRefno = Convert.ToInt32(results[0].ProductSchemeVariantRefno);
                        _ = await conn.ExecuteAsync(
                            "INSERT INTO tActivityDetailProductSchemeVariantXRef" +
                                " (ActivityDetailRefno, ProductSchemeVariantRefno) VALUES (?, ?)",
                            Convert.ToInt32(actDetailRefno),
                            Convert.ToInt32(schemeVarRefno));
                    }
                    else
                    {
                        throw new Exception("drSchemeVar.Read() failed.");
                    }
                }

                if (!string.IsNullOrEmpty(this.ActExtra.Text1) ||
                    string.IsNullOrEmpty(this.ActExtra.Text2) ||
                    string.IsNullOrEmpty(this.ActExtra.Text3) ||
                    string.IsNullOrEmpty(this.ActExtra.Text4) ||
                    SysCommon.IsEmptyDate(this.ActExtra.DateTime1))
                {
                    string queryExtra = "INSERT INTO tActivityDetailExtra (" +
                        "ActivityDetailRefno, " +
                        "ActivityDetailSubType, " +
                        "ActivityDetailExtra1Text, " +
                        "ActivityDetailExtra2Text, " +
                        "ActivityDetailExtra3Text, " +
                        "ActivityDetailExtra4Text, " +
                        "ActivityDetailExtra5Date" +
                        ") VALUES (" +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?, " +
                        "?" +
                        ")";

                    object[] parametersExtra = new object[]
                    {
                        Convert.ToInt32(actDetailRefno),
                        Convert.ToInt32(this.ActSubType),
                        Convert.ToString(this.ActExtra.Text1),
                        Convert.ToString(this.ActExtra.Text2),
                        Convert.ToString(this.ActExtra.Text3),
                        Convert.ToString(this.ActExtra.Text4),
                        Convert.ToDateTime(this.ActExtra.DateTime1),
                    };

                    _ = await conn.ExecuteAsync(queryExtra, parametersExtra);
                }
            }
            catch (SQLiteException ex)
            {
                throw new Exception(
                    "Database exception occurred. Inserting detail in disconnected mode failed.",
                    ex);
            }

            return actDetailRefno;
        }

        /// <summary>
        /// Data layer status out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="outMessage"><see cref="string"/> Status out message.</param>
        /// <param name="overrideDisconnectedMode"><see cref="bool"/> Do not raise event if in disconnected mode.</param>
        public void DataLayerStatusOutHandler(
            object sender,
            EventArgs e,
            string outMessage,
            bool overrideDisconnectedMode)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || overrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = overrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = outMessage;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress value.</param>
        /// <param name="minValue"><see cref="int"/> Minimum progress value.</param>
        /// <param name="maxValue"><see cref="int"/> Maximum progress value.</param>
        public void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue, int minValue, int maxValue)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.MinValue = minValue;
                this.DataEventArgs.MaxValue = maxValue;
                this.DataEventArgs.SetMinMaxValue = true;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress value.</param>
        public void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Register data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Event listener.</param>
        public void RegLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut += new OnDataLayerStatusOutEventHandler(listener.OnLayerStatOut);
        }

        /// <summary>
        /// Unregister data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Event listener.</param>
        public void UnregLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut -= new OnDataLayerStatusOutEventHandler(listener.OnLayerStatOut);
        }

        /// <summary>
        /// Register data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Event listener.</param>
        public void RegLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut += new OnDataLayerProgressOutEventHandler(listener.OnLayerProgOut);
        }

        /// <summary>
        /// Unregister data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Event listener.</param>
        public void UnregLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut -= new OnDataLayerProgressOutEventHandler(listener.OnLayerProgOut);
        }

        /// <summary>
        /// On data access status out event.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Event publisher.</param>
        /// <param name="eventArgs"><see cref="DataAccessEventArgs"/> Data access event arguments.</param>
        public void OnDataAccessStatusOut(IDataAccessEventPublisher publisher, DataAccessEventArgs eventArgs)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || eventArgs.OverrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = eventArgs.OverrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = eventArgs.StatusOutMsg;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// On data access progress out event.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Event publisher.</param>
        /// <param name="eventArgs"><see cref="DataAccessEventArgs"/> Data access event arguments.</param>
        public void OnDataAccessProgressOut(IDataAccessEventPublisher publisher, DataAccessEventArgs eventArgs)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.OverrideDisconnectedMode = eventArgs.OverrideDisconnectedMode;
                this.DataEventArgs.CurrentValue = eventArgs.CurrentValue;
                this.DataEventArgs.MinValue = eventArgs.MinValue;
                this.DataEventArgs.MaxValue = eventArgs.MaxValue;
                this.DataEventArgs.SetMinMaxValue = eventArgs.SetMinMaxValue;

                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }
    }
}
