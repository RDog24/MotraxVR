using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class FileHandler : MonoBehaviour
{
    private string filePath;
    public string exercise_scene;
    public bool clear = false;

    StreamReader reader = null;

    void Start()
    {
        // Get exercise scene name
        exercise_scene = SceneManager.GetActiveScene().name;

        // Define the file path
        filePath = Path.Combine(Application.persistentDataPath, exercise_scene + ".txt");

        if(clear == true)
        {
            ClearFile();
        }

    }

    // Method to write line without wiping file
    public void WriteLines(string lines)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(lines);
            }
            Debug.Log("Lines written to file successfully.");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }

    // Method to clear file and write line
    public void WriteLineReset(string line)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(line);
            }
            Debug.Log("Lines written to file successfully.");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to write to file: " + e.Message);
        }
    }

    // Method to read data from the file
    public string ReadFromFile()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string data = File.ReadAllText(filePath);
                Debug.Log("Data read from file successfully.");
                return data;
            }
            else
            {
                Debug.LogWarning("File does not exist.");
                return "";
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to read from file: " + e.Message);
            return "";
        }
    }

    public string GetLine()
    {
        try
        {
            if (File.Exists(filePath))
            {
                if(reader == null)
                {
                    reader = new StreamReader(filePath);
                }

                string line;

                if ((line = reader.ReadLine()) != null) {
                    return line;
                }
                
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error reading file: " + e.Message);
        }

        // Return null if the line does not exist
        return null;
    }

    // Method to reset the reader to the beginning of the file
    public void ResetReader()
    {
        try {
            if (File.Exists(filePath))
            {
                if (reader == null)
                {
                    reader = new StreamReader(filePath);
                }
                reader.BaseStream.Seek(0, SeekOrigin.Begin); // Reset the stream position
                reader.DiscardBufferedData();               // Clear any cached data
                Debug.Log("Reader reset to the beginning of the file.");
            }
            else
            {
                Debug.LogWarning("File not found: " + filePath);
            }
        }
        catch (IOException e)
        {
            Debug.LogError("Error resetting file: " + e.Message);
        }
    }

    void ClearFile()
    {
        try
        {
            File.WriteAllText(filePath, "");
            Debug.Log("File Successfully Cleared.");
        }
        catch (IOException e)
        {
            Debug.LogError("Failed to clear file: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        if (reader != null) {
            reader.Close();
            reader = null;
        }
    }

    private void OnDestroy()
    {
        if (reader != null)
        {
            reader.Close();
            reader = null;
        }
    }
}
