using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fort.Tools.ReferenceFinder
{
    [Serializable]
    public class GuidReferenceFinder
    {
        public string[] ExtensionsToCheck = new[] {
            ".prefab", 
            ".scene", 
            ".mat", 
            ".unity", //scene
            ".vfx", //requires unity SRP
            ".shadergraph", //requires unity SRP
            ".preset"
            
        };

        public List<FileInfo> ToProcess = new List<FileInfo>();

        public List<string> DependencyPaths = new List<string>();
        public List<Object> DependencyAssets = new List<Object>();

        private int _filesProcessed = 0;

        public float GetApproximateProgressNormalized()
        {
            if (ToProcess.Count == 0)
                return 1;
            
            float progressNormalized = (float) _filesProcessed / ToProcess.Count;
            return progressNormalized;
        }
        
        public async Task FindDependencies(string toLookWithinAbsolutePath, string toFindGuid)
        {
            #region reset
            _filesProcessed = 0;

            ToProcess.Clear();
            DependencyPaths.Clear();
            DependencyAssets.Clear();
            #endregion
            
            #region collect files to process
            await Task.Factory.StartNew(() =>
            {
                DepthFirstRecurse(toLookWithinAbsolutePath);
                Debug.Log(toLookWithinAbsolutePath);
                Debug.Log($"Files To Process:\n Number:{ToProcess.Count}");

            });
            #endregion
            
            #region find refs from file contents
            var depPathsSparse = new string [ToProcess.Count];

            var findDepsTask = Task.Factory.StartNew(async () =>
                {
                    Task [] fileIOTasks = new Task[ToProcess.Count];
                    for (int i = 0; i < ToProcess.Count; i++)
                    {
                        int closuredIndex = i;
                        FileInfo fileInfo = ToProcess[i];
                        var fileReadTask = File.ReadAllTextAsync(fileInfo.FullName); //make this a async stream / pooled thang' thing

                        fileIOTasks[closuredIndex] = fileReadTask.ContinueWith(task =>
                        {
                            Interlocked.Increment(ref _filesProcessed);

                            if (task.Exception != null)
                            {
                                Debug.LogError(task.Exception);
                                return;
                            }

                            if (task.Result.Contains(toFindGuid)) //replace with something faster where you know what's happening
                            {
                                depPathsSparse[closuredIndex] = fileInfo.FullName;
                            }
                        });
                    }
                    await Task.WhenAll(fileIOTasks);
                }
            );
            #endregion

            // await Task.WhenAll(fileIOTasks);
            await findDepsTask;
            _filesProcessed = ToProcess.Count;
            for (int i = 0; i < depPathsSparse.Length; i++)
            {
                if (depPathsSparse[i] != null)
                {
                    DependencyPaths.Add(depPathsSparse[i]);
                }
            }

            ConvertAbsolutePathsToObjects();
        }

        private void ConvertAbsolutePathsToObjects()
        {
            for (int i = 0; i < DependencyPaths.Count; i++)
            {
                string depPathRelativeToAssets = GetRelativeToAssetsPath(DependencyPaths[i]);
                var loadedObject = AssetDatabase.LoadAssetAtPath<Object>(depPathRelativeToAssets);
                DependencyAssets.Add(loadedObject);
            }
        }

        private void DepthFirstRecurse(string directoryPath)
        {
            //Save all children that should be guid reference searched
            DirectoryInfo directory = new DirectoryInfo(directoryPath);
            foreach (var fileInfo in directory.EnumerateFiles())
            {
                //Add to check later if of correct file type
                string fileExtension = fileInfo.Extension;
                foreach (var extensionToProcess in ExtensionsToCheck)
                {
                    if (fileExtension == extensionToProcess)
                    {
                        ToProcess.Add(fileInfo);
                    }
                }
            }

            foreach (var childDirectories in Directory.EnumerateDirectories(directoryPath))
            {
                DepthFirstRecurse(childDirectories);
            }
        }

        public static string GetAbsolutePath(string relativeToAssetsPath)
        {
            if (string.IsNullOrEmpty(relativeToAssetsPath))
            {
                Debug.LogWarning($"Could not get from {relativeToAssetsPath}, perhaps it is not an asset on disk?");
                return null;
            }

            const string ASSETS_FOLDER_NAME = "Assets";
            string assetsPathAbsolute = Application.dataPath;
            string projectPathAbsolute = assetsPathAbsolute.Substring(0, assetsPathAbsolute.Length - ASSETS_FOLDER_NAME.Length); //removes "Assets" at end of datapath
            
            string absoluteRoot = projectPathAbsolute + relativeToAssetsPath;
            return absoluteRoot;
        }

        public static string GetRelativeToAssetsPath(string absolutePath)
        {
            return GetRelativeToAssetsPath(absolutePath, Application.dataPath);
        }
        public static string GetRelativeToAssetsPath(string absolutePath, string projectPath)
        {
            string depPathRelativeToAssets = absolutePath.Remove(0, projectPath.Length - ("Assets".Length ));
            depPathRelativeToAssets = depPathRelativeToAssets.Replace('\\', '/');
            return depPathRelativeToAssets;
        }
    }
    
}