/// <summary>
/// This is the activity facade class that return an activity object.
/// </summary>
/// <remarks></remarks>
namespace dMobile.Controller
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Diagnostics;
    using System.Xml.Linq;
    using dMobile.Bedrock;

    public class ActivityFacade
    {
        /// <summary>
        /// Creates and returns a generic activity object.
        /// </summary>
        /// <param name="actSubType"></param>
        /// <returns></returns>
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

                case SysCommon.ActSubTypes.Transfers:
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
                    actFactory = new POWorkSheetActivityFactory();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    actFactory = new MoveInvActivityFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    actFactory = new ConvertUnitActivityFactory();
                    break;

                case SysCommon.ActSubTypes.MoveSkid:
                    actFactory = new clsMoveSkidActivityFactory();
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
            }

            Activity activity = null;

            if (actFactory == null)
            {

                // TODO: Resource strings
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
