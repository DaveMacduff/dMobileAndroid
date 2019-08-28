namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityConvertUnitItem : TableBase
  {
    #region Constructors

    public TableActivityConvertUnitItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnitItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnitItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemConvertUnit " 
              + "WHERE tActivityItemConvertUnit.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.ConvertUnit;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.ConvertUnit;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnitItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnitItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert unit items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingConvertUnitItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging convert unit items failed.", ex);
#endif
      }
    }

    #endregion
  }
}
