using System;
using System.IO;
using UnityEngine;

public class SessionLogger
{
    private const string dateFormat = "yyyyMMdd-HHmmss";

    private readonly DateTime sessionTime;

    public SessionLogger()
    {
        this.sessionTime = DateTime.Now;
    }

    public void Log(string playerType, TrashPickerGame game)
    {
        string formattedDate = sessionTime.ToString(dateFormat);

        string fileName = String.Format("{0}-{1}x{2}-{3}.csv",
            formattedDate, game.Rows, game.Cols, game.MaxTurns);

        string dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        string path = Path.Combine(dir, fileName);

        File.AppendAllText(path, $"{playerType},{game.Score}\n");
    }
}
