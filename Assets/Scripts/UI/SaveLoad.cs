using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveLoad : MonoBehaviour
{
    private string savePath;

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/settings.dat";
        Debug.Log("Application persistent data path: " + Application.persistentDataPath);
    }

    public void SaveSettings(GameSettings settings)
    {

        BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Create(savePath);
            formatter.Serialize(fileStream, settings);
            fileStream.Close();
            Debug.Log("Settings saved successfully.");
    }

    public GameSettings LoadSettings()
    {
        if (File.Exists(savePath))
        {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fileStream = File.Open(savePath, FileMode.Open);
                GameSettings settings = (GameSettings)formatter.Deserialize(fileStream);
                fileStream.Close();
                Debug.Log("File Loaded");
                return settings;
            }
        {
            Debug.LogWarning("Settings file does not exist at path: " + savePath);
            return new GameSettings();
        }
    }
}

