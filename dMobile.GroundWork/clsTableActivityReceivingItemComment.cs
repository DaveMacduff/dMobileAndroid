namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  using System.Data.SQLite;
  using dMobile.Bedrock;

  public class TableActivityReceivingItemComment : TableBase
  {
    #region Constructors

    public TableActivityReceivingItemComment()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReceivingItemComment";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItemComments), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments.", true);
#endif

        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemReceivingComment";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItemCommentsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments complete.", true);
#endif
      }
      catch (SQLiteException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingReceivingItemCommentsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging receiving item comments failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingReceivingItemCommentsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging receiving item comments failed.", ex);
#endif
      }
    }

    #endregion
  }
}
