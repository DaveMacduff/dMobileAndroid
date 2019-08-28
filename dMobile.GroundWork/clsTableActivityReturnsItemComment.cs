namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityReturnsItemComment : TableBase
  {
    #region Constructors

    public TableActivityReturnsItemComment()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityReturnsItemComment";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItemComments), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityReturnsItemComment";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItemCommentsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesReturnItemCommentsFailed), true);
#else
        DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales return item comments failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesReturnItemCommentsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales return item comments failed.", ex);
#endif
      }
    }

    #endregion
  }
}
