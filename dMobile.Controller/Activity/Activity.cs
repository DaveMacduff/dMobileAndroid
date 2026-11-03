// <copyright file="Activity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Xml.Linq;
using dMobile.Bedrock;
using SQLite;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="Activity" /> Activity header base class.
    /// </summary>
    public abstract class Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Activity"/> class.
        /// </summary>
        protected Activity()
        {
            this.HdrFkRefno = 0;
            this.HdrRefno = 0;
            this.SelectedListItemIdx = -1;
            this.ListHeaders = new List<object>();
            this.ActSubType = SysCommon.ActSubTypes.NotApplicable;
            this.SkidInfo = new SysSkidInfo();
            this.SysEnv = SysEnvironment.GetInstance();
            this.DataEventArgs = new DataLayerEventArgs();
            this.SelectedActivity = null;
            this.XmlDataCriteria = null;
        }

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
        public int HdrFkRefno { get; set; }

        /// <summary>
        /// Gets or sets the list of activity headers.
        /// </summary>
        public List<object> ListHeaders { get; set; }

        /// <summary>
        /// Gets or sets the list of activity items.
        /// </summary>
        public List<object> ListItems { get; set; }

        /// <summary>
        /// Gets or sets the ActSubType.
        /// </summary>
        public SysCommon.ActSubTypes ActSubType { get; set; }

        /// <summary>
        /// Gets or sets the location map reference number.
        /// </summary>
        public int LocationMapRefno { get; set; }

        /// <summary>
        /// Gets or sets the Refno.
        /// </summary>
        public int HdrRefno { get; set; }

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
        public int SelectedListItemIdx { get; set; }

        /// <summary>
        /// Gets or sets the SysEnv.
        /// </summary>
        protected SysEnvironment SysEnv { get; set; }

        /// <summary>
        /// Gets or sets the SQLite asynchronous connection.
        /// </summary>
        protected SQLiteAsyncConnection Conn { get; set; } = DatabaseConnection.GetInstance().SqlConn;

        /// <summary>
        /// Gets the minimum amount of time that must elapse between successive full
        /// data refreshes (<see cref="Purge"/> + request header data) for this activity type.
        /// Guards against redundant network/DB round trips when a page is re-appeared in quick
        /// succession (e.g., rapid back/forward navigation).
        /// </summary>
        protected virtual TimeSpan RefreshThrottleInterval { get; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets the UTC timestamp of the last time this activity type performed a full
        /// data refresh (<see cref="Purge"/> + request header data).
        /// </summary>
        private DateTime LastRefreshUtc { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Perform a data refresh.
        /// </summary>
        /// <param name="data">The data<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task Refresh(XDocument data);

        /// <summary>
        /// Determine whether a full data refresh (<see cref="Purge"/> + request header data)
        /// should be skipped because one was already performed within <see cref="RefreshThrottleInterval"/>.
        /// </summary>
        /// <returns><see cref="bool"/> True if the refresh should be skipped (throttled).</returns>
        protected bool ShouldThrottleRefresh()
        {
            return DateTime.UtcNow - this.LastRefreshUtc < this.RefreshThrottleInterval;
        }

        /// <summary>
        /// Record that a full data refresh was just performed, resetting the throttle window.
        /// </summary>
        protected void MarkRefreshed()
        {
            this.LastRefreshUtc = DateTime.UtcNow;
        }

        /// <summary>
        /// Search for a text match.
        /// </summary>
        /// <param name="searchText">Text to search for.</param>
        /// <returns><see cref="ObservableCollection{T}"/> Found activity object or null.</returns>
        public abstract ObservableCollection<object> Search(string searchText);

        /// <summary>
        /// Get a list of activity objects.
        /// </summary>
        /// <param name="isDisconnectedMode"><see cref="bool"/> Is disconnected mode.</param>
        /// <returns><see cref="List{T}"/> List of activity objects.</returns>
        public abstract Task<List<object>> GetActivityList(bool isDisconnectedMode);

        /// <summary>
        /// Design the activity grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/>.</param>
        /// <returns>Main activity collection bound to view.</returns>
        public abstract ObservableCollection<object> DesignGrid(ref Grid gridMainActivity, ref ListView listViewMainActivity);

        /// <summary>
        /// Select an activity.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Activity to select.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public abstract Task SelectActivity(object selectedActivity);

        /// <summary>
        /// The AssignRefnoFromActivity.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Selected activity to get the refno for.</param>
        public abstract void AssignRefnoFromActivity(object selectedActivity);

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
        /// Construct the display extra information collection.
        /// </summary>
        /// <param name="extraInfo">Collection to add to.</param>
        public virtual void DisplayExtraInfo(ObservableCollection<string> extraInfo)
        {
            extraInfo.Clear();
        }

        /// <summary>
        /// Perform an activity header insert.
        /// </summary>
        /// <param name="xmlParams"><see cref="XDocument"/> Activity insert detail.</param>
        /// <returns><see cref="int"/> Activity reference number.</returns>
        public virtual Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Perform an activity deletion.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Print an activity label. Could be a shipping label for example.
        /// </summary>
        /// <param name="xmlParam"><see cref="XDocument"/> Label criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public virtual Task PrintLabel(XDocument xmlParam)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the label quantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly"><see cref="bool"/> Print repack quantity only.</param>
        /// <returns><see cref="SLabelQuantity"/> Structure containing the label quantities.</returns>
        public virtual Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check if all items have been reviewed.
        /// Under quantity check for ALL items.
        /// </summary>
        /// <param name="selectedActivity">The selectedActivity<see cref="object"/>.</param>
        /// <returns><see cref="bool"/> True if status was changed.</returns>
        public virtual Task<bool> ChangeStatus(object selectedActivity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create an activity header. Insert a tActivity record and assign this.Refno.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public async Task Create(SysCommon.ActSubTypes actSubType)
        {
            try
            {
                const string sql = "insert into tActivity (ActivityStatus, ActivitySubType) values (?, ?)";
                _ = await DatabaseConnection.GetInstance().SqlConn.ExecuteAsync(
                    sql,
                    Convert.ToInt32(SysCommon.ActStatus.Inserted),
                    Convert.ToInt32(actSubType));

                var queryId = "select last_insert_rowid() from tActivity";
                this.HdrRefno = Convert.ToInt32(
                    await DatabaseConnection.GetInstance().SqlConn.ExecuteScalarAsync<int>(queryId));
            }
            catch (SQLiteException ex)
            {
                throw new BaseException("Database exception occurred. Create activity failed.", ex);
            }
        }
    }
}
