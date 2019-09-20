namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using dMobile.Bedrock;

  public class TableActivityConvertUnit : TableBase
  {
    #region Constructors

    public TableActivityConvertUnit()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityConvertUnit";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnits), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityConvertUnit " 
              + "WHERE tActivityConvertUnit.ActivitySubType = " + (int)SysCommon.ActSubTypes.ConvertUnit;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivity " 
              + "WHERE tActivity.ActivitySubType = " + (int)SysCommon.ActSubTypes.ConvertUnit;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnitsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingConvertUnitsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging convert units failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingConvertUnitsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging convert units failed.", ex);
#endif
      }
    }

    #endregion
  }
}
