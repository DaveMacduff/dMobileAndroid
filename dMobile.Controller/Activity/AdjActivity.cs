// <copyright file="AdjActivity.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using dMobile.Bedrock;
using dMobile.GroundWork;
using dMobile.Resources.Services;
using dMobile.Resources.Types;
using Xamarin.Forms;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Defines the <see cref="AdjActivity" /> adjustment activity header.
    /// </summary>
    public class AdjActivity : Activity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdjActivity"/> class.
        /// </summary>
        public AdjActivity()
          : base()
        {
            this.FullClassName = typeof(AdjActivity).FullName;
            this.AdjSettings = new AdjustmentSettings().LoadData();
            var dataTemplate = new DataTemplate(() =>
            {
                var gridData = new Grid
                {
                    Padding = new Thickness(5),
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                    },
                };

                var labelActivityStringFKRefno = new Label { VerticalOptions = LayoutOptions.Center };
                var labelActivityAdjustmentDateTime = new Label { VerticalOptions = LayoutOptions.Center };
                var labelActivityAdjustmentDescription = new Label { VerticalOptions = LayoutOptions.Center };

                labelActivityStringFKRefno.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityStringFKRefno"));
                labelActivityAdjustmentDateTime.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityAdjustmentDateTime", BindingMode.Default, null, null, "{0:MM/dd/yyyy}"));
                labelActivityAdjustmentDescription.SetBinding(
                    Label.TextProperty,
                    new Binding("ActivityAdjustmentDescription"));

                gridData.Children.Add(labelActivityStringFKRefno, 0, 0);
                gridData.Children.Add(labelActivityAdjustmentDateTime, 0, 0);
                gridData.Children.Add(labelActivityAdjustmentDescription, 0, 0);

                Grid.SetColumn(labelActivityStringFKRefno, 0);
                Grid.SetColumn(labelActivityAdjustmentDateTime, 1);
                Grid.SetColumn(labelActivityAdjustmentDescription, 2);

                return new ViewCell { View = gridData };
            });

            this.ResDict.Add("dataTemplateAdjustment", dataTemplate);
        }

        /// <summary>
        /// Gets or sets the adjustment settings.
        /// </summary>
        public AdjustmentSettings AdjSettings { get; set; }

        /// <summary>
        /// Gets or sets the list view / grid column 0 width.
        /// </summary>
        public double ColumnWidth0 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / grid column 1 width.
        /// </summary>
        public double ColumnWidth1 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the list view / grid column 2 width.
        /// </summary>
        public double ColumnWidth2 { get; set; } = 0;

        /// <summary>
        /// Gets or sets the resource dictionary of string for resource objects.
        /// </summary>
        public ResourceDictionary ResDict { get; set; } = new ResourceDictionary();

        /// <summary>
        /// Gets or sets a list of adjustment headers.
        /// </summary>
        public List<Adjustment> ListAdjustments { get; set; } = new List<Adjustment>();

        /// <summary>
        /// Purge adjustment headers, items and detail.
        /// </summary>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task Purge()
        {
            var adj = new TableActivityAdjustment();
            var adjItem = new TableActivityAdjustmentItem();
            var adjDetail = new TableActivityAdjustmentDetail();

            // NOTE: This should be in a transaction.
            await adjDetail.PurgeData();
            await adjItem.PurgeData();
            await adj.PurgeData();
        }

        /// <summary>
        /// Get the activity header foreign key reference index name.
        /// </summary>
        /// <returns><see cref="string"/> Index name.</returns>
        public override string GetFKRefnoIdx()
        {
            return "ActivityStringFKRefno";
        }

        /// <summary>
        /// Get the activity header control description.
        /// </summary>
        /// <returns><see cref="string"/> Control description.</returns>
        public override string GetControlDescription()
        {
            return "Adjustments";
        }

        /// <summary>
        /// Design the activity header grid.
        /// </summary>
        /// <param name="gridMainActivity"><see cref="Grid"/> Main activity header grid.</param>
        /// <param name="listViewMainActivity"><see cref="ListView"/> Main activity list.</param>
        /// <returns><see cref="ObservableCollection{object}"/> List of activity headers.</returns>
        public override ObservableCollection<object> DesignGrid(
            ref Grid gridMainActivity,
            ref ListView listViewMainActivity)
        {
            var headersList = new ObservableCollection<object>();

            var labelAdjustmentNumber = new Label
            {
                Text = "Adj #",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelAdjustmentDate = new Label
            {
                Text = "Date",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            var labelAdjustmentDescription = new Label
            {
                Text = "Description",
                VerticalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
            };

            // Calculate text size
            this.ColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentNumber.Text);
            this.ColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentDate.Text);
            this.ColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(labelAdjustmentDescription.Text);

            var gridHeader = new Grid
            {
                Padding = new Thickness(5),
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth0, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth1, GridUnitType.Absolute) },
                    new ColumnDefinition { Width = new GridLength(this.ColumnWidth2, GridUnitType.Absolute) },
                },
            };

            gridHeader.SetDynamicResource(VisualElement.BackgroundColorProperty, "HeaderColor");

            gridHeader.Children.Add(labelAdjustmentNumber);
            gridHeader.Children.Add(labelAdjustmentDate);
            gridHeader.Children.Add(labelAdjustmentDescription);

            Grid.SetColumn(labelAdjustmentNumber, 0);
            Grid.SetColumn(labelAdjustmentDate, 1);
            Grid.SetColumn(labelAdjustmentDescription, 2);

            Grid.SetRow(gridHeader, 0);

            gridMainActivity.Children.Add(gridHeader);

            foreach (Adjustment adjustment in this.ListHdrs)
            {
                headersList.Add(adjustment);

                var dataColumnWidth0 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    adjustment.ActivityStringFKRefno);
                var dataColumnWidth1 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    Convert.ToString(adjustment.ActivityAdjustmentDateTime.ToString("MM/dd/yyyy")));
                var dataColumnWidth2 = DependencyService.Get<IGridService>().CalculateTextWidth(
                    adjustment.ActivityAdjustmentDescription);
                if (dataColumnWidth0 > this.ColumnWidth0)
                {
                    this.ColumnWidth0 = dataColumnWidth0;
                    gridHeader.ColumnDefinitions[0].Width = this.ColumnWidth0;
                }

                if (dataColumnWidth1 > this.ColumnWidth1)
                {
                    this.ColumnWidth1 = dataColumnWidth1;
                    gridHeader.ColumnDefinitions[1].Width = this.ColumnWidth1;
                }

                if (dataColumnWidth2 > this.ColumnWidth2)
                {
                    this.ColumnWidth2 = dataColumnWidth2;
                    gridHeader.ColumnDefinitions[2].Width = this.ColumnWidth2;
                }
            }

            listViewMainActivity.ItemTemplate = (DataTemplate)this.ResDict["dataTemplateAdjustment"];

            return headersList;
        }

        /// <summary>
        /// Get list of activity headers.
        /// </summary>
        /// <param name="isSubmitBatch"><see cref="bool"/> Is submitting batch mode.</param>
        /// <returns><see cref="List{object}"/> List of headers.</returns>
        public override async Task<List<object>> GetActivityList(bool isSubmitBatch)
        {
            var adjSettings = new AdjustmentSettings().LoadData();
            var sql = new StringBuilder();

            if (isSubmitBatch)
            {
                _ = sql.Append("SELECT tActivityAdjustment.ActivityRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityFKRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityStringFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime AS ActivityAdjustmentDateTime,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDescription AS ActivityAdjustmentDescription,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentType AS ActivityAdjustmentType,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription AS ActivityAdjustmentTypeDescription,");
                _ = sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                _ = sql.Append(" FROM tActivityAdjustment");
                _ = sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                _ = sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                _ = sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                _ = sql.AppendFormat(" WHERE tActivity.ActivityStatus <> {0}", Convert.ToInt32(SysCommon.ActStatus.NoChange));
                _ = sql.Append(" UNION ");
                _ = sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityFKRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityStringFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
                _ = sql.Append("tActivity.ActivityStatus");
                _ = sql.Append(" FROM tActivityAdjustment");
                _ = sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                _ = sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                _ = sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                _ = sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                _ = sql.AppendFormat(
                    " WHERE tActivityItem.ActivityItemStatus <> {0}",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
                _ = sql.Append(" UNION ");
                _ = sql.Append(" SELECT tActivityAdjustment.ActivityRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityFKRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityStringFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseName,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDescription,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentType,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription,");
                _ = sql.Append("tActivity.ActivityStatus");
                _ = sql.Append(" FROM tActivityAdjustment");
                _ = sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
                _ = sql.Append(" INNER JOIN tActivityItem ON tActivityItem.ActivityRefno = tActivity.ActivityRefno");
                _ = sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                _ = sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                _ = sql.Append(" INNER JOIN tActivityDetail ON tActivityDetail.ActivityItemRefno = tActivityItem.ActivityItemRefno");
                _ = sql.AppendFormat(
                    " WHERE tActivityDetail.ActivityDetailStatus <> {0}",
                    Convert.ToInt32(SysCommon.ActStatus.NoChange));
            }
            else
            {
                _ = sql.Append("SELECT tActivityAdjustment.ActivityRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityFKRefno,");
                _ = sql.Append("tActivityAdjustment.ActivityStringFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseFKRefno AS LocationMapWarehouseFKRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapRefno AS LocationMapRefno,");
                _ = sql.Append("tLocationMapWarehouse.LocationMapWarehouseName AS LocationMapWarehouseName,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDateTime AS ActivityAdjustmentDateTime,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentDescription AS ActivityAdjustmentDescription,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentType AS ActivityAdjustmentType,");
                _ = sql.Append("tActivityAdjustment.ActivityAdjustmentTypeDescription AS ActivityAdjustmentTypeDescription,");
                _ = sql.Append("tActivity.ActivityStatus AS ActivityStatus");
                _ = sql.Append(" FROM tActivityAdjustment");
                _ = sql.Append(" INNER JOIN tLocationMapWarehouse ON tLocationMapWarehouse.LocationMapRefno = ");
                _ = sql.Append("tActivityAdjustment.WarehouseLocationMapRefno");
                _ = sql.Append(" INNER JOIN tActivity ON tActivity.ActivityRefno = tActivityAdjustment.ActivityRefno");
            }

            this.ListAdjustments = await DatabaseConnection.GetInstance().SQLConn.QueryAsync<Adjustment>(sql.ToString());

            foreach (var sort in adjSettings.GetDataViewSortBy())
            {
                if (sort.SortOrder == Resources.Enums.SortOrder.ASC)
                {
                    this.ListAdjustments = this.ListAdjustments.OrderBy(adj => adj.GetType().
                        GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
                else
                {
                    this.ListAdjustments = this.ListAdjustments.OrderByDescending(adj => adj.GetType().
                        GetProperty(sort.SortValue).GetValue(adj)).ToList();
                }
            }

            this.GetFilter();

            this.ListHdrs = this.ListAdjustments.Cast<object>().ToList();
            return this.ListHdrs;
        }

        /// <summary>
        /// Perform a search for a text match and return a list of matched headers.
        /// </summary>
        /// <param name="searchText"><see cref="string"/> String to search for.</param>
        /// <returns><see cref="ObservableCollection{object}"/> List of matched headers.</returns>
        public override ObservableCollection<object> Search(string searchText)
        {
            // TODO: Test search
            var activityHeaders = new ObservableCollection<object>();

            this.ListHdrs = this.ListAdjustments.Where(adjustment =>
                adjustment.ActivityStringFKRefno.ToLower().Contains(
                    searchText.ToLower().Trim())).Cast<object>().ToList();

            foreach (Adjustment adjustment in this.ListHdrs)
            {
                activityHeaders.Add(adjustment);
            }

            return activityHeaders;
        }

        /// <summary>
        /// Select the adjustment activity header.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Activity header to select.</param>
        /// <returns><see cref="Task"/> Task operation.</returns>
        public override async Task SelectActivity(object selectedActivity)
        {
            if (selectedActivity is Adjustment header)
            {
                this.SelectedActivity = header;
                this.HdrFKRefno = header.ActivityFKRefno;
                this.HdrRefno = header.ActivityRefno;

                if (this.SysEnv.SysSettings.IsDefaultLocToProductDefaultLoc)
                {
                    if (LocationMap.GetInstance().CurrLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().CurrLocMap = await LocationMapTasks.GetSLocMapFromLocMapRefno(
                            header.LocationMapRefno);
                    }

                    if (LocationMap.GetInstance().ChosenLocMap.WhsLocMapRefno != header.LocationMapRefno &&
                            await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        LocationMap.GetInstance().ChosenLocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }

                    if (this.SkidInfo.CurrSkid != null && this.SkidInfo.CurrSkid.SkidFKRefno > 0 &&
                        this.SkidInfo.CurrSkid.LocMapRefno != header.LocationMapRefno &&
                        await LocationMapTasks.IsCompleteLocMap(header.LocationMapRefno))
                    {
                        // NOTE: We may have to use a temp var to assign the current skid a new location.
                        this.SkidInfo.CurrSkid.LocMap = LocationMap.GetInstance().CurrLocMap.Clone();
                    }
                }

                this.AdjSettings = this.AdjSettings.LoadData();
                this.AdjSettings.AdjustmentType = (SysCommon.AdjTypes)Convert.ToInt32(header.ActivityAdjustmentType);

                if (!SysCommon.IsAllowedChangeNegateQuantityFromAdjType(this.AdjSettings.AdjustmentType))
                {
                    this.AdjSettings.NegateScanQuantity = SysCommon.DefaultNegateQuantityFromAdjType(
                        this.AdjSettings.AdjustmentType);
                    var tempAdjSettings = this.AdjSettings;
                    _ = this.AdjSettings.SaveData(ref tempAdjSettings);
                    this.AdjSettings = tempAdjSettings;
                }
            }
        }

        /// <summary>
        /// Get the activity reference number for the selected object.
        /// </summary>
        /// <param name="selectedItem"><see cref="object"/> Selected item.</param>
        /// <returns><see cref="int"/> Selected activity reference number.</returns>
        public override int GetRefnoFromRowIdx(object selectedItem)
        {
            if (selectedItem is Adjustment adjustment)
            {
                this.HdrFKRefno = Convert.ToInt32(adjustment.ActivityFKRefno);
                this.HdrRefno = Convert.ToInt32(adjustment.ActivityRefno);
            }

            return this.HdrRefno;
        }

        /// <summary>
        /// Refresh the activity headers.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Activity header filter criteria.</param>
        /// <returns>The <see cref="Task"/> Task operation.</returns>
        public override async Task Refresh(XDocument data)
        {
            if (this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);
            }
            else
            {
                this.DataLayerProgressOutHandler(this, new EventArgs(), 2);
                await this.Purge();

                this.DataLayerProgressOutHandler(this, new EventArgs(), 5);

                var adj = new TableActivityAdjustment();
                adj.RegisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
                await adj.RequestData();
                adj.UnregisterDataAccessListenerProgressOut((IDataAccessEventListener)this);
            }
        }

        /// <summary>
        /// Apply adjustment activity header filter.
        /// </summary>
        public override void GetFilter()
        {
            var filter = string.Empty;
            var adjSettings = new AdjustmentSettings().LoadData();

            if (adjSettings.IsApplyAdjustmentType)
            {
                this.ListAdjustments = this.ListAdjustments.Where(adj => adj.ActivityAdjustmentType == Convert.ToInt32(
                    adjSettings.AdjustmentType)).ToList();
            }

            if (adjSettings.IsApplyAdjustmentDate)
            {
                if (!SysCommon.IsEmptyDate(adjSettings.AdjustmentDate))
                {
                    var dt = adjSettings.AdjustmentDate;
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                    this.ListAdjustments = this.ListAdjustments.Where(adj => adj.ActivityAdjustmentDateTime >= minDate &&
                        adj.ActivityAdjustmentDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// Apply adjustment activity header filter.
        /// </summary>
        /// <param name="xmlDoc"><see cref="XDocument"/> Adjustment header extra filter criteria.</param>
        public override void GetFilter(XDocument xmlDoc)
        {
            var filter = string.Empty;
            var adjSettings = new AdjustmentSettings().LoadData();

            if (adjSettings.IsApplyAdjustmentType)
            {
                if (string.Compare(xmlDoc.Root.Element("adjustment_type").Value, string.Empty) > 0)
                {
                    this.ListAdjustments = this.ListAdjustments.Where(adj => adj.ActivityAdjustmentType == Convert.ToInt32(
                        xmlDoc.Root.Element("adjustment_type").Value)).ToList();
                }
            }

            if (adjSettings.IsApplyAdjustmentDate)
            {
                if (string.Compare(xmlDoc.Root.Element("adjustment_date").Value, string.Empty) > 0)
                {
                    var dt = DateTime.Parse(xmlDoc.Root.Element("adjustment_date").Value);
                    var minDate = new DateTime(dt.Year, dt.Month, dt.Day, 00, 00, 00);
                    var maxDate = new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
                    this.ListAdjustments = this.ListAdjustments.Where(adj => adj.ActivityAdjustmentDateTime >= minDate &&
                        adj.ActivityAdjustmentDateTime <= maxDate).ToList();
                }
            }
        }

        /// <summary>
        /// Print debox product labels.
        /// </summary>
        /// <param name="data"><see cref="XDocument"/> Product label criteria.</param>
        /// <returns><see cref="Task"/> task operation.</returns>
        public override async Task PrintLabel(XDocument data)
        {
            if (this.AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox && !this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                var sLocMapAndRefno = await LocationMapTasks.GetSLocMapFromLocMapFKRefno(
                    int.Parse(this.XMLDataCriteria.Root.Element("whs_id").Value),
                    int.Parse(this.XMLDataCriteria.Root.Element("sect_refno").Value),
                    slotLocMapFKRefno: 0);
                var locMap = sLocMapAndRefno.SLocMap;
                var prnSettings = new PrinterSettings().LoadData();

                // Don't even bother sending in a print request if no quantity was entered.
                var labelQuantity = int.Parse(data.Root.Value);

                // Used abs for handling the freaky event that a negative label quantity was requested.
                if (labelQuantity > 0)
                {
                    var request =
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>" + "<imclient>"
                    + "<command>"
                    + "<action>SUBMIT PRINT DEBOX PRODUCT LABEL</action>"
                    + "</command>"
                    + await SysCommon.GetRequestDefaults(false)
                    + "<refno>" + this.HdrFKRefno + "</refno>"
                    + "<whs_id>" + locMap.WhsFKRefno + "</whs_id>"
                    + "<sect_refno>" + locMap.SectFKRefno + "</sect_refno>"
                    + "<label_qty>" + labelQuantity + "</label_qty>"
                    + "<mobile_printer_name>"
                    + (prnSettings.IsEnabledProductLabelPrinter ? prnSettings.ProductLabelPrinterName : string.Empty)
                    + "</mobile_printer_name>"
                    + "</imclient>";

                    var reqData = SysCommon.SubmitComm(request);

                    if (reqData.Length > 0)
                    {
                        // check if result is an error message
                        if (reqData.Contains("<message>ERROR</message>"))
                        {
                            var docError = new XmlDocument();
                            docError.LoadXml(reqData);
                            throw new MessageOnlyException(string.Format(
                                "Order# {0} print shipping label(s) failed." + Environment.NewLine + "{1}",
                                this.HdrFKRefno,
                                docError.DocumentElement["detail"].InnerText));
                        }
                    }
                    else
                    {
                        throw new Exception(string.Format("Order# {0} Print shipping label(s) failed.", this.HdrFKRefno));
                    }
                }
            }
        }

        /// <summary>
        /// Change the activity header status.
        /// </summary>
        /// <param name="selectedActivity"><see cref="object"/> Selected activity to change the status on.</param>
        /// <returns><see cref="Task{bool}"/> True if successful else false.</returns>
        public override async Task<bool> ChangeStatus(object selectedActivity)
        {
            var result = false;
            if (this.AdjSettings.AdjustmentType == SysCommon.AdjTypes.Debox && !this.SysEnv.SysSettings.IsDisconnectedMode)
            {
                // do xml request to debox
                var xml =
                    "<?xml version=\"1.0\" encoding=\"utf-16\" standalone=\"yes\"?>"
                      + "<imclient>"
                      + "<command>"
                      + "<action>COMPLETE DEBOX ADJUSTMENT</action>"
                      + "</command>"
                      + await SysCommon.GetRequestDefaults(false)
                      + "<refno>" + this.HdrFKRefno + "</refno>"
                      + "</imclient>";

                var reqData = SysCommon.SubmitComm(xml);

                if (reqData.Length > 0)
                {
                    if (reqData.Contains("<message>ERROR</message>"))
                    {
                        var docError = new XmlDocument();
                        docError.LoadXml(reqData);
                        var errorDetail = docError.DocumentElement["detail"].InnerText;

                        throw new Exception(string.Format("Debox failed. {0}", Environment.NewLine + errorDetail));
                    }
                }
                else
                {
                    throw new Exception("Debox failed. No COM service response.");
                }

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Get the activity foreign key reference number.
        /// </summary>
        /// <param name="resourceType"><see cref="object"/> Resource model type (tActivity??Model).</param>
        /// <returns><see cref="int"/> Found activity foreign key refno, else 0 if not found.</returns>
        public override int GetFKRefno(object resourceType)
        {
            object tmpFKRefno = ((Adjustment)resourceType).ActivityFKRefno;
            return tmpFKRefno == null ? 0 : Convert.ToInt32(tmpFKRefno);
        }

        /// <summary>
        /// Insert, create a new activity adjustment header.
        /// </summary>
        /// <param name="xmlParams">The xmlParams<see cref="XDocument"/>.</param>
        /// <returns>The <see cref="Task{int}"/>.</returns>
        public override Task<int> Insert(XDocument xmlParams)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete the adjustment activity header.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        public override Task Delete()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get shipping label quantity.
        /// </summary>
        /// <param name="isRepackQuantityOnly">The isRepackQuantityOnly<see cref="bool"/>.</param>
        /// <returns>The <see cref="Task{SLabelQuantity}"/>.</returns>
        public override Task<SLabelQuantity> GetLabelQuantity(bool isRepackQuantityOnly)
        {
            throw new NotImplementedException();
        }
    }
}
