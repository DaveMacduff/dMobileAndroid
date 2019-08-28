namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityItem : TableBase
  {
    #region Constructors

    public TableActivityItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityItem";
    }

    #endregion

    #region Public Members

    /// <summary>
    /// 
    /// </summary>
    /// <param name="actSubType"></param>
    public override void Purge(SysCommon.ActSubTypes actSubType)
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivityItems), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items.", true);
#endif

        using (SqlCeCommand cmndPurgeActItem = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurgeActItem.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)actSubType;
          cmndPurgeActItem.CommandType = CommandType.Text;
          cmndPurgeActItem.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivityItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivityItemsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingActivityItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging activity items failed.", ex);
#endif
      }
    }

    #endregion
  }
}
