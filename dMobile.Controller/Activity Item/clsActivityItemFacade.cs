/// <summary>
/// This is the activity facade class that return an activity object.
/// </summary>
/// <remarks></remarks>
namespace dMobile.Controller
{
    using System;
    using dMobile.Bedrock;

    public class ActItemFacade
    {
        /// <summary>
        /// Creates and returns a generic activity object.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="actSubType"></param>
        /// <returns></returns>
        public static ActItem GetActItem(Activity activity, SysCommon.ActSubTypes actSubType)
        {
            ActItem actItem = null;
            ActItemFactory actItemFactory = null;

            switch (actSubType)
            {
                case SysCommon.ActSubTypes.BarCodeAssociation:
                    actItemFactory = new BarCodeActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    actItemFactory = new PhysicalActItemFactory();
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    actItemFactory = new CycleActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    actItemFactory = new RecvActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    actItemFactory = new PickingActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    actItemFactory = new PickSingleActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Returns:
                    actItemFactory = new ReturnsActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    actItemFactory = new TransferActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    actItemFactory = new TransferActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    actItemFactory = new AdjActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    actItemFactory = new TicketFinishedActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    actItemFactory = new TicketMaterialActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    actItemFactory = new TicketByProdActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    actItemFactory = new POWorkSheetActItemFactory();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    actItemFactory = new MoveInvActItemFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    actItemFactory = new ConvertUnitActItemFactory();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    actItemFactory = new SkidInfoActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    actItemFactory = new PutAwaySkidActItemFactory();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    actItemFactory = new AssignDefaultLocActItemFactory();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    actItemFactory = new InvInquiryActItemFactory();
                    break;

                case SysCommon.ActSubTypes.InventoryInquirySummary:
                    actItemFactory = new InvInquirySummaryActItemFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    actItemFactory = new ConvertProductActItemFactory();
                    break;

            }

            if (actItemFactory == null)
            {
                // TODO: Resource strings
                throw new NotImplementedException("Could not create activity item object.");
            }
            else
            {
                actItem = actItemFactory.GetActItem(activity);
                actItem.ActSubType = actSubType;
            }

            return actItem;
        }
    }
}
