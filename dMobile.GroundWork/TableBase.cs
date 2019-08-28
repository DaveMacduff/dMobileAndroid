namespace dMobile.GroundWork
{
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Net.Sockets;
  using System.Xml.Linq;
  using dMobile.Bedrock;

  public abstract class TableBase : IDataAccessEventPublisher
  {
    #region Fields

    private string fullClassName = "TableBase";
    private SysEnvironment sysEnv;
    private DataAccessEventArgs dataEventArgs = new DataAccessEventArgs();

    #endregion

    #region Constructors

    /// <summary>
    /// 
    /// </summary>
    public TableBase()
    {
      this.SysEnv = SysEnvironment.GetInstance();
    }

    #endregion

    #region Delegates

    public delegate void OnDataAccessStatusOutEventHandler(
        IDataAccessEventPublisher dataAccessEventPublisher,
        DataAccessEventArgs dataAccessEventArgs);

    public delegate void OnDataAccessProgressOutEventHandler(
        IDataAccessEventPublisher dataAccessEventPublisher,
        DataAccessEventArgs dataAccessEventArgs);

    #endregion

    #region Events

    public event OnDataAccessStatusOutEventHandler OnDataAccessStatusOut;

    public event OnDataAccessProgressOutEventHandler OnDataAccessProgressOut;

    #endregion

    #region Properties

    /// <summary>
    /// 
    /// </summary>
    public DataAccessEventArgs DataEventArgs
    {
      get
      {
        return this.dataEventArgs;
      }

      set
      {
        this.dataEventArgs = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public string FullClassName
    {
      get
      {
        return this.fullClassName;
      }

      set
      {
        this.fullClassName = value;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public SysEnvironment SysEnv
    {
      get
      {
        return this.sysEnv;
      }

      set
      {
        this.sysEnv = value;
      }
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    public virtual void Request()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actRefno"></param>
    public virtual void Request(int actRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="criteria"></param>
    public virtual void RequestData(XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actRefno"></param>
    /// <param name="criteria"></param>
    public virtual void RequestData(int actRefno, XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Purge()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sectLocMapRefno"></param>
    public virtual void PurgeData(int sectLocMapRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actSubType"></param>
    public virtual void Purge(SysCommon.ActSubTypes actSubType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public virtual int DataStatus()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="statMessage"></param>
    /// <param name="overrideDisconnectedMode"></param>
    public void DataAccessStatusOutHandler(
        object sender,
        EventArgs e,
        string statMessage,
        bool overrideDisconnectedMode)
    {
      if (!this.SysEnv.SysSettings.IsDisconnectedMode || overrideDisconnectedMode)
      {
        if (this.OnDataAccessStatusOut != null)
        {
          this.DataEventArgs.StatusOutMsg = statMessage;
          this.DataEventArgs.OverrideDisconnectedMode = overrideDisconnectedMode;
          this.OnDataAccessStatusOut(this, this.DataEventArgs);
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="currentValue"></param>
    /// <param name="minValue"></param>
    /// <param name="maxValue"></param>
    public void DataAccessProgressOutHandler(
        object sender,
        EventArgs e,
        int currentValue,
        int minValue,
        int maxValue)
    {
      if (this.OnDataAccessProgressOut != null)
      {
        this.DataEventArgs.CurrentValue = currentValue;
        this.DataEventArgs.MinValue = minValue;
        this.DataEventArgs.MaxValue = maxValue;
        this.DataEventArgs.SetMinMaxValue = true;
        this.OnDataAccessProgressOut(this, this.DataEventArgs);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <param name="currentValue"></param>
    public void DataAccessProgressOutHandler(object sender, EventArgs e, int currentValue)
    {
      if (this.OnDataAccessProgressOut != null)
      {
        this.DataEventArgs.CurrentValue = currentValue;
        this.DataEventArgs.SetMinMaxValue = false;
        this.OnDataAccessProgressOut(this, this.DataEventArgs);
      }
    }

    #region IDataAccessEventPublisher Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataAccessEventListener"></param>
    public void RegisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener)
    {
      this.OnDataAccessStatusOut += new OnDataAccessStatusOutEventHandler(
          dataAccessEventListener.OnDataAccessStatusOut);
    }

    /// <summary>
    /// /
    /// </summary>
    /// <param name="dataAccessEventListener"></param>
    public void UnregisterDataAccessListenerStatusOut(IDataAccessEventListener dataAccessEventListener)
    {
      this.OnDataAccessStatusOut -= new OnDataAccessStatusOutEventHandler(dataAccessEventListener.OnDataAccessStatusOut);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataAccessEventListener"></param>
    public void RegisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener)
    {
      this.OnDataAccessProgressOut += new OnDataAccessProgressOutEventHandler(
          dataAccessEventListener.OnDataAccessProgressOut);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataAccessEventListener"></param>
    public void UnregisterDataAccessListenerProgressOut(IDataAccessEventListener dataAccessEventListener)
    {
      this.OnDataAccessProgressOut -= new OnDataAccessProgressOutEventHandler(
          dataAccessEventListener.OnDataAccessProgressOut);
    }

    #endregion

    #endregion

    #region Protected Methods

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyCode"></param>
    /// <returns></returns>
    protected virtual string SendRequest(string familyCode)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actRefno"></param>
    /// <returns></returns>
    protected virtual string Send(int actRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual string Send()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tableName"></param>
    /// <returns></returns>
    protected virtual string Send(string tableName)
    {
      return this.SendComm(this.Create());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlCriteria"></param>
    /// <returns></returns>
    protected virtual string SendRequest(XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actRefno"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    protected virtual string SendRequest(int actRefno, XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sendData"></param>
    /// <param name="familyCode"></param>
    /// <returns></returns>
    protected virtual string SendRequest(ref string sendData, string familyCode)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodLocMapType"></param>
    /// <param name="warehouseLocMapFKRefno"></param>
    protected virtual string SendRequest(int prodLocMapType, int whsLocMapFKRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodLocMapType"></param>
    /// <returns></returns>
    protected virtual string SendRequest(int prodLocMapType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestData"></param>
    protected virtual void Save(string requestData)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestData"></param>
    /// <param name="actItemAdjustmentRefno"></param>
    protected virtual void Save(string requestData, int refno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="requestData"></param>
    protected virtual void SaveData(ref string requestData)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual string Create()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlCriteria"></param>
    /// <returns></returns>
    protected virtual string CreateRequest(XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="familyCode"></param>
    /// <returns></returns>
    protected virtual string CreateRequest(string familyCode)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actFKRefno"></param>
    /// <returns></returns>
    protected virtual string Create(int actFKRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actFKRefno"></param>
    /// <param name="criteria"></param>
    /// <returns></returns>
    protected virtual string CreateRequest(int actFKRefno, XDocument criteria)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodLocMapType"></param>
    /// <param name="whsLocMapFKRefno"></param>
    /// <returns></returns>
    protected virtual string CreateRequest(int prodLocMapType, int whsLocMapFKRefno)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prodLocMapType"></param>
    /// <returns></returns>
    protected virtual string CreateRequest(int prodLocMapType)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="drSchemeVar"></param>
    /// <param name="recSchemeVarXRef"></param>
    /// <param name="actDetailRefno"></param>
    /// <param name="useFNo"></param>
    /// <returns></returns>
    protected virtual bool SaveCommonDetailVariant(
        DataRow drow,
        SqlCeDataReader drSchemeVar,
        ref SqlCeUpdatableRecord recSchemeVarXRef,
        int actDetailRefno,
        bool useFNo)
    {
      int schemeVariantNo = 0;
      bool isNeedToSave = false;

      if (useFNo)
      {
        schemeVariantNo = Convert.ToInt32(drow["f17"]);
      }
      else
      {
        schemeVariantNo = Convert.ToInt32(drow["ProductSchemeVariantNo"]);
      }

      if (0 < schemeVariantNo)
      {
        if (drSchemeVar.Seek(
            DbSeekOptions.FirstEqual,
            new object[] { schemeVariantNo }))
        {
          if (drSchemeVar.Read())
          {
            recSchemeVarXRef.SetInt32(
                (int)SysCommon.ActDetailSchemeVariantXRefFields.ActDetailRefno,
                actDetailRefno);

            recSchemeVarXRef.SetInt32(
                (int)SysCommon.ActDetailSchemeVariantXRefFields.ProdSchemeVariantRefno,
                drSchemeVar.GetInt32((int)SysCommon.ProdSchemeVariantFields.Refno));

            isNeedToSave = true;
          }
          else
          {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrschemevarreadFailed));
#else
            throw new Exception("drSchemeVar.Read() failed.");
#endif
          }
        }
        else
        {
          throw new Exception(
#if MULTILANGUAGE
            string.Format(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateProductSchemeNoInTproductschemevariant), schemeVariantNo));
#else
            string.Format("Unable to locate product scheme no {0} in tProductSchemeVariant", schemeVariantNo));
#endif
        }
      }

      return isNeedToSave;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="recExtra"></param>
    /// <param name="actDetailRefno"></param>
    /// <param name="useFNo"></param>
    protected virtual void SaveDetailExtra(
        DataRow drow,
        ref SqlCeUpdatableRecord recExtra,
        int actDetailRefno,
        bool useFNo)
    {
      recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailRefno, actDetailRefno);
      recExtra.SetInt32((int)SysCommon.ActDetailExtraFields.ActDetailSubType, (int)SysCommon.ActSubTypes.CycleCounts);

      if (useFNo)
      {
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["f19"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["f20"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["f21"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["f22"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["f23"].ToString());
      }
      else
      {
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text1, drow["ActivityDetailExtra1"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text2, drow["ActivityDetailExtra2"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text3, drow["ActivityDetailExtra3"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text4, drow["ActivityDetailExtra4"].ToString());
        recExtra.SetString((int)SysCommon.ActDetailExtraFields.Text5, drow["ActivityDetailExtra5"].ToString());
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="drLocMapSlot"></param>
    /// <param name="drLocMapSection"></param>
    /// <param name="drLocMapWhs"></param>
    /// <param name="recDetail"></param>
    /// <param name="useFNo"></param>
    protected virtual void SaveDetailLocation(
        DataRow drow,
        SqlCeDataReader drLocMapSlot,
        SqlCeDataReader drLocMapSection,
        SqlCeDataReader drLocMapWhs,
        ref SqlCeUpdatableRecord recDetail,
        bool useFNo)
    {
      int locMapRefno = 0;
      int locMapWhsFKRefno = 0;
      int locMapSectFKRefno = 0;
      int locMapSlotFKRefno = 0;

      if (useFNo)
      {
        locMapWhsFKRefno = Convert.ToInt32(drow["f07"]);
        locMapSectFKRefno = Convert.ToInt32(drow["f08"]);
        locMapSlotFKRefno = Convert.ToInt32(drow["f09"]);
      }
      else
      {
        locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
        locMapSectFKRefno = Convert.ToInt32(drow["LocationMapSectionFKRefno"]);
        locMapSlotFKRefno = Convert.ToInt32(drow["LocationMapSlotFKRefno"]);
      }

      if (locMapRefno == 0)
      {
        if (drLocMapSlot.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSlotFKRefno }))
        {
          if (drLocMapSlot.Read())
          {
            locMapRefno = Convert.ToInt32(drLocMapSlot.GetValue((int)SysCommon.LocMapSlotFields.LocMapRefno));
          }
          else
          {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmapslotreadFailed));
#else
            throw new Exception("drLocMapSlot.Read() failed.");
#endif
          }
        }
      }

      if (locMapRefno == 0)
      {
        if (drLocMapSection.Seek(DbSeekOptions.FirstEqual, new object[] { locMapSectFKRefno }))
        {
          if (drLocMapSection.Read())
          {
            locMapRefno = Convert.ToInt32(drLocMapSection.GetValue((int)SysCommon.LocMapSectFields.LocMapRefno));
          }
          else
          {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmapsectionreadFailed));
#else
            throw new Exception("drLocMapSection.Read() failed.");
#endif
          }
        }
      }

      if (locMapRefno == 0)
      {
        if (drLocMapWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
        {
          if (drLocMapWhs.Read())
          {
            locMapRefno = Convert.ToInt32(drLocMapWhs.GetValue((int)SysCommon.LocMapWhsFields.LocMapRefno));
          }
          else
          {
#if MULTILANGUAGE
            throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrlocmapwhsreadFailed));
#else
            throw new Exception("drLocMapWhs.Read() failed.");
#endif
          }
        }
      }

      if (locMapRefno == 0)
      {
        throw new Exception(
            string.Format(
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.UnableToIdentifyTheLocationMapRefno)
            + Translator.GetInstance().Translate(TranslatorVocabulary.WhsFkRefnoSectionFkRefnoSlotFkRefno),
#else
            "Unable to identify the location map refno. {0}"
            + "Whs FK Refno {1}{2}Section FK Refno {3}{4} Slot FK Refno {5}{6}",
#endif
            Environment.NewLine,
            locMapWhsFKRefno,
            Environment.NewLine,
            locMapSectFKRefno,
            Environment.NewLine,
            locMapSlotFKRefno));
      }

      recDetail.SetInt32((int)SysCommon.ActDetailFields.LocationMapRefno, locMapRefno);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="detail"></param>
    /// <param name="useFNo"></param>
    protected virtual void SaveDetail(DataRow drow, ref SqlCeUpdatableRecord detail, bool useFNo)
    {
      string emptyDate = "01-01-1900";

      detail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);

      if (useFNo)
      {
        detail.SetString((int)SysCommon.ActDetailFields.UniqueSchemeID, drow["f05"].ToString());
        detail.SetString((int)SysCommon.ActDetailFields.SchemeID, drow["f06"].ToString());
        detail.SetDecimal((int)SysCommon.ActDetailFields.Quantity, Convert.ToDecimal(drow["f02"]));
        detail.SetDecimal((int)SysCommon.ActDetailFields.BaseQuantity, Convert.ToDecimal(drow["f03"]));
        detail.SetDecimal((int)SysCommon.ActDetailFields.WeightQuantity, Convert.ToDecimal(drow["f04"]));
        detail.SetString((int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber, drow["f10"].ToString());
        detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber, drow["f11"].ToString());
        detail.SetString((int)SysCommon.ActDetailFields.LogisticUnitSkidNumber, drow["f18"].ToString());

        if (string.IsNullOrEmpty(drow["f13"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
            (int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate,
            Convert.ToDateTime(drow["f13"]));
        }

        if (string.IsNullOrEmpty(drow["f14"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate,
              Convert.ToDateTime(drow["f14"]));
        }

        if (string.IsNullOrEmpty(drow["f12"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitProductionDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitProductionDate,
              Convert.ToDateTime(drow["f12"]));
        }

        if (string.IsNullOrEmpty(drow["f15"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitDueDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitDueDate,
              Convert.ToDateTime(drow["f15"]));
        }

        if (string.IsNullOrEmpty(drow["f16"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitPackagingDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitPackagingDate,
              Convert.ToDateTime(drow["f16"]));
        }
      }
      else
      {
        detail.SetInt32((int)SysCommon.ActDetailFields.Status, 0);

        detail.SetString(
            (int)SysCommon.ActDetailFields.UniqueSchemeID,
            drow["ActivityDetailUniqueSchemeID"].ToString());

        detail.SetString(
            (int)SysCommon.ActDetailFields.SchemeID,
            drow["ActivityDetailSchemeID"].ToString());

        detail.SetDecimal(
            (int)SysCommon.ActDetailFields.Quantity,
            Convert.ToDecimal(drow["ActivityDetailQuantity"]));

        detail.SetDecimal(
            (int)SysCommon.ActDetailFields.BaseQuantity,
            Convert.ToDecimal(drow["ActivityDetailBaseQuantity"]));

        detail.SetDecimal(
            (int)SysCommon.ActDetailFields.WeightQuantity,
            Convert.ToDecimal(drow["ActivityDetailWeightQuantity"]));

        detail.SetString(
            (int)SysCommon.ActDetailFields.SingleTradeItemSerialNumber,
            drow["ActivityDetailSingleTradeItemSerialNumber"].ToString());

        detail.SetString(
            (int)SysCommon.ActDetailFields.LogisticUnitBatchLotNumber,
            drow["ActivityDetailLogisticUnitBatchLotNumber"].ToString());

        detail.SetString(
            (int)SysCommon.ActDetailFields.LogisticUnitSkidNumber,
            drow["ActivityDetailLogisticUnitSkidCode"].ToString());

        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
            (int)SysCommon.ActDetailFields.LogisticUnitBestBeforeSellByDate,
            Convert.ToDateTime(drow["ActivityDetailLogisticUnitBestBeforeSellByDate"]));
        }

        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitExpiryUseByDate"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitExpiryUseByDate,
              Convert.ToDateTime(drow["ActivityDetailLogisticUnitExpiryUseByDate"]));
        }

        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitProductionDate"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitProductionDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitProductionDate,
              Convert.ToDateTime(drow["ActivityDetailLogisticUnitProductionDate"]));
        }

        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitDueDate"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitDueDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitDueDate,
              Convert.ToDateTime(drow["ActivityDetailLogisticUnitDueDate"]));
        }

        if (string.IsNullOrEmpty(drow["ActivityDetailLogisticUnitPackagingDate"].ToString()))
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitPackagingDate,
              DateTime.Parse(emptyDate));
        }
        else
        {
          detail.SetDateTime(
              (int)SysCommon.ActDetailFields.LogisticUnitPackagingDate,
              Convert.ToDateTime(drow["ActivityDetailLogisticUnitPackagingDate"]));
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sendData"></param>
    /// <returns></returns>
    protected virtual string SendComm(string sendData)
    {
      return SysCommon.SubmitComm(sendData);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="errMsg"></param>
    /// <param name="drWhs"></param>
    /// <returns></returns>
    protected virtual int GetWhsLocMapRefnoFromDataRow(DataRow drow, ref string errMsg, SqlCeDataReader drWhs)
    {
      int locMapRefno = 0;

      int locMapWhsFKRefno = Convert.ToInt32(drow["LocationMapWarehouseFKRefno"]);
      errMsg += Environment.NewLine + "Whs " + locMapWhsFKRefno;

      if (drWhs.Seek(DbSeekOptions.FirstEqual, new object[] { locMapWhsFKRefno }))
      {
        if (drWhs.Read())
        {
          locMapRefno = drWhs.GetInt32((int)SysCommon.LocMapWhsFields.LocMapRefno);
        }
        else
        {
#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrwhsreadFailed) + Environment.NewLine + errMsg);
#else
          throw new Exception("drWhs.Read() failed." + Environment.NewLine + errMsg);
#endif
        }
      }

      if (locMapRefno == 0)
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheLocationMapRefno) + Environment.NewLine + errMsg);
#else
        throw new Exception("Unable to locate the location map refno." + Environment.NewLine + errMsg);
#endif
      }

      return locMapRefno;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="drow"></param>
    /// <param name="errMsg"></param>
    /// <param name="drProduct"></param>
    /// <param name="drUnit"></param>
    /// <param name="drProdUnit"></param>
    /// <returns></returns>
    protected virtual int GetProductUnitRefnoFromDataRow(
        DataRow drow,
        ref string errMsg,
        SqlCeDataReader drProduct,
        SqlCeDataReader drUnit,
        SqlCeDataReader drProdUnit,
        bool useFNo)
    {
      int prodRefno = 0;
      int prodFKRefno = 0;
      if (useFNo)
      {
        prodFKRefno = Convert.ToInt32(drow["f03"]);
      }
      else
      {
        prodFKRefno = Convert.ToInt32(drow["ProductFKRefno"]);
      }

      errMsg += Environment.NewLine + "Prod fk refno " + prodFKRefno;

      if (drProduct.Seek(DbSeekOptions.FirstEqual, new object[] { prodFKRefno }))
      {
        if (drProduct.Read())
        {
          prodRefno = drProduct.GetInt32((int)SysCommon.ProductFields.Refno);
        }
        else
        {
#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrproductreadFailed) + Environment.NewLine + errMsg);
#else
          throw new Exception("drProduct.Read() failed." + Environment.NewLine + errMsg);
#endif
        }
      }
      else
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductRefnoFromTheProductFkRefnoInTproduct)
#else
        throw new Exception("Unable to locate the product refno from the product fk refno in tProduct."
#endif
            + Environment.NewLine + errMsg);
      }

      string unitCode = string.Empty;
      int unitRefno = 0;
      if (useFNo)
      {
        unitCode = drow["f05"].ToString();
      }
      else
      {
        unitCode = drow["UnitCode"].ToString();
      }

      errMsg += Environment.NewLine + "Unit \'" + unitCode + "\'";

      if (drUnit.Seek(DbSeekOptions.FirstEqual, new object[] { unitCode }))
      {
        if (drUnit.Read())
        {
          unitRefno = drUnit.GetInt32((int)SysCommon.UnitFields.UnitRefno);
        }
        else
        {
#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrunitreadFailed) + Environment.NewLine + errMsg);
#else
          throw new Exception("drUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
        }
      }
      else
      {
#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheUnitRefnoFromTheUnitCodeInTunitzzz)
#else
        throw new Exception("Unable to locate the unit refno from the unit code in  tUnit."
#endif
            + Environment.NewLine + errMsg);
      }

      int prodUnitRefno = 0;
      bool prodUnitIsBC = false;
      if (useFNo)
      {
        prodUnitIsBC = Convert.ToBoolean(drow["f04"]);
      }
      else
      {
        prodUnitIsBC = Convert.ToBoolean(drow["ProductUnitIsBC"]);
      }

      errMsg += Environment.NewLine + "Is bc " + prodUnitIsBC;

      if (drProdUnit.Seek(DbSeekOptions.FirstEqual, new object[] { prodRefno, unitRefno, prodUnitIsBC }))
      {
        if (drProdUnit.Read())
        {
          prodUnitRefno = drProdUnit.GetInt32((int)SysCommon.ProdUnitFields.Refno);
        }
        else
        {
#if MULTILANGUAGE
          throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DrprodunitreadFailed) + Environment.NewLine + errMsg);
#else
          throw new Exception("drProdUnit.Read() failed." + Environment.NewLine + errMsg);
#endif
        }
      }
      else
      {
        throw new Exception(
#if MULTILANGUAGE
          Translator.GetInstance().Translate(TranslatorVocabulary.UnableToLocateTheProductUnitRefnoFromTheProductFkRefnoBcAndUnitCodeInTproductunit)
#else
          "Unable to locate the product unit refno from the product fk refno, bc and unit code in tProductUnit."
#endif
          + Environment.NewLine + errMsg);
      }

      return prodUnitRefno;
    }

    #endregion
  }
}
