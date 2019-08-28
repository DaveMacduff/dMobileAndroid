namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityPickingItemComment : TableBase
  {
    #region Constructors

    public TableActivityPickingItemComment()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityPickingItemComment";
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
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderItemComments), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments.", true);
#endif

        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemPickingComment";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingSalesOrderItemCommentsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging sales order item comments complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgeSalesOrderItemCommentsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purge sales order item comments failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingSalesOrderItemCommentsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging sales order item comments failed.", ex);
#endif
      }
    }

    #endregion
  }
}
