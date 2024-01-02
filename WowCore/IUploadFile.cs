
namespace WowCore
{
  public interface IUploadFile
  {
    string fileNameUpload();

    bool isNewValue();

    string localFileName();

    bool hasFileUpload();
  }
}
