using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Orchestration.Editor
{
    /// <summary>
    ///     Rider移行によって起きた文字化けを修正する
    /// </summary>
    public class FixBrokenComments
    {
        [MenuItem("Tools/Fix Corrupted Comments")]
        public static void FixComments()
        {
            string[] csFiles = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
            foreach (string filePath in csFiles)
            {
                string content = File.ReadAllText(filePath, Encoding.UTF8);
                string original = content;

                // 「//」から行末までに「�」が含まれる行のコメント部分を削除
                content = Regex.Replace(content, @"(//.*?)[�]+.*", "//");

                if (content != original)
                {
                    File.WriteAllText(filePath, content, Encoding.UTF8);
                    Debug.Log($"Fixed corrupted comment in: {filePath}");
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("修正完了：文字化けコメントを削除しました");
        }
    }
}