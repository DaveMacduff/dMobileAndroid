namespace dMobile.BarCodeScanner
{
  using System;
  using System.IO;
  using System.Reflection;
  using System.Xml.Serialization;

  public class clsCommon
  {
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public string GetAssemblyVersion()
    {
      int major = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Major;
      int minor = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Minor;
      int build = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build;
      int revision = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;

      return String.Format("{0}.{1}.{2}.{3}", major, minor, build, revision);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="ObjectType"></typeparam>
    /// <param name="oObject"></param>
    /// <returns></returns>
    public static ObjectType CopyObject<ObjectType>(ObjectType oObject)
    {
      XmlSerializer oSerializer = null;

      // Creates the serializer
      oSerializer = new XmlSerializer(oObject.GetType());

      // Use the stream
      using (MemoryStream oStream = new MemoryStream())
      {
        // Serialize the object
        oSerializer.Serialize(oStream, oObject);

        // Set the strem position
        oStream.Position = 0;

        // Return the object
        return (ObjectType)oSerializer.Deserialize(oStream);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="original"></param>
    /// <returns></returns>
    public static T clone<T>(T original)
    {
      T tempMyClass = (T)Activator.CreateInstance(original.GetType());

      FieldInfo[] fis = original.GetType().GetFields(
          BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

      foreach (FieldInfo fi in fis)
      {
        object fieldValue = fi.GetValue(original);
        if (fi.FieldType.Namespace != original.GetType().Namespace)
        {
          fi.SetValue(tempMyClass, fieldValue);
        }
        else
        {
          fi.SetValue(tempMyClass, clone(fieldValue));
        }
      }

      return tempMyClass;
    }
  }
}
