// <copyright file="ActivityFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

/// <summary>
/// This is the activity facade class that return an activity object.
/// </summary>

using System;
using dMobile.Bedrock;

namespace dMobile.Controller.Activity
{
    /// <summary>
    /// Activity facade to create activity objects based on an activity sub type.
    /// </summary>
    public class ActivityFacade
    {
        /// <summary>
        /// Creates activity object based on the activity sub type.
        /// </summary>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes"/> Activity sub type.</param>
        /// <returns><see cref="Activity"/> Created activity instance.</returns>
        public static Activity GetActivity(SysCommon.ActSubTypes actSubType)
        {
            ActivityFactory actFactory = null;

            switch (actSubType)
            {
                case SysCommon.ActSubTypes.BarCodeAssociation:
                    actFactory = new BarCodeAssocActivityFactory();
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    actFactory = new PhysicalActivityFactory();
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    actFactory = new CycleActivityFactory();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    actFactory = new PickingActivityFactory();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    actFactory = new PickingActivityFactory();
                    break;

                case SysCommon.ActSubTypes.ReleaseByOrder:
                    actFactory = new ReleaseByOrderActivityFactory();
                    break;
                case SysCommon.ActSubTypes.ReleaseByItemSection:
                    break;

                case SysCommon.ActSubTypes.Returns:
                    actFactory = new ReturnsActivityFactory();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    actFactory = new RecvFactory();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    actFactory = new TransferActivityFactory();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    actFactory = new TransferActivityFactory();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    actFactory = new AdjActivityFactory();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    actFactory = new TicketFinishedActivityFactory();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    actFactory = new TicketMaterialActivityFactory();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    actFactory = new TicketByProdActivityFactory();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    actFactory = new PoWorkSheetActivityFactory();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    actFactory = new MoveInvActivityFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    actFactory = new ConvertUnitActivityFactory();
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    actFactory = new MoveSkidActivityFactory();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    actFactory = new SkidInfoActivityFactory();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    actFactory = new PutAwaySkidActivityFactory();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    actFactory = new AssignDefaultLocActivityFactory();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    actFactory = new InvInquiryActivityFactory();
                    break;

                case SysCommon.ActSubTypes.InventoryInquirySummary:
                    actFactory = new InvInquirySummaryActivityFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    actFactory = new ConvertProductActivityFactory();
                    break;

                case SysCommon.ActSubTypes.NotApplicable:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.RefreshData:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.DataStatus:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.SelectProduct:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.SelectLocation:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.PurgeActivityData:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.BreakSkid:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.InventoryReplenishment:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.LoginStartup:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.SplashStartup:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.PrintProductLabel:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.PickingSelectCriteria:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.PickingFilter:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.SubmitBatch:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.CommunicationSetup:
                    throw new NotImplementedException();

                case SysCommon.ActSubTypes.SetupDefaults:
                    throw new NotImplementedException();

                default:
                    throw new NotImplementedException();
            }

            Activity activity;

            if (actFactory == null)
            {
                throw new NotImplementedException("Could not create activity object.");
            }
            else
            {
                activity = actFactory.GetActivity();
                activity.ActSubType = actSubType;
            }

            return activity;
        }
    }
}
