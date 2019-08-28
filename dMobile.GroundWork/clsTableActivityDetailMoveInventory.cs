namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityMoveInventoryDetail : TableBase
  {
    #region Constructors

    public TableActivityMoveInventoryDetail()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryDetail";
    }

    #endregion

    #region Public Members

    /// <summary>
    ///
    /// </summary>
    /// <remarks></remarks>
    public override void Purge()
    {
#if MULTILANGUAGE
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryDetails), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details.", true);
#endif

      try
      {
        using (SqlCeCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityDetailProductSchemeVariantXRef " 
              + "WHERE tActivityDetailProductSchemeVariantXRef.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.MoveInventory + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetailMoveInventory " 
              + "WHERE tActivityDetailMoveInventory.ActivityDetailRefno IN "
              + "(SELECT tActivityDetail.ActivityDetailRefno " 
              + "FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.MoveInventory + ")";
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityDetail " 
              + "WHERE tActivityDetail.ActivityDetailSubType = " + (int)SysCommon.ActSubTypes.MoveInventory;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }

#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryDetailsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryDetailsFailed), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory details failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingMoveInventoryDetailsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging move inventory details failed.", ex);
#endif
      }
    }

    #endregion
  }
}
