/// <summary>
/// This is the activity facade class that return an activity object.
/// </summary>
/// <remarks></remarks>
namespace dMobile.Controller
{
    using System;
    using dMobile.Bedrock;

    public class ActDetailFacade
    {
        #region Public Members

        /// <summary>
        /// Creates and returns a generic activity object.
        /// </summary>
        /// <param name="actItem"></param>
        /// <param name="actSubType"></param>
        /// <returns></returns>
        public static ActDetail GetActDetail(ActItem actItem, SysCommon.ActSubTypes actSubType)
        {
            ActDetail actDetail = null;
            ActDetailFactory actDetailFactory = null;

            switch (actSubType)
            {
                case SysCommon.ActSubTypes.BarCodeAssociation:
                    actDetailFactory = new BarCodeAssocActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.PhysicalCounts:
                    actDetailFactory = new PhysicalCountActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.CycleCounts:
                    actDetailFactory = new CycleCountActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    actDetailFactory = new PickingActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.PickingSingle:
                    actDetailFactory = new PickingActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.Returns:
                    actDetailFactory = new ReturnsActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                    actDetailFactory = new RecvActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.TransferFrom:
                    actDetailFactory = new TransferActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.TransferTo:
                    actDetailFactory = new TransferActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.Adjustment:
                    actDetailFactory = new AdjActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.TicketFinished:
                    actDetailFactory = new TicketFinishedDetailFactory();
                    break;

                case SysCommon.ActSubTypes.TicketMaterial:
                    actDetailFactory = new TicketMaterialDetailFactory();
                    break;

                case SysCommon.ActSubTypes.TicketAddProduct:
                    actDetailFactory = new TicketByProdDetailFactory();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    actDetailFactory = new POWorkSheetActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.MoveInventory:
                    actDetailFactory = new clsMoveInventoryActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertUnit:
                    actDetailFactory = new ConvertUnitActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.SkidInfo:
                    actDetailFactory = new SkidInfoActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.PutAwaySkid:
                    actDetailFactory = new PutAwaySkidActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.AssignDefaultLocation:
                    actDetailFactory = new AssignDefaultLocActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.InventoryInquiry:
                    actDetailFactory = new InventoryInquiryActDetailFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    actDetailFactory = new ConvertProductActDetailFactory();
                    break;
            }

            if (actDetailFactory == null)
            {
                // TODO: Resource strings
                throw new NotImplementedException("Could not create activity detail object.");
            }
            else
            {
                actDetail = actDetailFactory.GetActDetail(actItem);
                actDetail.ActSubType = actSubType;
            }

            return actDetail;
        }

        #endregion
    }
}