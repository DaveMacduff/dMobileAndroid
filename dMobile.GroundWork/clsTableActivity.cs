namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivity : TableBase
  {
    #region Constructors

    public TableActivity()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivity";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge(SysCommon.ActSubTypes actSubType)
    {
      try
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivity), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity.", true);
#endif

        using (SqlCeCommand cmndPurgeActivity = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurgeActivity.CommandText = "DELETE FROM tActivity "
              + "WHERE tActivity.ActivitySubType = " + (int)actSubType;
          cmndPurgeActivity.CommandType = CommandType.Text;
          cmndPurgeActivity.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivityComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingActivityFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging activity failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingActivityFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging activity failed.", ex);
#endif
      }
    }

    #endregion
  }
}
