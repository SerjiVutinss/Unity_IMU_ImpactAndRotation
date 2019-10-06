using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SaveProfile : MonoBehaviour
{
    public InputField playerName;
    public InputField playerAge;
    public InputField playerHeight;
    public InputField playerWeight;
    public InputField playerTrainingAge;

    public void SavePlayerProfile(){
        
        List<string> playerProfile = new List<string>();

        playerProfile.Add(playerName.text);
        playerProfile.Add(playerAge.text);
        playerProfile.Add(playerHeight.text);
        playerProfile.Add(playerWeight.text);
        playerProfile.Add(playerTrainingAge.text);

        var path = EditorUtility.SaveFilePanel(
            "Save File",
            "",
            playerName.text + "_" + DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss") + ".txt",
            "txt");
        
        if (path.Length != 0){

            if (playerProfile != null)
            {
                //File.WriteAllBytes(path, pngData);
                File.WriteAllLines(path, playerProfile.ToArray());
                Debug.Log("File Saved as: " + path);
            }
        }
    
    }
}
