// <copyright file="clsActivityItem.cs" company="InterSect Business Systems Inc">
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
    using dMobile.GroundWork;
    using SQLite;
    using Xamarin.Forms;

    /// <summary>
    /// Activity item base class.
    /// </summary>
    public abstract class ActItem : IDataLayerEventPublisher, IDataAccessEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ActItem(Activity activity)
        {
            this.SysEnv = SysEnvironment.GetInstance();
            this.LocMap = LocationMap.GetInstance();
            this.ActHdr = activity;
            this.ActSubType = this.ActHdr.ActSubType;
        }

        /// <summary>
        /// The data layer status out event handler.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="args">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public delegate void OnDataLayerStatusOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs args);

        /// <summary>
        /// The data layer progress out event handler.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="args">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public delegate void OnDataLayerProgressOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs args);

        /// <summary>
        /// Data layer status out event handler.
        /// </summary>
        public event OnDataLayerStatusOutEventHandler OnDataLayerStatusOut;

        /// <summary>
        /// Data layer progress out event handler.
        /// </summary>
        public event OnDataLayerProgressOutEventHandler OnDataLayerProgressOut;

        /// <summary>
        /// Gets or sets a value indicating whether to ask to create a new item..
        /// </summary>
        public bool IsAskCreateNewItem { get; set; } = true;

        /// <summary>
        /// Gets or sets the data event arguments..
        /// </summary>
        public DataLayerEventArgs DataEventArgs { get; set; } = new DataLayerEventArgs();

        /// <summary>
        /// Gets or sets the full class name..
        /// </summary>
        public string FullClassName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add an item..
        /// </summary>
        public bool IsAllowAddItem { get; set; } = false;

        /// <summary>
        /// Gets or sets the current selected line item location map..
        /// </summary>
        public LocationMap LocMap { get; set; } = LocationMap.GetInstance();

        /// <summary>
        /// Gets or sets the Product information..
        /// </summary>
        public ProductInfo ProdInfo { get; set; }

        /// <summary>
        /// Gets or sets the system environment..
        /// </summary>
        public SysEnvironment SysEnv { get; set; } = SysEnvironment.GetInstance();

        /// <summary>
        /// Gets or sets the list items..
        /// </summary>
        public List<object> ListItems { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets the activity item sub type..
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; } = SysCommon.ActSubTypes.NotApplicable;

        /// <summary>
        /// Gets or sets the activity item reference number..
        /// </summary>
        public int Refno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item foreign key reference number..
        /// </summary>
        public int FKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity header..
        /// </summary>
        public Activity ActHdr { get; set; }

        /// <summary>
        /// Gets or sets the selected activity item..
        /// </summary>
        public object SelectedActivityItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether allowed to add skid detail.
        /// </summary>
        /// <returns><see cref="bool"/> True if allowed to add skid detail.</returns>
        public abstract bool IsAllowAddSkidDetail();

        /// <summary>
        /// Insert an activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Data to insert.</param>
        /// <param name="askToInsertAsNewItem"><see cref="bool"/> Ask a question to insert a new item.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task Insert(XDocument data, bool askToInsertAsNewItem);

        /// <summary>
        /// Refresh the activity item data.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Filter criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Refresh(XDocument data);

        /// <summary>
        /// Create XML data for activity item submission.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Filter criteria.</param>
        /// <returns><see cref="string"/> Created XML submission data.</returns>
        public abstract Task<string> CreateSubmitXML(XDocument data);

        /// <summary>
        /// Search for activity items using the search text.
        /// </summary>
        /// <param name="searchText"><see cref="string"/> Search text.</param>
        /// <returns><see cref="object"/> Found object.</returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// Get a list of activity items.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is batch submission.</param>
        /// <returns>.</returns>
        public abstract Task<List<object>> GetActivityItemList(bool isSubmitBatch);

        /// <summary>
        /// Design the list view grid.
        /// </summary>
        /// <param name="GridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="ListViewMainActivity"><see cref="ListView"/> Main activity list view.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public abstract ObservableCollection<object> DesignGrid(
            ref Grid GridMainActivity,
            ref ListView ListViewMainActivity);

        /// <summary>
        /// Get the activity item reference number from the product unit reference number.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item refno else 0.</returns>
        public abstract Task<int> GetActivityItemRefno(int prodUnitRefno);

        /// <summary>
        /// Get the data view sort foreign key reference number index name.
        /// </summary>
        /// <returns><see cref="string"/> Foreign key index name.</returns>
        public abstract string GetDataViewSortFKRefnoIdx();

        /// <summary>
        /// Get the activity item control description.
        /// </summary>
        /// <returns><see cref="string"/> Activity item control description.</returns>
        public abstract string GetControlDescription();

        /// <summary>
        /// Get the unable to find item message.
        /// </summary>
        /// <param name="actItemStringFKRefno"><see cref="string"/> Activity item string foreign key reference number.</param>
        /// <returns><see cref="string"/> Unable to find item message.</returns>
        public abstract string GetUnableToFindItemFKRefnoMessage(string actItemStringFKRefno);

        /// <summary>
        /// Get the unable to parse item message.
        /// </summary>
        /// <param name="actItemStringFKRefno"><see cref="string"/> Activity item string foreign key reference number.</param>
        /// <returns><see cref="string"/> Unable to find item message.</returns>
        public abstract string GetUnableToParseItemFKRefnoMessage(string actItemStringFKRefno);

        /// <summary>
        /// Update the item data view quantity. Adds to the existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public abstract void UpdateListItemQuantity(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity);

        /// <summary>
        /// Replaces the item data view quantity. Does not add to the existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public abstract void RefreshQuantity(
            object selectedActivityItem,
            decimal quantity,
            decimal baseQuantity,
            decimal weightQuantity);

        /// <summary>
        /// Update the activity line item quantity in the database from the line item data view.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <param name="actItemRefno"><see cref="int"/> Activity item reference number.</param>
        /// <returns><see cref="Task"/> Taks operation.</returns>
        public abstract Task UpdateQuantityFromActivityItemList(
            object selectedActivityItem,
            int actItemRefno);

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type (t????Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public abstract int GetFKRefno(object resourceType);

        /// <summary>
        /// Get the product scheme data from the detail fk reference number.
        /// </summary>
        /// <param name="actDetailFKRefno"><see cref="int"/> Activity detail foreign key reference number.</param>
        /// <returns><see cref="Task{SActItemSchemeData}"/> Activity item scheme data.</returns>
        public abstract Task<SActItemSchemeData> GetProdSchemeDataFromDetailFKRefno(int actDetailFKRefno);

        /// <summary>
        /// Get activity item comment.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <returns><see cref="Task{string}"/> Line item comment.</returns>
        public abstract Task<string> GetComment(object selectedActivityItem);

        /// <summary>
        /// Submit and activity item.
        /// </summary>
        /// <param name="data">.</param>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Submit(XDocument data, object selectedActivityItem);

        /// <summary>
        /// Delete one or a set of activity item(s).
        /// </summary>
        /// <returns><see cref="int"/> Previous non deleted item foreign key reference number.</returns>
        public abstract Task<int> Delete();

        /// <summary>
        /// Create delete XML request.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Create delete XML request.</param>
        /// <returns><see cref="Task{string}"/> Delete XML.</returns>
        public abstract Task<string> CreateDeleteXML(XDocument data);

        /// <summary>
        /// Update the activity item.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Update data.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Update(XDocument data);

        /// <summary>
        /// Return the total quantity as a message string for displaying in a message box.
        /// </summary>
        /// <returns><see cref="string"/> Total quantity.</returns>
        public abstract Task<string> GetTotalQuantity();

        /// <summary>
        /// Select the activity item.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task SelectActivityItem(object selectedActivityItem);

        /// <summary>
        /// Get the default location map reference number for a warehouse, product.
        /// </summary>
        /// <param name="whsLocMapRefno"><see cref="int"/> Warehouse location map reference number.</param>
        /// <param name="prodRefno"><see cref="int"/> Product reference number.</param>
        /// <returns><see cref="int"/> Default location map refno or zero.</returns>
        public abstract Task<int> GetDefaultLocMapRefno(int whsLocMapRefno, int prodRefno);

        /// <summary>
        /// Get label quantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly"><see cref="bool"/> Is repack quantity only.</param>
        /// <returns><see cref="SLabelQuantity"/> Structure containing the label quantities.</returns>
        public abstract Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly);

        /// <summary>
        /// Check if the product catch ask is enabled.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch ask is enabled else false.</returns>
        public abstract bool IsCatchAsk();

        /// <summary>
        /// Check if the product catch ask zero is enabled.
        /// </summary>
        /// <returns><see cref="bool"/> True if catch ask zero is enabled else false.</returns>
        public abstract bool IsCatchAskZero();

        /// <summary>
        /// Determine what the selected location map should be when an activity line item or detail is selected.
        /// </summary>
        /// <param name="activityLocMapRefno"><see cref="int"/> Activity location map reference number.</param>
        /// <returns><see cref="Task{void}"/> Task operation.</returns>
        public virtual async Task GetSelectedItemLocation(int activityLocMapRefno)
        {
            SLocMap defaultLocMap = null;

            // The line item location may be a warehouse or event a slot.
            SLocMap itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(activityLocMapRefno);

            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                // If there is a skid, it's location will always be the warehouse product location.
                int locMapRefno = await this.GetDefaultLocMapRefno(
                    itemLocMap.WhsLocMapRefno,
                    this.ProdInfo.Refno);
                defaultLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);

                if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0)
                {
                    this.ActHdr.SkidInfo.CurrSkid.LocMap = defaultLocMap;
                }
            }
            else
            {
                // A skid or selected location must match the line item warehouse. This checked during
                // scanning or a manual adding of detail not during an item selection.
                if (this.SysEnv.SysSettings.IsVerifyProductLocation)
                {
                    if (this.ActHdr.SkidInfo.CurrSkid.SkidFKRefno > 0 &&
                        this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno == this.ProdInfo.Refno)
                    {
                        defaultLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                    }
                }
            }

            if (defaultLocMap == null || defaultLocMap.WhsLocMapRefno == 0)
            {
                defaultLocMap = LocationMap.GetInstance().ChosenLocMap;
            }

            if (defaultLocMap == null || defaultLocMap.WhsLocMapRefno == 0)
            {
                defaultLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                    LocationMap.GetInstance().LoginWhsLocMapRefno);
            }

            if (defaultLocMap == null || defaultLocMap.WhsLocMapRefno == 0)
            {
                defaultLocMap = new SLocMap();
            }

            this.LocMap.CurrLocMap = defaultLocMap;

            if (this.SysEnv.SysSettings.DoesSelectingADifferentProductClearSkid &&
                this.ActHdr.SkidInfo.CurrSkid.ProdInfo.Refno != this.ProdInfo.Refno)
            {
                this.ActHdr.SkidInfo.CurrSkid = new SSkidInfo();
            }
        }

        /// <summary>
        /// Get item not found message.
        /// </summary>
        /// <param name="locMap"><see cref="LocationMap"/> Location map reference number.</param>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product informations.</param>
        /// <returns><see cref="string"/> Item not found message.</returns>
        public virtual string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
        {
            string message = string.Format(
                "Item not found for " + Environment.NewLine + "Location {0} " +
                    Environment.NewLine + "Product {1} BC {2} Unit {3}",
                LocationMapTasks.GetCurrentReadableLocation(),
                prodInfo.Code,
                prodInfo.UnitBC,
                prodInfo.UnitCode);

            return message;
        }

        public virtual List<object> GetListItems()
        {
            return this.ListItems;
        }

        /// <summary>
        /// Perform validations, such as duplicate checks and quantity over / under validations.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity detail unique scheme id.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public virtual async Task IsReadyToApply(XDocument data)
        {
            // Check for duplicate
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                string actDetailUniqSchemeID = data.Root.Element("ActivityDetailUniqueSchemeID").Value;
                if (string.Compare(actDetailUniqSchemeID, string.Empty) > 0)
                {
                    bool isDupSchemeID = await ProductInfoTasks.IsDuplicateUniqueSchemeID(
                        actDetailUniqSchemeID);
                    if (isDupSchemeID)
                    {
                        throw new IsReadyToApplyException(
                            "Duplicate scheme id!" +
                            Environment.NewLine + actDetailUniqSchemeID,
                            false,
                            false);
                    }
                }
            }
        }

        /// <summary>
        /// Create activity.
        /// </summary>
        /// <param name="actSubTypes"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public async Task Create(SysCommon.ActSubTypes actSubTypes)
        {
            try
            {
                SQLiteAsyncConnection conn = DatabaseConnection.GetInstance().SQLConn;

                await conn.ExecuteAsync(
                    "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                    " ActivityRefno) VALUES (?, ?, ?)",
                    Convert.ToInt32(actSubTypes),
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(this.ActHdr.Refno));

                this.Refno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(
                    "SELECT last_insert_rowid() FROM tActivityItem"));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred.", ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Data layer status out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <param name="statusMessage"><see cref="string"/> Status message to display.</param>
        /// <param name="overrideDisconnectedMode"><see cref="bool"/> Override disconnected mode.</param>
        public void DataLayerStatusOutHandler(
            object sender,
            EventArgs e,
            string statusMessage,
            bool overrideDisconnectedMode)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || overrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = overrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = statusMessage;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress value.</param>
        /// <param name="minValue"><see cref="int"/> Progress minimum value.</param>
        /// <param name="maxValue"><see cref="int"/> Progress maximum value.</param>
        public void DataLayerProgressOutHandler(
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
        /// Data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender object.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
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
        /// On data access status out.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event arguments.</param>
        public void OnDataAccessStatusOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode ||
                dataAccessEventArgs.OverrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = dataAccessEventArgs.OverrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = dataAccessEventArgs.StatusOutMsg;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// On data access progress out.
        /// </summary>
        /// <param name="dataAccessEventPublisher"><see cref="IDataAccessEventPublisher"/> Data access event publisher.</param>
        /// <param name="dataAccessEventArgs"><see cref="DataAccessEventArgs"/> Data access event arguments.</param>
        public void OnDataAccessProgressOut(
            IDataAccessEventPublisher dataAccessEventPublisher,
            DataAccessEventArgs dataAccessEventArgs)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.OverrideDisconnectedMode = dataAccessEventArgs.OverrideDisconnectedMode;
                this.DataEventArgs.CurrentValue = dataAccessEventArgs.CurrentValue;
                this.DataEventArgs.MinValue = dataAccessEventArgs.MinValue;
                this.DataEventArgs.MaxValue = dataAccessEventArgs.MaxValue;
                this.DataEventArgs.SetMinMaxValue = dataAccessEventArgs.SetMinMaxValue;

                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Register the data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void RegLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut += new OnDataLayerStatusOutEventHandler(
                listener.OnLayerStatOut);
        }

        /// <summary>
        /// Unregister the data layer listener status out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut -= new OnDataLayerStatusOutEventHandler(
                listener.OnLayerStatOut);
        }

        /// <summary>
        /// Register the data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void RegLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut += new OnDataLayerProgressOutEventHandler(
                listener.OnLayerProgOut);
        }

        /// <summary>
        /// Unregister the data layer listener progress out.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access event listener.</param>
        public void UnregLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut -= new OnDataLayerProgressOutEventHandler(
                listener.OnLayerProgOut);
        }
    }
}
