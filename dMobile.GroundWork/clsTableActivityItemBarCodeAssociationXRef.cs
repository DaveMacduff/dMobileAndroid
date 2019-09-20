namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using dMobile.Bedrock;

  public class TableActivityItemBarCodeAssociationXRef : TableBase
  {
    #region Constructors

    public TableActivityItemBarCodeAssociationXRef()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityItemBarCodeAssociationXRef";
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
        this.DataAccessStatusOutHandler(
            this, 
            EventArgs.Empty, 
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.PurgingItemBarCodeAssociationXrefs), 
#else
            "Purging item bar code association xrefs.", 
#endif
            true);

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemBarCodeAssociationXRef";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

        this.DataAccessStatusOutHandler(
            this,
            EventArgs.Empty,
#if MULTILANGUAGE
            Translator.GetInstance().Translate(TranslatorVocabulary.PurgingItemBarCodeAssociationXrefsComplete),
#else
            "Purging item bar code association xrefs complete.",
#endif
            true);
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingItemBarCodeAssociationXrefsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging item bar code association xrefs failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingItemBarCodeAssociationXrefsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging item bar code association xrefs failed.", ex);
#endif
      }
    }

    #endregion
  }
}
