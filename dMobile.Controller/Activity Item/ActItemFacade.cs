// <copyright file="ActItemFacade.cs" company="InterSect Business Systems Inc">
// Copyright (c) InterSect Business Systems Inc. All rights reserved.
// </copyright>

namespace dMobile.Controller.Activity_Item
{
    using System;
    using Activity;
    using Bedrock;

    /// <summary>
    ///     This is the activity facade class that return an activity object.
    /// </summary>
    public class ActItemFacade
    {
        /// <summary>
        ///     Creates and returns a generic activity object.
        /// </summary>
        /// <param name="activity"><see cref="Activity" /> Activity header.</param>
        /// <param name="actSubType"><see cref="SysCommon.ActSubTypes" /> Activity sub type.</param>
        /// <returns><see cref="ActItem" /> Activity item object.</returns>
        public static ActItem GetActItem(Activity activity, SysCommon.ActSubTypes actSubType)
        {
            ActItemFactory actItemFactory = null;

            switch (actSubType)
            {
                case SysCommon.ActSubTypes.BarCodeAssociation:
                    actItemFactory = new BarCodeActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PhysicalCountItemsList:
                case SysCommon.ActSubTypes.PhysicalCounts:
                    actItemFactory = new PhysicalActItemFactory();
                    break;

                case SysCommon.ActSubTypes.CycleCountItemsList:
                case SysCommon.ActSubTypes.CycleCounts:
                    actItemFactory = new CycleActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Receiving:
                case SysCommon.ActSubTypes.ReceiveItemsList:
                    actItemFactory = new RecvActItemFactory();
                    break;

                case SysCommon.ActSubTypes.Picking:
                    actItemFactory = new PickSingleActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PickingItemsList:
                    actItemFactory = new PickingActItemFactory();
                    break;

                case SysCommon.ActSubTypes.ReturnItemsList:
                case SysCommon.ActSubTypes.Returns:
                    actItemFactory = new ReturnsActItemFactory();
                    break;

                case SysCommon.ActSubTypes.TransferItemsList:
                case SysCommon.ActSubTypes.TransferFrom:
                case SysCommon.ActSubTypes.TransferTo:
                    actItemFactory = new TransferActItemFactory();
                    break;

                case SysCommon.ActSubTypes.AdjustmentItemsList:
                case SysCommon.ActSubTypes.Adjustment:
                    actItemFactory = new AdjActItemFactory();
                    break;

                case SysCommon.ActSubTypes.FinishedGoodsItemsList:
                case SysCommon.ActSubTypes.TicketFinished:
                    actItemFactory = new TicketFinishedActItemFactory();
                    break;

                case SysCommon.ActSubTypes.RawMaterialItemsList:
                case SysCommon.ActSubTypes.TicketMaterial:
                    actItemFactory = new TicketMaterialActItemFactory();
                    break;

                case SysCommon.ActSubTypes.ByProductsItemsList:
                case SysCommon.ActSubTypes.TicketAddProduct:
                    actItemFactory = new TicketByProdActItemFactory();
                    break;

                case SysCommon.ActSubTypes.PurchaseOrderWorkSheet:
                    actItemFactory = new PoWorkSheetActItemFactory();
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

                case SysCommon.ActSubTypes.InventoryInquiryActivitySingle:
                    actItemFactory = new InvInquiryActItemSingleFactory();
                    break;

                case SysCommon.ActSubTypes.ConvertProduct:
                    actItemFactory = new ConvertProductActItemFactory();
                    break;

                case SysCommon.ActSubTypes.CutSheetFinishedItemsList:
                case SysCommon.ActSubTypes.CutSheetFinished:
                    actItemFactory = new CutSheetFinishedActItemFactory();
                    break;

                case SysCommon.ActSubTypes.CutSheetMaterialItemsList:
                case SysCommon.ActSubTypes.CutSheetMaterial:
                    actItemFactory = new CutSheetMaterialActItemFactory();
                    break;

                case SysCommon.ActSubTypes.YieldSheetFinishedItemsList:
                case SysCommon.ActSubTypes.YieldSheetFinished:
                    actItemFactory = new YieldSheetFinishedActItemFactory();
                    break;

                case SysCommon.ActSubTypes.YieldSheetMaterialItemsList:
                case SysCommon.ActSubTypes.YieldSheetMaterial:
                    actItemFactory = new YieldSheetMaterialActItemFactory();
                    break;
            }

            ActItem actItem;
            if (actItemFactory == null)
            {
                throw new NotImplementedException("Could not create activity item object.");
            }

            actItem = actItemFactory.GetActItem(activity);
            actItem.ActSubType = actSubType;

            return actItem;
        }
    }
}