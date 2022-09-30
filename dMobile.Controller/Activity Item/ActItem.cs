// <copyright file="clsActivityItem.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity_Item
{
    /// <summary>
    /// Activity item base class.
    /// </summary>
    public abstract class ActItem : IDataLayerEventPublisher, IDataAccessEventListener
    {
        /// <summary>
        /// Activity line item location map reference number.
        /// </summary>
        private int locMapRefno = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActItem"/> class.
        /// </summary>
        /// <param name="activity">.</param>
        public ActItem(Activity.Activity activity)
        {
            this.ActHdr = activity;
            this.ActSubType = this.ActHdr.ActSubType;
        }

        /// <summary>
        /// The data layer status out event handler.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="args">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public delegate void OnDataLayerStatusOutEventHandler(IDataLayerEventPublisher publisher, DataLayerEventArgs args);

        /// <summary>
        /// The data layer progress out event handler.
        /// </summary>
        /// <param name="publisher">The publisher<see cref="IDataLayerEventPublisher"/>.</param>
        /// <param name="args">The dataLayerEventArgs<see cref="DataLayerEventArgs"/>.</param>
        public delegate void OnDataLayerProgressOutEventHandler(IDataLayerEventPublisher publisher, DataLayerEventArgs args);

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
        public SLocMap LocMap { get; set; } = new SLocMap();

        /// <summary>
        /// Gets or sets the current location map reference number.
        /// </summary>
        public int LocMapRefno
        {
            get
            {
                if (this.LocMap.SlotLocMapRefno != 0)
                {
                    this.locMapRefno = this.LocMap.SlotLocMapRefno;
                }
                else if (this.LocMap.SectLocMapRefno != 0)
                {
                    this.locMapRefno = this.LocMap.SectLocMapRefno;
                }
                else
                {
                    this.locMapRefno = this.LocMap.WhsLocMapRefno;
                }

                return this.locMapRefno;
            }

            set
            {
                this.locMapRefno = value;
            }
        }

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
        public int ItemRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity item foreign key reference number..
        /// </summary>
        public int ItemFKRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the activity header..
        /// </summary>
        public Activity.Activity ActHdr { get; set; }

        /// <summary>
        /// Gets or sets the selected activity item..
        /// </summary>
        public object SelectedActivityItem { get; set; }

        /// <summary>
        /// Gets or sets the activity item list item selected index.
        /// </summary>
        public int SelectedListItemIdx { get; set; } = -1;


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
        /// <returns><see cref="Task"/> Task operation.</returns>
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
        /// Set the activity item selected activity item.
        /// </summary>
        /// <remarks>
        /// Iterate through the ListItems looking for a matched activity item reference number.
        /// If a match is found set the SelectedActivityItem to one found in the ListItems
        /// else the SelectedActivityItem is set to null.
        /// </remarks>
        /// <param name="actItemFoundRefno"><see cref="int"/> Activity item reference number to look up.</param>
        public abstract void SetSelectedActivityItem(int actItemFoundRefno);

        /// <summary>
        /// Get a list of activity items.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is batch submission.</param>
        /// <returns>.</returns>
        public abstract Task<List<object>> GetActivityItemList(bool isSubmitBatch);

        /// <summary>
        /// Design the list view grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/> Main activity list view.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/>.</returns>
        public abstract ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity);

        /// <summary>
        /// Get the activity item reference number from the product unit reference number and current location warehouse.
        /// </summary>
        /// <param name="prodUnitRefno"><see cref="int"/> Product unit reference number.</param>
        /// <returns><see cref="int"/> Found activity item reference number else 0.</returns>
        /// <remarks>
        /// The location map parameter most of the time will be the warehouse location map reference number, however for
        /// cycle count line items for it could be any location map sub type; slot, section or warehouse.
        /// </remarks>
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
        /// Add quantity to the existing activity item quantities. Does not reset existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item to change.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public abstract void AddToQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity);

        /// <summary>
        /// Replaces the activity item quantities. Does not add to the existing values.
        /// </summary>
        /// <param name="selectedActivityItem"><see cref="object"/> Selected activity item to change.</param>
        /// <param name="quantity"><see cref="decimal"/> Alt quantity.</param>
        /// <param name="baseQuantity"><see cref="decimal"/> Base quantity.</param>
        /// <param name="weightQuantity"><see cref="decimal"/> Weight quantity.</param>
        public abstract void ReplaceQuantityInActivityItem(object selectedActivityItem, decimal quantity, decimal baseQuantity, decimal weightQuantity);

        /// <summary>
        /// Update the activity item table data with the selected activity item object data.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task UpdateItemTableQuantity(object activityItem);

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type (t????Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public abstract int GetFKRefno(object resourceType);

        /// <summary>
        /// Get the product scheme data from the detail foreign key reference number.
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
        /// Assign the activity item values from the activity item parameter arguments.
        /// </summary>
        /// <param name="activityItem"><see cref="object"/> Activity item.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public abstract Task AssignActivityItem(object activityItem);

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
        /// <param name="itemLocMapRefno"><see cref="int"/> Activity item location map reference number.</param>
        /// <returns><see cref="Task{SLocMap}"/> Selected item location or null. Only complete locations are returned.</returns>
        public virtual async Task<SLocMap> GetSelectedItemLocation(int itemLocMapRefno)
        {
            SLocMap selectedLocMap = null;

            if (itemLocMapRefno <= 0)
            {
                // Current location should not be considered because the warehouse may not match the line item
                // location warehouse.
                // We can't use a activity header warehouse location because an activity header does not always have
                // a location. Line items already where filtered by a header location possibly upon refreshing data.
                itemLocMapRefno = LocationMap.GetInstance().LoginWhsLocMapRefno;
            }

            var itemLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(itemLocMapRefno);

            if (await LocationMapTasks.IsCompleteLocMap(itemLocMapRefno))
            {
                selectedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(itemLocMapRefno);
            }

            var isGetDefault = false;
            if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
            {
                if ((this.ActSubType == SysCommon.ActSubTypes.PhysicalCounts && this.SysEnv.SysSettings.IsPhysicalDoNotDefaultProductLocation)
                    || (this.ActSubType == SysCommon.ActSubTypes.CycleCounts && this.SysEnv.SysSettings.IsCycleDoNotDefaultProductLocation))
                {
                    isGetDefault = false;
                }
                else
                {
                    isGetDefault = true;
                }
            }

            // Even if the line item selected location is complete, default location will override.
            if (isGetDefault)
            {
                // Note: A selected skid location is not considered as a default, rather the skid location will be
                // assigned and updated in the calling method.
                // Default location however will be based on the any activity item warehouse or header warehouse.
                var locMapRefno = await this.GetDefaultLocMapRefno(itemLocMap.WhsLocMapRefno, this.ProdInfo.Refno);
                if (await LocationMapTasks.IsCompleteLocMap(locMapRefno))
                {
                    selectedLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(locMapRefno);
                }
            }
            else
            {
                // Verify product location is almost the same as defaulting, except the user can set the location but
                // it must match the default warehouse product location.
                // So we might as well default the location to the skid.
                if (this.SysEnv.SysSettings.IsVerifyProductLocation)
                {
                    var skidInfo = this.ActHdr.SkidInfo.CurrSkid;
                    if (skidInfo.SkidFKRefno > 0 && skidInfo.ProdInfo.Refno == this.ProdInfo.Refno &&
                        skidInfo.LocMap.WhsLocMapRefno == itemLocMap.WhsLocMapRefno)
                    {
                        if (await LocationMapTasks.IsCompleteLocMap(skidInfo.LocMap))
                        {
                            selectedLocMap = this.ActHdr.SkidInfo.CurrSkid.LocMap;
                        }
                    }
                }
            }

            // Current location is not considered because it is a result of the default rather than a source location.
            if (selectedLocMap == null || selectedLocMap.WhsLocMapRefno == 0)
            {
                if (await LocationMapTasks.IsCompleteLocMap(LocationMap.GetInstance().ChosenLocMap))
                {
                    selectedLocMap = LocationMap.GetInstance().ChosenLocMap;
                }
            }

            return selectedLocMap;
        }

        /// <summary>
        /// Get item not found message.
        /// </summary>
        /// <param name="locMap"><see cref="LocationMap"/> Location map reference number.</param>
        /// <param name="prodInfo"><see cref="ProductInfo"/> Product informations.</param>
        /// <returns><see cref="string"/> Item not found message.</returns>
        public virtual string GetItemNotFoundMessage(LocationMap locMap, ProductInfo prodInfo)
        {
            var message = string.Format(
                "Item not found for " + Environment.NewLine + "Location {0} " +
                    Environment.NewLine + "Product {1} BC {2} Unit {3}",
                LocationMapTasks.GetCurrentReadableLocation(),
                prodInfo.Code,
                prodInfo.UnitBC,
                prodInfo.UnitCode);

            return message;
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
                var actDetailUniqSchemeID = data.Root.Element("ActivityDetailUniqueSchemeID").Value;
                if (string.Compare(actDetailUniqSchemeID, string.Empty) > 0)
                {
                    var isDupSchemeID = await ProductInfoTasks.IsDuplicateUniqueSchemeID(
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
                var conn = DatabaseConnection.GetInstance().SQLConn;

                await conn.ExecuteAsync(
                    "INSERT INTO tActivityItem (ActivityItemSubType, ActivityItemStatus," +
                    " ActivityRefno) VALUES (?, ?, ?)",
                    Convert.ToInt32(actSubTypes),
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(this.ActHdr.HdrRefno));

                this.ItemRefno = Convert.ToInt32(await conn.ExecuteScalarAsync<int>(
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
