namespace dMobile.GroundWork
{
  using System;
  using System.Data;
  
  using dMobile.Bedrock;

  public class TableActivityMoveInventoryItem : TableBase
  {
    #region Constructors

    public TableActivityMoveInventoryItem()
      : base()
    {
      this.FullClassName = "dMobile.GroundWork.TableActivityMoveInventoryItem";
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
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryItems), true);
#else
      this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items.", true);
#endif

      try
      {
        using (SQLiteCommand cmndPurge = DatabaseConnection.GetInstance().SQLConn.CreateCommand())
        {
          cmndPurge.CommandText = "DELETE FROM tActivityItemMoveInventory " 
              + "WHERE tActivityItemMoveInventory.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.MoveInventory;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();

          cmndPurge.CommandText = "DELETE FROM tActivityItem " 
              + "WHERE tActivityItem.ActivityItemSubType = " + (int)SysCommon.ActSubTypes.MoveInventory;
          cmndPurge.CommandType = CommandType.Text;
          cmndPurge.ExecuteNonQuery();
        }
      
#if MULTILANGUAGE
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryItemsComplete), true);
#else
        this.DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items complete.", true);
#endif
      }
      catch (SqlCeException ex)
      {
#if MULTILANGUAGE
        DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurred), true);
        DataAccessStatusOutHandler(this, EventArgs.Empty, Translator.GetInstance().Translate(TranslatorVocabulary.PurgingMoveInventoryItemsFailed), true);
#else
        DataAccessStatusOutHandler(this, EventArgs.Empty, "Database exception occurred.", true);
        DataAccessStatusOutHandler(this, EventArgs.Empty, "Purging move inventory items failed.", true);
#endif

#if MULTILANGUAGE
        throw new Exception(Translator.GetInstance().Translate(TranslatorVocabulary.DatabaseExceptionOccurredPurgingMoveInventoryItemsFailed), ex);
#else
        throw new Exception("Database exception occurred. Purging move inventory items failed.", ex);
#endif
      }
    }

    #endregion
  }
}
