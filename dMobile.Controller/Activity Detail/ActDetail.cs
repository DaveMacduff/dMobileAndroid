// <copyright file="ActDetail.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Detail
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Linq;
    using Activity_Item;
    using Bedrock;
    using GroundWork;
    using Resources.Models.Products;
    using Resources.Types.Details;
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    ///     <see cref="ActDetail" /> Activity detail base class.
    /// </summary>
    public abstract class ActDetail : IDataLayerEventPublisher, IDataAccess
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ActDetail" /> class.
        /// </summary>
        /// <param name="activityItem">.</param>
        protected ActDetail(ActItem activityItem)
        {
            this.ActItem = activityItem;
            this.ActSubType = this.ActItem.ActSubType;
        }

        /// <summary>
        ///     On data layer progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher" /> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs" /> Data layer event arguments.</param>
        public delegate void DataLayerProgressOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        ///     On data layer status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher" /> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs" /> Data layer event arguments.</param>
        public delegate void DataLayerStatusOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        ///     On data layer status out event.
        /// </summary>
        public event DataLayerStatusOutEventHandler OnDataLayerStatusOut;

        /// <summary>
        ///     On data layer progress out event.
        /// </summary>
        public event DataLayerProgressOutEventHandler OnDataLayerProgressOut;

        public bool Flag { get; set; }

        public Color ItemBackColor => Flag ? Color.Green : Color.Red;

        /// <summary>
        ///     Gets or sets the full class name.
        /// </summary>
        public string FullClassName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the selected location map.
        /// </summary>
        public SLocMap SelectedLocMap { get; set; } = new SLocMap();

        /// <summary>
        ///     Gets or sets the data layer event arguments.
        /// </summary>
        public DataLayerEventArgs DataEventArgs { get; set; } = new DataLayerEventArgs();

        /// <summary>
        ///     Gets or sets the extra activity detail.
        /// </summary>
        public ActDetailExtra ActExtra { get; set; } = new ActDetailExtra();

        /// <summary>
        ///     Gets or sets the activity detail reference number.
        /// </summary>
        public int DetailRefno { get; set; }

        /// <summary>
        ///     Gets or sets the activity detail foreign key reference number.
        /// </summary>
        public int DetailFkRefno { get; set; }

        /// <summary>
        ///     Gets or sets the activity detail status.
        /// </summary>
        public SysCommon.ActStatus ActStatus { get; set; } = SysCommon.ActStatus.NoChange;

        /// <summary>
        ///     Gets or sets the product scheme data.
        /// </summary>
        public ProductSchemeData SchemeData { get; set; } = new ProductSchemeData();

        /// <summary>
        ///     Gets or sets the product label scheme data.
        /// </summary>
        public ProductSchemeData LabelSchemeData { get; set; } = new ProductSchemeData();

        /// <summary>
        ///     Gets or sets the unique scheme id.
        /// </summary>
        public string UniqueSchemeId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the scheme id.
        /// </summary>
        public string SchemeId { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the alt quantity.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        ///     Gets or sets the base pack quantity.
        /// </summary>
        public decimal BaseQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the weight quantity.
        /// </summary>
        public decimal WeightQuantity { get; set; }

        /// <summary>
        ///     Gets or sets the activity list details.
        /// </summary>
        public List<object> ListDetails { get; set; } = new List<object>();

        /// <summary>
        ///     Gets or sets the activity sub type.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        ///     Gets or sets the system environment settings.
        /// </summary>
        public SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        ///     Gets or sets the location map.
        /// </summary>
        public LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        ///     Gets or sets the ActItem.
        /// </summary>
        public ActItem ActItem { get; set; }

        /// <summary>
        ///     Gets or sets the selected activity detail.
        /// </summary>
        public object SelectedDetail { get; set; }

        /// <summary>
        ///     Gets or sets the list item current index.
        /// </summary>
        public int SelectedListIdx { get; set; } = -1;

        /// <summary>
        ///     Get the scheme data variant information from the scheme data variant number.
        /// </summary>
        /// <param name="prodSchemeData"><see cref="ActItemSchemeData" /> Product scheme data.</param>
        /// <returns><see cref="Task{SProductSchemeData}" /> ProductSchemeData.</returns>
        public static async Task<ProductSchemeData> FillSchemeDataVarInfoFromSchemeDataVarNo(
            ProductSchemeData prodSchemeData)
        {
            if (prodSchemeData.VariantNumber > 0)
            {
                var conn = DatabaseConnection.GetInstance().SqlConn;
                var results = await conn.QueryAsync<ProductSchemeVariantModel>(
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
        ///     Get the scheme data variant information from the scheme data variant code.
        /// </summary>
        /// <param name="prodSchemeData"><see cref="ActItemSchemeData" /> Product scheme data.</param>
        /// <returns><see cref="Task{SProductSchemeData}" /> ProductSchemeData.</returns>
        public async Task<ProductSchemeData> FillSchemeDataVarInfoFromSchemeDataVarCode(
            ProductSchemeData prodSchemeData)
        {
            if (string.IsNullOrEmpty(prodSchemeData.VariantCode))
            {
                var conn = DatabaseConnection.GetInstance().SqlConn;
                var results = await conn.QueryAsync<ProductSchemeVariantModel>(
                    "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantCode = ?",
                    prodSchemeData.VariantCode);

                if (results?.Count > 0)
                {
                    prodSchemeData.VariantNumber = results[0].ProductSchemeVariantNo;
                    prodSchemeData.VariantDescription = results[0].ProductSchemeVariantDescription;
                }
                else
                {
                    throw new Exception("FillSchemeDataVarInfoFromSchemeDataVarCode in" +
                                        " ActDetail returned no results");
                }
            }

            return prodSchemeData;
        }

        /// <summary>
        ///     On data access status out event.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher" /> Event publisher.</param>
        /// <param name="eventArgs"><see cref="DataAccessEventArgs" /> Data access event arguments.</param>
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
        ///     On data access progress out event.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher" /> Event publisher.</param>
        /// <param name="eventArgs"><see cref="DataAccessEventArgs" /> Data access event arguments.</param>
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

        /// <summary>
        ///     Register data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess" /> Event listener.</param>
        public void RegLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut += listener.OnLayerStatOut;
        }

        /// <summary>
        ///     Un-register data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess" /> Event listener.</param>
        public void UnregLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut -= listener.OnLayerStatOut;
        }

        /// <summary>
        ///     Register data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess" /> Event listener.</param>
        public void RegLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut += listener.OnLayerProgressOut;
        }

        /// <summary>
        ///     Unregister data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccess" /> Event listener.</param>
        public void UnregLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut -= listener.OnLayerProgressOut;
        }

        /// <summary>
        ///     The Insert.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument" />.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public abstract Task Insert(XDocument data);

        /// <summary>
        ///     Delete one or a set of activity detail.
        /// </summary>
        /// <returns><see cref="int" /> Previous non deleted detail foreign key reference number.</returns>
        /// <remarks>this is a test of the emergency broadcast system.</remarks>
        public abstract Task<int> Delete();

        /// <summary>
        ///     Refresh the activity detail.
        /// </summary>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public abstract Task Refresh();

        /// <summary>
        ///     Create submission XML.
        /// </summary>
        /// <param name="data"><see cref="XDocument" /> Filter criteria.</param>
        /// <returns>The <see cref="string" /> Submission XML.</returns>
        public abstract Task<string> CreateSubmitXml(XDocument data);

        /// <summary>
        ///     Delete submission XML.
        /// </summary>
        /// <param name="data"><see cref="XDocument" /> Filter criteria.</param>
        /// <returns>The <see cref="string" /> Deletion XML.</returns>
        public abstract Task<string> CreateDeleteXml(XDocument data);

        /// <summary>
        ///     Submit the request to the comm server.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object" /> Selected activity detail.</param>
        /// <param name="data"><see cref="XDocument" /> XML data to submit.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public abstract Task Submit(object selectedActivityDetail, XDocument data);

        /// <summary>
        ///     Perform a search in the detail list for the search text.
        /// </summary>
        /// <param name="searchText"><see cref="string" /> Text to search for a match.</param>
        /// <returns>Found activity detail or null.</returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        ///     Get a list of activity details.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool" /> Is this a batch submission.</param>
        /// <returns>List of activity detail objects.</returns>
        public abstract Task<List<object>> GetActivityDetailList(bool isSubmitBatch);

        /// <summary>
        ///     Design the activity detail grid.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid" /> Main activity grid.</param>
        /// <param name="listViewMain"><see cref="ListView" /> List view of activity details.</param>
        /// <returns>Constructed observable collection.</returns>
        public abstract ObservableCollection<object> DesignGrid(ref Grid gridMain, ref ListView listViewMain);

        /// <summary>
        ///     Create an activity data dump.
        /// </summary>
        /// <returns><see cref="XDocument"/> XML data dump.</returns>
        public abstract XDocument CreateDump();

        /// <summary>
        ///     Design activity grid template.
        /// </summary>
        /// <param name="gridMain"><see cref="Grid" /> Main activity grid.</param>
        /// <param name="actDetail"><see cref="ActivityDetail" /> Activity detail.</param>
        /// <param name="mainActivityItems"><see cref="ObservableCollection{T}" /> Activity items list.</param>
        /// <returns><see cref="Grid" /> Created grid.</returns>
        public abstract DataTemplate DesignGrid(
            ref Grid gridMain,
            ActivityDetail actDetail,
            ObservableCollection<object> mainActivityItems);

        /// <summary>
        ///     Set the activity detail selected activity item and selected activity detail index.
        /// </summary>
        /// <remarks>
        ///     Iterate through the ListDetails looking for a matched activity detail reference number.
        ///     If a match is found set the SelectedDetail to one found in the ListDetails
        ///     else the SelectedDetail is set to null and the selected list detail index to -1.
        /// </remarks>
        /// <param name="actDetailFoundRefno"><see cref="int" /> Activity detail reference number to look up.</param>
        public virtual void SetSelectedActivityDetailFromList(int actDetailFoundRefno)
        {
            this.SelectedListIdx = -1;
            if (actDetailFoundRefno >= 0)
            {
                var idx = 0;
                foreach (var detail in this.ListDetails.Cast<ActivityDetail>())
                {
                    if (detail.ActivityDetailRefno == this.DetailRefno)
                    {
                        this.SelectedDetail = detail;
                        this.SelectedListIdx = idx;
                        break;
                    }

                    idx++;
                }
            }
            else
            {
                this.SelectedDetail = null;
            }
        }

        /// <summary>
        ///     Get the activity detail reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object" /> Selected activity detail.</param>
        /// <returns><see cref="int" /> Activity detail reference number.</returns>
        public abstract int GetRefno(object selectedActivityDetail);

        /// <summary>
        ///     Get the activity detail foreign key reference number.
        /// </summary>
        /// <param name="selectedActivityDetail"><see cref="object" /> Selected activity detail.</param>
        /// <returns><see cref="int" /> Activity detail foreign key reference number.</returns>
        public abstract int GetFkRefno(object selectedActivityDetail);

        /// <summary>
        ///     Insert a skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo" /> Skid information to insert.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public abstract Task InsertSkid(SysSkidInfo skidInfo);

        /// <summary>
        ///     Delete a skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SysSkidInfo" /> Skid information to delete.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        public abstract Task DeleteSkid(SysSkidInfo skidInfo);

        /// <summary>
        ///     Get the activity detail description.
        /// </summary>
        /// <returns>The <see cref="string" />.</returns>
        public abstract string GetControlDescription();

        /// <summary>
        ///     Total all skid quantity from the list details for the selected skid.
        /// </summary>
        /// <param name="skidInfo"><see cref="SSkidInfo" /> Skid information.</param>
        /// <returns><see cref="SFillQuantity" /> Summed quantities.</returns>
        public virtual SFillQuantity GetTotalSkidQuantityFromListDetail(SSkidInfo skidInfo)
        {
            var fillQty = new SFillQuantity
            {
                Quantity = 0,
                BaseQuantity = 0,
                WeightQuantity = 0
            };

            if (skidInfo.SkidFkRefno > 0 && this.ListDetails?.Count > 0)
            {
                foreach (var detail in this.ListDetails.Cast<ActivityDetail>())
                {
                    // Only sum the quantities for the selected skid.
                    if (detail.ActivityDetailLogisticUnitSkidNumber == skidInfo.SkidCode)
                    {
                        fillQty.Quantity += Convert.ToDecimal(detail.ActivityDetailQuantity);
                        fillQty.BaseQuantity += Convert.ToDecimal(detail.ActivityDetailBaseQuantity);
                        fillQty.WeightQuantity += Convert.ToDecimal(detail.ActivityDetailWeightQuantity);
                    }
                }
            }

            return fillQty;
        }

        /// <summary>
        ///     Total all activity detail quantity from the list details.
        /// </summary>
        /// <returns><see cref="SFillQuantity" /> Summed quantities.</returns>
        public virtual async Task<SFillQuantity> GetTotalQuantityFromListDetail()
        {
            var fillQty = new SFillQuantity
            {
                Quantity = 0,
                BaseQuantity = 0,
                WeightQuantity = 0
            };
            await Device.InvokeOnMainThreadAsync(() =>
            {
                if (this.ListDetails?.Count > 0)
                {
                    foreach (var detail in this.ListDetails.Cast<ActivityDetail>())
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
        ///     Clear the activity properties.
        /// </summary>
        public virtual void ClearProperties()
        {
            // Document activity references
            this.DetailRefno = 0;
            this.DetailFkRefno = 0;

            // Quantity and weights
            this.Quantity = 0;
            this.BaseQuantity = 0;
            this.WeightQuantity = 0;

            // Scheme

            // Replace empty scheme dates with date time now, not empty.
            var tmpSchemeData = this.SchemeData;
            tmpSchemeData.LogisticUnitBestBeforeSellByDate = DateTime.Now;
            tmpSchemeData.LogisticUnitProductionDate = DateTime.Now;
            this.SchemeData = new ProductSchemeData();
            this.SchemeData = tmpSchemeData;
            this.SchemeId = string.Empty;
            this.UniqueSchemeId = string.Empty;

            // Extra HACCP etc.
            this.ActExtra.Text1 = string.Empty;
            this.ActExtra.Text2 = string.Empty;
            this.ActExtra.Text3 = string.Empty;
            this.ActExtra.Text4 = string.Empty;
            this.ActExtra.DateTime1 = SysCommon.EmptyDateTime;

            ////this.SelectedLocMap = new SLocMap();
            ////this.SelectedDetail = null;
        }

        /// <summary>
        ///     Select an activity detail.
        /// </summary>
        /// <param name="actDetail"><see cref="object" /> Activity detail to select.</param>
        /// <returns>The <see cref="Task" /> Task operation.</returns>
        /// <remarks>
        ///     Assigns this.SelectedDetail.
        ///     Handles verifying product default location.
        ///     Current location is set to the selected skid location.
        ///     Clears the selected skid if the line item product changed (Flag).
        /// </remarks>
        public virtual async Task AssignActivityDetail(object actDetail)
        {
            if (actDetail is ActivityDetail detail)
            {
                this.DetailRefno = detail.ActivityDetailRefno;
                this.DetailFkRefno = detail.ActivityDetailFKRefno;
                this.Quantity = detail.ActivityDetailQuantity;
                this.BaseQuantity = detail.ActivityDetailBaseQuantity;
                this.WeightQuantity = detail.ActivityDetailWeightQuantity;

                // SelectedLocMap is used in the activity detail edit for label printing.
                // detail.LocationMapRefno could be a warehouse or even a slot.
                this.SelectedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(detail.LocationMapRefno);

                var schemeData = new ProductSchemeData
                {
                    LogisticUnitBatchLotNumber = detail.ActivityDetailLogisticUnitBatchLotNumber,
                    LogisticUnitBestBeforeSellByDate = detail.ActivityDetailLogisticUnitBestBeforeSellByDate,
                    LogisticUnitProductionDate = detail.ActivityDetailLogisticUnitProductionDate,
                    LogisticUnitSkidNumber = detail.ActivityDetailLogisticUnitSkidNumber,
                    SingleTradeItemSerialNumber = detail.ActivityDetailSingleTradeItemSerialNumber,
                    VariantCode = detail.ProductSchemeVariantCode,
                    VariantDescription = detail.ProductSchemeVariantDescription,
                    VariantNumber = detail.ProductSchemeVariantNo
                };
                this.SchemeData = schemeData;

                AssignExtra();
                AssignScheme();
            }
            else
            {
                throw new Exception("Expected actDetail parameter to be of type ActivityDetail.");
            }

            return;

            //////////////////////////////////////////////////////////////////////////
            void AssignExtra()
            {
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

                this.ActExtra.DateTime1 = detail.ActivityDetailExtra5Date <= SysCommon.EmptyDateTime
                    ? SysCommon.EmptyDateTime
                    : detail.ActivityDetailExtra5Date;
            }

            //////////////////////////////////////////////////////////////////////////
            void AssignScheme()
            {
                var tmpSchemeData = this.SchemeData;

                // Default best before date to today.
                var bestBefore = detail.ActivityDetailLogisticUnitBestBeforeSellByDate;
                tmpSchemeData.LogisticUnitBestBeforeSellByDate = SysCommon.IsEmptyDateTime(bestBefore) &&
                                                                 this.ActItem.ProdInfo.Scheme
                                                                     .IsBestBeforeDateTrackedDefault
                    ? DateTime.Now
                    : bestBefore;

                // Default production date to today.
                var production = detail.ActivityDetailLogisticUnitProductionDate;
                tmpSchemeData.LogisticUnitProductionDate = SysCommon.IsEmptyDateTime(production) &&
                                                           this.ActItem.ProdInfo.Scheme.IsProductionDateTrackedDefault
                    ? DateTime.Now
                    : production;

                tmpSchemeData.SingleTradeItemSerialNumber = detail.ActivityDetailSingleTradeItemSerialNumber;
                tmpSchemeData.LogisticUnitBatchLotNumber = detail.ActivityDetailLogisticUnitBatchLotNumber;

                // Check for DBNull because variants are in a child table, left join from the data query.
                if (detail.ProductSchemeVariantCode != null)
                {
                    tmpSchemeData.VariantCode = detail.ProductSchemeVariantCode;
                    tmpSchemeData.VariantNumber = detail.ProductSchemeVariantNo;
                }

                this.SchemeData = tmpSchemeData;

                this.SchemeId = detail.ActivityDetailSchemeID;
                this.UniqueSchemeId = detail.ActivityDetailUniqueSchemeID;
            }
        }

        /// <summary>
        ///     Check if the activity detail can be applied.
        /// </summary>
        /// <param name="data"><see cref="XDocument" /> Activity detail.</param>
        /// <returns><see cref="bool" /> True if the detail is ready to be applied else false.</returns>
        public virtual async Task IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                if (String.CompareOrdinal(this.UniqueSchemeId, string.Empty) > 0)
                {
                    var isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeId(this.UniqueSchemeId);

                    if (isDupSchemeId)
                    {
                        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeId);
                    }
                }
            }
        }

        /// <summary>
        ///     Create a submit product label XML request.
        /// </summary>
        /// <param name="detailData"><see cref="XDocument" /> Data to use to populate label.</param>
        /// <returns><see cref="string" /> Submission string.</returns>
        public virtual async Task<string> CreateSubmitProductLabelXml(XDocument detailData)
        {
            var xmlString = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Async = true,
                Indent = false,
            };
            var xml = XmlWriter.Create(xmlString, settings);

            // Init the production and best before date as empty intentional.
            // We want to send a 'blank' date if it is not tracked rather than {01/01/1900} as the empty date.
            var bestBefore = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked)
            {
                bestBefore = DateTime.Parse(detailData.Root.Element("bdate").Value).ToString("MM/dd/yyyy");
            }

            var production = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.LabelInfo.IsProductionDateTracked)
            {
                production = DateTime.Parse(detailData.Root.Element("pdate").Value).ToString("MM/dd/yyyy");
            }

            var prnSettings = new PrinterSettings().LoadData();

            decimal altQty = Convert.ToInt32(detailData.Root.Element("alt_qty").Value);
            var wtQty = Convert.ToDecimal(detailData.Root.Element("wt_qty").Value);

            var request = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "<action>SUBMIT PRINT PRODUCT LABEL</action>"
                + "<whs_id>0</whs_id>"
                + "<sect_refno>0</sect_refno>"
                + "</command>"
                + await SysCommon.GetRequestDefaults(false)
                + "<prod_id>" + this.ActItem.ProdInfo.Code + "</prod_id>"
                + "<is_bc>" + this.ActItem.ProdInfo.UnitIsBc + "</is_bc>"
                + "<whs_id>" + this.LocMap.CurrLocMap.WhsFkRefno + "</whs_id>"
                + "<sect_refno>" + this.LocMap.CurrLocMap.SectFkRefno + "</sect_refno>"
                + "<slot_refno>" + this.LocMap.CurrLocMap.SlotFkRefno + "</slot_refno>"
                + "<refno>" + this.ActItem.ActHdr.HdrFkRefno + "</refno>"
                + "<itm_refno>" + this.ActItem.ItemFkRefno + "</itm_refno>"
                + "<alt_qty>" + altQty + "</alt_qty>"
                + "<alt_unit>" + this.ActItem.ProdInfo.UnitCode + "</alt_unit>"
                + "<base_qty>" + detailData.Root.Element("base_qty").Value + "</base_qty>"
                + "<base_unit>" + this.ActItem.ProdInfo.BaseUnitCode + "</base_unit>"
                + "<wt_qty>" + wtQty + "</wt_qty>"
                + "<wt_unit>" + this.ActItem.ProdInfo.WeightUnitCode + "</wt_unit>"
                + "<serial>" + detailData.Root.Element("serial").Value + "</serial>"
                + "<lot_no>" + detailData.Root.Element("lot_no").Value + "</lot_no>"
                + "<pdate>" + production + "</pdate>"
                + "<bdate>" + bestBefore + "</bdate>"
                + "<variant_no>" + detailData.Root.Element("variant_no").Value + "</variant_no>"
                + "<variant_code>" + detailData.Root.Element("variant_code").Value + "</variant_code>"
                + "<mobile_printer_name>"
                + (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty)
                + "</mobile_printer_name>"
                + "</imclient>");

            request.Save(xml);
            await xml.FlushAsync();
            xml.Close();

            return xmlString.ToString();
        }

        /// <summary>
        ///     Get activity detail XML data.
        /// </summary>
        /// <param name="actDetail"><see cref="ActDetail" /> Activity detail.</param>
        /// <returns><see cref="XDocument" /> Activity detail XML.</returns>
        public XDocument GetXml(ActDetail actDetail)
        {
            var xml = $@"<?xml version=""1.0"" encoding=""utf-16"" standalone=""yes""?>
                <params>
                <refno></refno>
                <ser_refno></ser_refno>
                <serial>{actDetail.SchemeData.SingleTradeItemSerialNumber}</serial>
                <pdate>{actDetail.SchemeData.LogisticUnitProductionDate}</pdate>
                <lot_no>{actDetail.SchemeData.LogisticUnitBatchLotNumber}</lot_no>
                <bdate>{actDetail.SchemeData.LogisticUnitBestBeforeSellByDate}</bdate>
                <variant_code>{actDetail.SchemeData.VariantCode}</variant_code>
                <variant_no>{actDetail.SchemeData.VariantNumber}</variant_no>
                <alt_qty>{actDetail.Quantity}</alt_qty>
                <wt_qty>{actDetail.WeightQuantity}</wt_qty>
                <base_qty>{actDetail.BaseQuantity}</base_qty>
                <skid_code>{this.ActItem.ActHdr.SkidInfo.CurrSkid.SkidCode}</skid_code>
                <is_batch_print_product_label>{this.SysEnv.SysSettings.IsPrintProductLabelsAsBatch}</is_batch_print_product_label>
                <extra1>{actDetail.ActExtra.Text1}</extra1>
                <extra2>{actDetail.ActExtra.Text2}</extra2>
                <extra3>{actDetail.ActExtra.Text3}</extra3>
                <extra4>{actDetail.ActExtra.Text4}</extra4>
                <extra5>{actDetail.ActExtra.DateTime1}</extra5>
                </params>";
            return XDocument.Parse(xml);
        }

        /// <summary>
        ///     Check if the inventory tracking scheme is complete.
        /// </summary>
        /// <param name="actItem"><see cref="ActItem" /> Activity item.</param>
        /// <returns>.</returns>
        public bool IsSchemeComplete(ActItem actItem)
        {
            var isComplete = !(actItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                                SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate));

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
        ///     Check if the product label scheme is complete.
        /// </summary>
        /// <returns><see cref="bool" /> True if the label scheme is complete else false.</returns>
        public bool IsLabelSchemeComplete()
        {
            return this.IsLabelSchemeComplete(this.SchemeData);
        }

        /// <summary>
        ///     Check if the product label scheme is complete.
        /// </summary>
        /// <param name="schemeData"><see cref="ActItemSchemeData" /> Product scheme data.</param>
        /// <returns><see cref="bool" /> True if the label scheme is complete else false.</returns>
        public bool IsLabelSchemeComplete(ProductSchemeData schemeData)
        {
            var isComplete = !(this.ActItem.ProdInfo.Scheme.LabelInfo.IsBestBeforeDateTracked &&
                               SysCommon.IsEmptyDateTime(schemeData.LogisticUnitBestBeforeSellByDate));

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
        ///     Fill the activity detail object from XML data.
        /// </summary>
        /// <param name="data"><see cref="XDocument" /> Activity detail data.</param>
        /// <returns><see cref="Task" /> Task operation.</returns>
        public async Task FillActDetailObjFromXml(XDocument data)
        {
            data = data ?? throw new NullReferenceException("data");
            if (data is { Root: null })
            {
                throw new NullReferenceException("data.Root");
            }

            if (!string.IsNullOrEmpty(data.Root.Element("ser_refno")?.Value))
            {
                var fkRefno = data.Root.Element("ser_refno")?.Value;
                if (fkRefno != null)
                {
                    this.DetailFkRefno = int.Parse(fkRefno);
                }
            }

            if (!string.IsNullOrEmpty(data.Root.Element("refno") !.Value))
            {
                this.DetailRefno = int.Parse(data.Root.Element("refno") !.Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("alt_qty") !.Value))
            {
                this.Quantity = decimal.Parse(data.Root.Element("alt_qty") !.Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("base_qty") !.Value))
            {
                this.BaseQuantity = decimal.Parse(data.Root.Element("base_qty") !.Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("wt_qty") !.Value))
            {
                this.WeightQuantity = decimal.Parse(data.Root.Element("wt_qty") !.Value);
            }

            var tmpSchemeData = this.SchemeData;

            if (!string.IsNullOrEmpty(data.Root.Element("bdate") !.Value))
            {
                tmpSchemeData.LogisticUnitBestBeforeSellByDate =
                    Convert.ToDateTime(data.Root.Element("bdate") !.Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("pdate") !.Value))
            {
                tmpSchemeData.LogisticUnitProductionDate =
                    Convert.ToDateTime(data.Root.Element("pdate") !.Value);
            }

            if (!string.IsNullOrEmpty(data.Root.Element("serial") !.Value))
            {
                tmpSchemeData.SingleTradeItemSerialNumber = data.Root.Element("serial") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("lot_no") !.Value))
            {
                tmpSchemeData.LogisticUnitBatchLotNumber = data.Root.Element("lot_no") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_code") !.Value))
            {
                tmpSchemeData.VariantCode = data.Root.Element("variant_code") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("variant_no") !.Value))
            {
                tmpSchemeData.VariantNumber = int.Parse(data.Root.Element("variant_no") !.Value);
            }

            tmpSchemeData = FillSchemeDataVarInfoFromSchemeDataVarNo(tmpSchemeData).GetAwaiter().GetResult();
            this.SchemeData = tmpSchemeData;

            this.SchemeId = ProductInfoTasks.GetActSchemeIdFromSProdSchemeData(
                this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme,
                this.SchemeData);
            this.UniqueSchemeId = ProductInfoTasks.GetUniqueSchemeIdFromSProdSchemeData(
                this.ActItem.ProdInfo.Code,
                this.ActItem.ProdInfo.Scheme,
                this.SchemeData);

            if (!string.IsNullOrEmpty(data.Root.Element("extra1") !.Value))
            {
                this.ActExtra.Text1 = data.Root.Element("extra1") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra2") !.Value))
            {
                this.ActExtra.Text2 = data.Root.Element("extra2") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra3") !.Value))
            {
                this.ActExtra.Text3 = data.Root.Element("extra3") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra4") !.Value))
            {
                this.ActExtra.Text4 = data.Root.Element("extra4") !.Value;
            }

            if (!string.IsNullOrEmpty(data.Root.Element("extra5") !.Value))
            {
                this.ActExtra.DateTime1 = Convert.ToDateTime(data.Root.Element("extra5") !.Value);
            }
        }

        /// <summary>
        ///     Find the current detail in the list detail.
        /// </summary>
        /// <param name="activityDetailFkRefno"><see cref="int" /> Activity detail FK reference number.</param>
        /// <returns><see cref="int" /> Found list detail refno or negative one if not found.</returns>
        public virtual int FindListDetailRefno(int activityDetailFkRefno)
        {
            var detailRefno = -1;

            // Go to the list item to update the detail quantity.
            for (var idx = 0; idx < this.ListDetails.Count; idx++)
            {
                var detail = (ActivityDetail)this.ListDetails[idx];
                if (detail.ActivityDetailFKRefno == activityDetailFkRefno)
                {
                    detailRefno = detail.ActivityDetailRefno;
                    break;
                }
            }

            return detailRefno;
        }

        /// <summary>
        ///     Add an activity detail into the list details.
        /// </summary>
        /// <returns><see cref="ActivityDetail" /> Activity detail.</returns>
        public abstract ActivityDetail GatherDetail();

        /// <summary>
        ///     Update detail in the detail list.
        /// </summary>
        /// <param name="listDetailIdx"><see cref="int" /> Zero based list detail index.</param>
        public abstract void UpdateListDetail(int listDetailIdx);

        /// <summary>
        ///     Get a string formatted xml of default detail values.
        /// </summary>
        /// <returns><see cref="string" /> Activity detail defaults XML string.</returns>
        protected virtual string GetRequestDetailDefaults()
        {
            // Init the production and best before date as empty intentional.
            // We want to send a 'blank' date if it is not tracked rather than {01/01/1900} as the empty date.
            var bestBeforeDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsBestBeforeDateTracked &&
                !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitBestBeforeSellByDate))
            {
                bestBeforeDate = this.SchemeData.LogisticUnitBestBeforeSellByDate.ToString("MM/dd/yyyy");
            }

            var productionDate = string.Empty;

            if (this.ActItem.ProdInfo.Scheme.IsProductionDateTracked &&
                !SysCommon.IsEmptyDateTime(this.SchemeData.LogisticUnitProductionDate))
            {
                productionDate = this.SchemeData.LogisticUnitProductionDate.ToString("MM/dd/yyyy");
            }

            var itemFkRefno = this.ActItem.ActStatus == SysCommon.ActStatus.Inserted ? 0 : this.ActItem.ItemFkRefno;
            var def = $@"
                <prod_id>{this.ActItem.ProdInfo.Code}</prod_id>
                <is_bc>{this.ActItem.ProdInfo.UnitIsBc}</is_bc>
                <whs_id>{this.LocMap.CurrLocMap.WhsFkRefno}</whs_id>
                <sect_refno>{this.LocMap.CurrLocMap.SectFkRefno}</sect_refno>
                <slot_refno>{this.LocMap.CurrLocMap.SlotFkRefno}</slot_refno>
                <refno>{this.ActItem.ActHdr.HdrFkRefno}</refno>
                <itm_refno>{itemFkRefno}</itm_refno>
                <ser_refno>{this.DetailFkRefno}</ser_refno>
                <alt_qty>{this.Quantity}</alt_qty>
                <alt_unit>{this.ActItem.ProdInfo.UnitCode}</alt_unit>
                <base_qty>{this.BaseQuantity}</base_qty>
                <base_unit>{this.ActItem.ProdInfo.BaseUnitCode}</base_unit>
                <wt_qty>{this.WeightQuantity}</wt_qty>
                <wt_unit>{this.ActItem.ProdInfo.WeightUnitCode}</wt_unit>
                <serial>{this.SchemeData.SingleTradeItemSerialNumber}</serial>
                <lot_no>{this.SchemeData.LogisticUnitBatchLotNumber}</lot_no>
                <pdate>{productionDate}</pdate>
                <bdate>{bestBeforeDate}</bdate>
                <variant_no>{this.SchemeData.VariantNumber}</variant_no>
                <extra1>{this.ActExtra.Text1}</extra1>
                <extra2>{this.ActExtra.Text2}</extra2>
                <extra3>{this.ActExtra.Text3}</extra3>
                <extra4>{this.ActExtra.Text4}</extra4>
                <extra5>{this.ActExtra.DateTime1}</extra5>
                <is_disconnected>{this.SysEnv.SysSettings.IsDisconnectedMode}</is_disconnected>";

            return def;
        }

        /// <summary>
        ///     Assign activity detail product information from the activity item.
        /// </summary>
        /// <param name="detail"><see cref="ActivityDetail" /> detail object.</param>
        protected virtual void GatherProductDetail(ActivityDetail detail)
        {
            detail.ProductRefno = this.ActItem.ProdInfo.Refno;
            detail.ProductUnitRefno = this.ActItem.ProdInfo.UnitRefno;
            detail.ProductCode = this.ActItem.ProdInfo.Code;
            detail.ProductUnitBC = this.ActItem.ProdInfo.UnitBc;
            detail.ProductUnitIsBC = this.ActItem.ProdInfo.UnitIsBc;
            detail.ProductDescription = this.ActItem.ProdInfo.Desc;
            detail.ProductUnitSizeDescription = this.ActItem.ProdInfo.UnitSizeDesc;

            detail.UnitCode = this.ActItem.ProdInfo.UnitCode;
            detail.BaseUnitCode = this.ActItem.ProdInfo.BaseUnitCode;
            detail.WtUnitCode = this.ActItem.ProdInfo.WeightUnitCode;
        }

        /// <summary>
        ///     Assign activity detail location information from the current location.
        /// </summary>
        /// <param name="detail"><see cref="ActivityDetail" /> detail object.</param>
        protected virtual void GatherLocationDetail(ActivityDetail detail)
        {
            detail.LocationMapRefno = this.LocMap.CurrLocMapRefno;

            string locName;
            int locCode;
            if (this.LocMap.CurrLocMap.SlotLocMapRefno > 0)
            {
                locCode = this.LocMap.CurrLocMap.SlotCode;
                locName = this.LocMap.CurrLocMap.SlotName;
            }
            else if (this.LocMap.CurrLocMap.SectLocMapRefno > 0)
            {
                locCode = this.LocMap.CurrLocMap.SectCode;
                locName = this.LocMap.CurrLocMap.SectName;
            }
            else
            {
                locCode = this.LocMap.CurrLocMap.WhsCode;
                locName = this.LocMap.CurrLocMap.WhsName;
            }

            detail.LocationCode = locCode;
            detail.LocationName = locName;
        }

        /// <summary>
        ///     Assign activity detail scheme information from the scheme data.
        /// </summary>
        /// <param name="detail"><see cref="ActivityDetail" /> detail object.</param>
        protected virtual void GatherSchemeDetail(ActivityDetail detail)
        {
            detail.ProductSchemeVariantCode = this.SchemeData.VariantCode;
            detail.ProductSchemeVariantNo = this.SchemeData.VariantNumber;
            detail.ActivityDetailSingleTradeItemSerialNumber = this.SchemeData.SingleTradeItemSerialNumber;
            detail.ActivityDetailLogisticUnitBestBeforeSellByDate = this.SchemeData.LogisticUnitBestBeforeSellByDate;
            detail.ActivityDetailLogisticUnitBatchLotNumber = this.SchemeData.LogisticUnitBatchLotNumber;
            detail.ActivityDetailLogisticUnitProductionDate = this.SchemeData.LogisticUnitProductionDate;
            detail.ActivityDetailLogisticUnitSkidNumber = this.SchemeData.LogisticUnitSkidNumber;
        }

        /// <summary>
        ///     Assign activity detail extra information from the activity extra data.
        /// </summary>
        /// <param name="detail"><see cref="ActivityDetail" /> detail object.</param>
        protected virtual void GatherExtraDetail(ActivityDetail detail)
        {
            detail.ActivityDetailExtra1Text = this.ActExtra.Text1;
            detail.ActivityDetailExtra2Text = this.ActExtra.Text2;
            detail.ActivityDetailExtra3Text = this.ActExtra.Text3;
            detail.ActivityDetailExtra4Text = this.ActExtra.Text4;
            detail.ActivityDetailExtra5Date = this.ActExtra.DateTime1;
        }

        /// <summary>
        ///     Assign activity detail quantity information from the activity quantity data.
        /// </summary>
        /// <param name="detail"><see cref="ActivityDetail" /> detail object.</param>
        protected virtual void GatherQuantityDetail(ActivityDetail detail)
        {
            detail.ActivityDetailQuantity = Math.Round(this.Quantity, 3);
            detail.ActivityDetailBaseQuantity = Math.Round(this.BaseQuantity, 3);
            detail.ActivityDetailWeightQuantity = Math.Round(this.WeightQuantity, 3);
        }

        /// <summary>
        ///     Insert activity detail into activity detail tables.
        /// </summary>
        /// <returns><see cref="int" /> Inserted activity detail reference number.</returns>
        protected async Task<int> InsertDetailIntoTablesDetail()
        {
            var conn = DatabaseConnection.GetInstance().SqlConn;
            int actDetailRefno;
            try
            {
                ////if (string.CompareOrdinal(this.UniqueSchemeId, string.Empty) > 0)
                ////{
                ////    var isDupSchemeId = await ProductInfoTasks.IsDuplicateUniqueSchemeId(this.UniqueSchemeId);
                ////    if (isDupSchemeId)
                ////    {
                ////        throw new Exception("Duplicate scheme ID!" + Environment.NewLine + this.UniqueSchemeId);
                ////    }
                ////}

                const string query = @"
                    INSERT INTO tActivityDetail (
                        ActivityDetailSubType, 
                        ActivityDetailStatus, 
                        ActivityItemRefno, 
                        LocationMapRefno, 
                        ProductUnitRefno, 
                        ActivityDetailUniqueSchemeID, 
                        ActivityDetailSchemeID, 
                        ActivityDetailQuantity, 
                        ActivityDetailBaseQuantity, 
                        ActivityDetailWeightQuantity, 
                        ActivityDetailSingleTradeItemSerialNumber, 
                        ActivityDetailLogisticUnitBatchLotNumber, 
                        ActivityDetailLogisticUnitBestBeforeSellByDate, 
                        ActivityDetailLogisticUnitExpiryUseByDate, 
                        ActivityDetailLogisticUnitProductionDate, 
                        ActivityDetailLogisticUnitDueDate, 
                        ActivityDetailLogisticUnitPackagingDate, 
                        ActivityDetailLogisticUnitSkidNumber
                    ) VALUES (
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?, 
                        ?
                    )";

                var parameters = new object[]
                {
                    Convert.ToInt32(this.ActSubType),
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(this.ActItem.ItemRefno),
                    Convert.ToInt32(this.LocMap.CurrLocMapRefno),
                    Convert.ToInt32(this.ActItem.ProdInfo.UnitRefno),
                    Convert.ToString(this.UniqueSchemeId),
                    Convert.ToString(this.SchemeId),
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
                    Convert.ToString(this.SchemeData.LogisticUnitSkidNumber)
                };

                _ = await conn.ExecuteAsync(query, parameters);

                actDetailRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(
                    "SELECT last_insert_rowid() FROM tActivityDetail"));
                this.DetailRefno = actDetailRefno;

                if (this.SchemeData.VariantNumber > 0)
                {
                    var results = await conn.QueryAsync<ProductSchemeVariantModel>(
                        "SELECT * FROM tProductSchemeVariant WHERE ProductSchemeVariantNo = ?",
                        Convert.ToInt32(this.SchemeData.VariantNumber));

                    if (results != null && results.Count > 0)
                    {
                        var schemeVarRefno = Convert.ToInt32(results[0].ProductSchemeVariantRefno);
                        _ = await conn.ExecuteAsync(
                            @"INSERT INTO tActivityDetailProductSchemeVariantXRef
                                (ActivityDetailRefno, ProductSchemeVariantRefno)
                                VALUES (?, ?)",
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
                    var queryExtra = @"
                        INSERT INTO tActivityDetailExtra (
                            ActivityDetailRefno, 
                            ActivityDetailSubType, 
                            ActivityDetailExtra1Text, 
                            ActivityDetailExtra2Text, 
                            ActivityDetailExtra3Text, 
                            ActivityDetailExtra4Text, 
                            ActivityDetailExtra5Date
                        ) VALUES (
                            ?, 
                            ?, 
                            ?, 
                            ?, 
                            ?, 
                            ?, 
                            ?
                        )";

                    var parametersExtra = new object[]
                    {
                        Convert.ToInt32(actDetailRefno),
                        Convert.ToInt32(this.ActSubType),
                        Convert.ToString(this.ActExtra.Text1),
                        Convert.ToString(this.ActExtra.Text2),
                        Convert.ToString(this.ActExtra.Text3),
                        Convert.ToString(this.ActExtra.Text4),
                        Convert.ToDateTime(this.ActExtra.DateTime1)
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
        ///     Data layer status out handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender event arguments.</param>
        /// <param name="outMessage"><see cref="string" /> Status out message.</param>
        /// <param name="overrideDisconnectedMode"><see cref="bool" /> Do not raise event if in disconnected mode.</param>
        protected void DataLayerStatusOutHandler(
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
        ///     Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int" /> Current progress value.</param>
        /// <param name="minValue"><see cref="int" /> Minimum progress value.</param>
        /// <param name="maxValue"><see cref="int" /> Maximum progress value.</param>
        protected void DataLayerProgressOutHandler(
            object sender,
            EventArgs e,
            int currentValue,
            int minValue,
            int maxValue)
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
        ///     Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object" /> Sender object.</param>
        /// <param name="e"><see cref="EventArgs" /> Sender event arguments.</param>
        /// <param name="currentValue"><see cref="int" /> Current progress value.</param>
        protected void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }
    }
}