using System;
using System.IO;
using UnityEngine;

public class SessionLogger
{
    private const string dateFormat = "yyyyMMdd-HHmmss";

    private readonly DateTime sessionTime;
    private readonly string sessionId;
    private readonly string filePath;

    public SessionLogger(string sessionId)
    {
        this.sessionTime = DateTime.Now;
        this.sessionId = sessionId;

        this.filePath = GetFilePath();

        File.WriteAllText(filePath, $"{sessionId}\n");
    }

    public void Log(string playerType, TrashPickerGame game)
    {
        File.AppendAllText(filePath, $"{playerType},{game.Score}\n");
    }

    private string GetFilePath()
    {
        string formattedDate = sessionTime.ToString(dateFormat);

        string fileName = String.Format("lft-{0}-{1}.csv", formattedDate, sessionId);

        string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        return Path.Combine(dir, fileName);
    }
}
