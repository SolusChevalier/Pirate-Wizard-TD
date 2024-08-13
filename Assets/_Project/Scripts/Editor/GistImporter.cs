using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Gist importer.
/// http://diegogiacomelli.com.br/unitytips-gist-importer
/// </summary>
[InitializeOnLoad]
public static class GistImporter
{
    private const string GistsFolder = "Gists";
    private static readonly Regex _getGistUrlInfoRegex = new Regex("https://gist.github.com/(?<owner>.+)/(?<gistId>[a-z0-9]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _getDescriptionRegex = new Regex(@"\<title\>(?<description>.+)\</title\>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static readonly Regex _getFileUrlRegex = new Regex("href=\"(?<url>.+/raw/[a-z0-9\\./\\-]+)\"", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    [MenuItem("Tools/Import Gist")]
    private static void ImportGist()
    {
        var gistUrl = EditorGUIUtility.systemCopyBuffer;

        try
        {
            using (var client = new WebClient())
            {
                var gistPageContent = client.DownloadString(gistUrl);

                // Extract the files raw urls.
                var filesMatches = _getFileUrlRegex.Matches(gistPageContent);

                if (filesMatches.Count > 0)
                {
                    var infoMatch = _getGistUrlInfoRegex.Match(gistUrl).Groups;
                    var gistOwner = infoMatch["owner"].Value;
                    var gistId = infoMatch["gistId"].Value;
                    var rawUrls = filesMatches
                                    .OfType<Match>()
                                    .Select(m => $"https://gist.github.com{m.Groups["url"].Value}")
                                    .OrderByDescending(u => u.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                                    .ToArray();

                    var destinationFolder = Path.Combine(Application.dataPath, GistsFolder, gistOwner, $"{Path.GetFileNameWithoutExtension(rawUrls.First())} ({gistId})");

                    // Downloads and write the Gist files.
                    for (var i = 0; i < rawUrls.Length; i++)
                    {
                        var rawUrl = rawUrls[i];
                        var filename = Path.GetFileName(rawUrl);

                        EditorUtility.DisplayProgressBar("Importing Gist...", filename, i / (float)filesMatches.Count);
                        DownloadFile(client, rawUrl, destinationFolder, filename);
                    }

                    EditorUtility.ClearProgressBar();
                    WriteReadme(gistUrl, gistPageContent, destinationFolder);
                }
                else
                    Debug.LogWarning($"No files found for Gist '{gistUrl}'.");
            }
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    [MenuItem("Tools/Import Gist", true)]
    private static bool ValidateImportGist()
    {
        return _getGistUrlInfoRegex.IsMatch(EditorGUIUtility.systemCopyBuffer);
    }

    private static void DownloadFile(WebClient client, string url, string destinationFolder, string filename)
    {
        var fileContent = client.DownloadString(url);

        // Is an editor script?
        if (fileContent.IndexOf("using UnityEditor;", StringComparison.OrdinalIgnoreCase) > -1)
            destinationFolder = Path.Combine(destinationFolder, "Editor");

        Directory.CreateDirectory(destinationFolder);
        File.WriteAllText(Path.Combine(destinationFolder, filename), fileContent);
    }

    private static void WriteReadme(string gistUrl, string gistPageContent, string folder)
    {
        var description = _getDescriptionRegex.Match(gistPageContent).Groups["description"].Value;
        var readmePath = Path.Combine(folder, "readme.txt");
        File.WriteAllText(readmePath, $"{description}\n\nUrl: {gistUrl}\nDate: {DateTime.Now:dd/MM/yyyy HH:mm}\n\nImported using Gist Importer (http://diegogiacomelli.com.br/unitytips-gist-importer).");

        // Selects the readme file on project window.
        AssetDatabase.Refresh();
        var readmeAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>($"Assets{readmePath.Replace(Application.dataPath, string.Empty)}");
        Selection.activeObject = readmeAsset;
        EditorGUIUtility.PingObject(readmeAsset);
    }
}