using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DataController : MonoBehaviour
{
    [SerializeField] Text viewText;
    [SerializeField] Button dataButton;
    [SerializeField] InputField nameField, commentField;
    string url = "スプレッドシートのURL";
    string gasUrl = "GASのURL";
    List<string> datas = new List<string>();
    void Start()
    {
        StartCoroutine(GetData());
        dataButton.onClick.AddListener(()=> StartCoroutine(PostData()));
    }

    IEnumerator GetData()
    {
        datas.Clear();
        viewText.text = "";
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            yield return req.SendWebRequest();

            if (IsWebRequestSuccessful(req))
            {
                ParseData(req.downloadHandler.text);
                DisplayText();
            }
            else
            {
                Debug.Log("error");
            }
        }
    }

    IEnumerator PostData()
    {
        WWWForm form = new WWWForm();
        
        string nameText = nameField.text;
        string commentText = commentField.text;
        if(string.IsNullOrEmpty(nameText) || string.IsNullOrEmpty(commentText))
        {
            Debug.Log("empty!");
            yield break;
        }
        string combinedText = string.Join(",", nameText, commentText);
        form.AddField("val", combinedText);
        using (UnityWebRequest req = UnityWebRequest.Post(gasUrl, form))
        {
            yield return req.SendWebRequest();
            if (IsWebRequestSuccessful(req))
            {
                Debug.Log("success");
                ResetInputFields();
            }
            else
            {
                Debug.Log("error");
            }
        }
       StartCoroutine(GetData());
    }

    void ResetInputFields()
    {
        nameField.text = "";
        commentField.text = "";
    }

    void ParseData(string csvData)
    {
        string[] rows = csvData.Split(new []{ "\n"},System.StringSplitOptions.RemoveEmptyEntries);
        foreach(string row in rows)
        {
            string[] cells = row.Split(new[] { ',' },System.StringSplitOptions.RemoveEmptyEntries);
            foreach(string cell in cells)
            {
                string trimCell = cell.Trim('"');
                if (!string.IsNullOrEmpty(trimCell))
                {
                    datas.Add(trimCell);
                }
            }
        }
    }

    void DisplayText()
    {
        foreach(string data in datas)
        {
            viewText.text += data + "\n";
        }
    }

    bool IsWebRequestSuccessful(UnityWebRequest req)
    {
        return req.result != UnityWebRequest.Result.ProtocolError && 
               req.result != UnityWebRequest.Result.ConnectionError;
    }
}
