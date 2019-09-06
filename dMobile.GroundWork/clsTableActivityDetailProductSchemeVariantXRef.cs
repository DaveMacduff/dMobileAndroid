namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityDetailProductSchemeVariantXRef : TableBase
  {
    #region Constructors

    public TableActivityDetailProductSchemeVariantXRef()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityDetailProductSchemeVariantXRef";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantXrefDetails), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging product scheme variant xref details.", true);
#endif

        using (SQLiteCommand cmndPurgeProdSchemeVarXRef = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurgeProdSchemeVarXRef.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef";
          cmndPurgeProdSchemeVarXRef.CommandType = CommandType.Text;
          cmndPurgeProdSchemeVarXRef.ExecuteNonQuery();
        }

        this.DataAccessStatusOutHandler(
          this, 
          EventArgs.Empty, 
#if MULTILANGUAGE
          Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantXrefDetailsComplete), 
#else
          "Purging product scheme variant xref details complete.", 
#endif
          true);
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
#endif
        this.DataAccessStatusOutHandler(
            this, 
            EventArgs.Empty, 
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.PurgingProductSchemeVariantXrefDetailsFailed), 
#else
            "Purging product scheme variant xref details failed.", 
#endif
            true);

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingProductSchemeVariantXrefDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging product scheme variant xref details failed.", ex);
#endif
      }
    }

    #endregion
  }
}
