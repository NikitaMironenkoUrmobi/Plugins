using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Redpenguin.GoogleSheets
{
  public class GoogleSheetsReader
  {
    private static readonly string FilePath = $"{Application.dataPath}/";
    private static readonly string[] Scopes = { SheetsService.Scope.Spreadsheets };
    private static readonly string ApplicationName = PlayerSettings.productName;
    //private static readonly string ApplicationName = "google-sheets-api";

    //private static string SpredsheetId = "1c30Wf6s0f4iNPDB38YDhfGvTM2lXcsLp-pfm4rfrcIA";
    private static string SpredsheetId = "";
    private static GoogleSheetsSettings googleSheetsSettings;

    private static SheetsService service;

    public static IList<IList<object>> GetRows<T>(T readerParams = null) where T : ReaderParams, new()
    {
      GoogleSheetsSettingsInit();
      GoogleCredential credential;
      //using (var stream = new FileStream(FilePath + "client_secrets.json", FileMode.Open, FileAccess.Read))
      //{
      //  credential = GoogleCredential.FromStream(stream).CreateScoped(Scopes);
      //}
      credential = GoogleCredential.FromJson(googleSheetsSettings.clientSecrets.text).CreateScoped(Scopes);
      service = new SheetsService(new BaseClientService.Initializer()
      {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName
      });
      readerParams ??= new T();
      return ReadEntries(readerParams);
    }


    private static IList<IList<object>> ReadEntries<T>(T value) where T : ReaderParams
    {
      var range = value.Range;
      var request = service.Spreadsheets.Values.Get(SpredsheetId, range);
      var response = request.Execute();
      var values = response.Values;

      if (values != null)
      {
        if (values.Count > 0)
        {
          return values;
        }
      }
      return null;
    }

    private static void GoogleSheetsSettingsInit()
    {
      if (googleSheetsSettings != null) return;
      googleSheetsSettings = Resources.Load<GoogleSheetsSettings>("GoogleSheets");
      if (googleSheetsSettings != null)
      {
        SpredsheetId = googleSheetsSettings.sheetID;
        Debug.Log($"SpredsheetId is set {SpredsheetId}");
      }
      else
      {
        Debug.Log("Cant find GoogleSheetsSettings");
      }
     }
  }
}