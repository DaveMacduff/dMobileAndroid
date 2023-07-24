// <copyright file="Activity.cs" company="InterSect Business Systems Inc">
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

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="Activity" /> Activity header base class.
    /// </summary>
    public abstract class Activity : IDataLayerEventPublisher, IDataAccessEventListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        protected Activity()
        {

            this.HdrFkRefno = 0;
            this.HdrRefno = 0;
            this.SelectedListItemIdx = -1;
            this.ListHdrs = new List<object>();
            this.ActSubType = SysCommon.ActSubTypes.NotApplicable;
            this.SkidInfo = new SysSkidInfo();
            this.SysEnv = SysEnvironment.GetInstance();
            this.FullClassName = typeof(Activity).FullName;
            this.DataEventArgs = new DataLayerEventArgs();
            this.SelectedActivity = null;
            this.XmlDataCriteria = null;
        }

        /// <summary>
        /// The on data layer status out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        public delegate void DataLayerStatusOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        /// The on data layer progress out event handler.
        /// </summary>
        /// <param name="publisher"><see cref="IDataLayerEventPublisher"/> Data layer event publisher.</param>
        /// <param name="eventArgs"><see cref="DataLayerEventArgs"/> Data layer event arguments.</param>
        public delegate void DataLayerProgressOutEventHandler(
            IDataLayerEventPublisher publisher,
            DataLayerEventArgs eventArgs);

        /// <summary>
        /// On data layer status out event handler.
        /// </summary>
        public event DataLayerStatusOutEventHandler OnDataLayerStatusOut;

        /// <summary>
        /// On data layer progress out event handler.
        /// </summary>
        public event DataLayerProgressOutEventHandler OnDataLayerProgressOut;

        /// <summary>
        /// Gets or sets the full class name. Used for error reporting.
        /// </summary>
        public string FullClassName { get; set; }

        /// <summary>
        /// Gets or sets the skid information.
        /// </summary>
        /// <remarks>
        /// Skid info is on the header activity because working with skid maintenance does have a
        /// header.
        /// </remarks>
        public SysSkidInfo SkidInfo { get; set; }

        /// <summary>
        /// Gets or sets the XML data criteria. Used for data exchange between activities.
        /// </summary>
        public XDocument XmlDataCriteria { get; set; }

        /// <summary>
        /// Gets or sets the activity foreign key reference number.
        /// </summary>
        public int HdrFkRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list of activity headers.
        /// </summary>
        public List<object> ListHdrs { get; set; }

        /// <summary>
        /// Gets or sets the list of activity items.
        /// </summary>
        public List<object> ListItems { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the Refno.
        /// </summary>
        public int HdrRefno { get; set; } = 0;

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        public SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the DataEventArgs.
        /// </summary>
        public DataLayerEventArgs DataEventArgs { get; set; }

        /// <summary>
        /// Gets or sets the SelectedActivity.
        /// </summary>
        public object SelectedActivity { get; set; }

        /// <summary>
        /// Gets or sets the activity item list item selected index.
        /// </summary>
        public int SelectedListItemIdx { get; set; } = 0;

        /// <summary>
        /// Perform a data refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task Refresh(XDocument data);

        /// <summary>
        /// Search for a text match.
        /// </summary>
        /// <param name="searchText">.</param>
        /// <returns><see cref="ObservableCollection{T}"/> Found object.</returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// Get a list of activity objects.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is submit batch operation.</param>
        /// <returns><see cref="List{T}"/> List of activity objects.</returns>
        public abstract Task<List<object>> GetActivityList(bool isSubmitBatch);

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/>.</param>
        /// <returns>The <see cref="ObservableCollection{object}"/> Main activity list view.</returns>
        public abstract ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity);

        /// <summary>
        /// Select an activity.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Activity to select.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task SelectActivity(object selectedActivity);

        /// <summary>
        /// The GetRefnoFromRowIdx.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Selected activity to get the refno for.</param>
        /// <returns>.</returns>
        public abstract int GetRefnoFromRowIdx(object selectedActivity);

        /// <summary>
        /// Get the activity filter.
        /// </summary>
        public abstract void GetFilter();

        /// <summary>
        /// Get the activity filter.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Filter criteria.</param>
        public abstract void GetFilter(XDocument data);

        /// <summary>
        /// Get the activity control description.
        /// </summary>
        /// <returns><see cref="string"/> Activity control description.</returns>
        public abstract string GetControlDescription();

        /// <summary>
        /// Get the foreign key refno index.
        /// </summary>
        /// <returns>.</returns>
        public abstract string GetFkRefnoIdx();

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type (t????Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key reference number, else 0 if not found.</returns>
        public abstract int GetFkRefno(object resourceType);

        /// <summary>
        /// Purge all activity data.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task Purge();

        /// <summary>
        /// Make create activity submission XML.
        /// </summary>
        /// <returns>.</returns>
        public virtual XDocument CreateSubmitXml()
        {
            var xmldocSubmission = XDocument.Parse(
                "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                + "<imclient>"
                + "<command>"
                + "</command>"
                + "</imclient>");

            return xmldocSubmission;
        }

        /// <summary>
        /// Perform an activity header insert.
        /// </summary>
        /// <param name="xmlParams"><see cref="XDocument"/> Activity insert detail.</param>
        /// <returns><see cref="int"/> Activity reference number.</returns>
        public abstract Task<int> Insert(XDocument xmlParams);

        /// <summary>
        /// Perform an activity deletion.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task Delete();

        /// <summary>
        /// Print an activity label. Could be a shipping label for example.
        /// </summary>
        /// <param name="xmlParam"><see cref="XDocument"/> Label criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public abstract Task PrintLabel(XDocument xmlParam);

        /// <summary>
        /// Get the label quantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly"><see cref="bool"/> Print repack quantity only.</param>
        /// <returns><see cref="SLabelQuantity"/> Structure containing the label quantities.</returns>
        public abstract Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly);

        /// <summary>
        /// Check if all items have been reviewed.
        /// Under quantity check for ALL items.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns><see cref="bool"/> True if status was changed.</returns>
        public abstract Task<bool> ChangeStatus(object selectedActivity);

        /// <summary>
        /// Create an activity header. Insert a tActivity record and assign this.Refno.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public async Task Create(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                var sql = "insert into tActivity (ActivityStatus, ActivitySubType) values (?, ?)";
                _ = await DatabaseConnection.GetInstance().SQLConn.ExecuteAsync(
                    sql,
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(actSubType));

                var queryId = "select last_insert_rowid() from tActivity";
                this.HdrRefno = Convert.ToInt32(
                    await DatabaseConnection.GetInstance().SQLConn.ExecuteScalarAsync<int>(queryId));
            }
            catch (SQLiteException ex)
            {
                throw new Exception("Database exception occurred. Create activity failed.", ex);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// The data layer status out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <param name="outMessage"><see cref="string"/> Status out message.</param>
        /// <param name="overrideDisconnectedMode"><see cref="bool"/> Override disconnected mode.</param>
        public void DataLayerStatusOutHandler(
            object sender,
            EventArgs e,
            string outMessage,
            bool overrideDisconnectedMode)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (string.IsNullOrEmpty(outMessage))
            {
                throw new ArgumentException($"'{nameof(outMessage)}' cannot be null or empty.", nameof(outMessage));
            }

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
        /// The data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress value.</param>
        /// <param name="minValue"><see cref="int"/> Minimum progress value.</param>
        /// <param name="maxValue"><see cref="int"/> Maximum progress value.</param>
        public void DataLayerProgressOutHandler(
            object sender,
            EventArgs e,
            int currentValue,
            int minValue,
            int maxValue)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

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
        /// The data layer progress out handler.
        /// </summary>
        /// <param name="sender"><see cref="object"/> Sender instance.</param>
        /// <param name="e"><see cref="EventArgs"/> Sender object event arguments.</param>
        /// <param name="currentValue"><see cref="int"/> Current progress value.</param>
        public void DataLayerProgressOutHandler(object sender, EventArgs e, int currentValue)
        {
            if (sender is null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (e is null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.CurrentValue = currentValue;
                this.DataEventArgs.SetMinMaxValue = false;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Data access status out event publisher.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Event publisher.</param>
        /// <param name="args"><see cref="DataAccessEventArgs"/> Publisher event arguments.</param>
        public void OnDataAccessStatusOut(
            IDataAccessEventPublisher publisher,
            DataAccessEventArgs args)
        {
            if (!this.SysEnv.SysSettings.IsDisconnectedMode || args.OverrideDisconnectedMode)
            {
                if (this.OnDataLayerStatusOut != null)
                {
                    this.DataEventArgs.OverrideDisconnectedMode = args.OverrideDisconnectedMode;
                    this.DataEventArgs.StatusOutMsg = args.StatusOutMsg;
                    this.OnDataLayerStatusOut(this, this.DataEventArgs);
                }
            }
        }

        /// <summary>
        /// Data access progress out event publisher.
        /// </summary>
        /// <param name="publisher"><see cref="IDataAccessEventPublisher"/> Event publisher.</param>
        /// <param name="args"><see cref="DataAccessEventArgs"/> Publisher event arguments.</param>
        public void OnDataAccessProgressOut(
            IDataAccessEventPublisher publisher,
            DataAccessEventArgs args)
        {
            if (this.OnDataLayerProgressOut != null)
            {
                this.DataEventArgs.OverrideDisconnectedMode = args.OverrideDisconnectedMode;
                this.DataEventArgs.CurrentValue = args.CurrentValue;
                this.DataEventArgs.MinValue = args.MinValue;
                this.DataEventArgs.MaxValue = args.MaxValue;
                this.DataEventArgs.SetMinMaxValue = args.SetMinMaxValue;
                this.OnDataLayerProgressOut(this, this.DataEventArgs);
            }
        }

        /// <summary>
        /// Register data layer status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access
        /// status out listener.</param>
        public void RegLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut += new DataLayerStatusOutEventHandler(
                listener.OnLayerStatOut);
        }

        /// <summary>
        /// Unregister data layer status out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access
        /// status out listener.</param>
        public void UnregLayerListenerStatOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerStatusOut -= new DataLayerStatusOutEventHandler(
                listener.OnLayerStatOut);
        }

        /// <summary>
        /// Register data layer progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access
        /// status out listener.</param>
        public void RegLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut += new DataLayerProgressOutEventHandler(
                listener.OnLayerProgressOut);
        }

        /// <summary>
        /// Unregister data layer progress out listener.
        /// </summary>
        /// <param name="listener"><see cref="IDataAccessEventListener"/> Data access
        /// status out listener.</param>
        public void UnregLayerListenerProgOut(IDataLayerEventListener listener)
        {
            this.OnDataLayerProgressOut -= new DataLayerProgressOutEventHandler(
                listener.OnLayerProgressOut);
        }
    }
}
