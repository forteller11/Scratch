using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fort.Tools.ReferenceFinder
{
    public class GuidReferenceFinderEditor : EditorWindow
    {
        [SerializeField] GuidReferenceFinder ReferenceFinder;
        
        public string FolderPathToLookInAbsolute = String.Empty;
        
        public string GuidToFind = String.Empty;
        public Object AssetToFind = null;

        public Vector2 ScrollAmount;
        
        [MenuItem("Test/Editor/GuidReferenceDependencyFinder")]
        static void Init()
        {
            var target = GetWindow<GuidReferenceFinderEditor>();
            target.Show();
        }

        private void OnEnable()
        {
            ReferenceFinder = new GuidReferenceFinder();
        }

        private void Update()
        {
            //repaint if progress is occuring
            float progress = ReferenceFinder.GetApproximateProgressNormalized();
            if (progress > 0 && progress < 1)
            {
                Repaint();
            }

        }

        private void OnGUI()
        {
            DrawFolderPath(ref FolderPathToLookInAbsolute);
            EditorGUILayout.Space();
            DrawObjectGuidLinked("Asset To Find", ref AssetToFind, "Guid To Find", ref GuidToFind);
            EditorGUILayout.Space();
            
            EditorGUILayout.Separator();
            EditorGUI.BeginChangeCheck();
            
            if (GUILayout.Button("Find Dependencies"))
            {
                ReferenceFinder.FindDependencies(FolderPathToLookInAbsolute, GuidToFind).ContinueWith(task =>
                {
                    Repaint(); //so that progress and UI is updated appropriately on finish
                }, TaskContinuationOptions.ExecuteSynchronously); //so the repaint happens on the MAIN thread and works
            }
            
            float progress = ReferenceFinder.GetApproximateProgressNormalized();
            if (progress > 0 && progress < 1)
            {
                DrawProgressBar(progress);
            }

            var dependencies = ReferenceFinder.DependencyPaths;

            EditorGUILayout.Separator();
            EditorGUILayout.LabelField($"Dependencies: {ReferenceFinder.DependencyPaths.Count}");

            ScrollAmount = EditorGUILayout.BeginScrollView(ScrollAmount, false, false);
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUILayout.Space();
                for (int i = 0; i < dependencies.Count; i++)
                {
                    string relativePath =
                        GuidReferenceFinder.GetRelativeToAssetsPath(ReferenceFinder.DependencyPaths[i]);
                    GUIContent label = new GUIContent(relativePath);
                    label.tooltip = ReferenceFinder.DependencyPaths[i];
                    label.text = relativePath;
                    EditorGUILayout.ObjectField(label, ReferenceFinder.DependencyAssets[i], typeof(Object));
                }
            }
            EditorGUILayout.EndScrollView();
        }

        public static void DrawObjectGuidLinked<T>(string assetLabel, ref T asset, string assetGuidLabel, ref string assetGuidText) where T : Object
        {
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                EditorGUI.BeginChangeCheck();
                asset = (T) EditorGUILayout.ObjectField(assetLabel, asset, typeof(T), false);
                if (EditorGUI.EndChangeCheck())
                {
                    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out assetGuidText, out long localID);
                }

                EditorGUI.BeginChangeCheck();
                assetGuidText = EditorGUILayout.TextField(assetGuidLabel, assetGuidText);
                if (EditorGUI.EndChangeCheck())
                {
                    string assetPathFromGuidString = AssetDatabase.GUIDToAssetPath(assetGuidText);
                    if (!string.IsNullOrEmpty(assetPathFromGuidString))
                    {
                        asset = AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(assetGuidText));
                    }
                    else
                    {
                        asset = null;
                    }
                }
            }
        }

        public void DrawProgressBar(float progressNormalized)
        {
            EditorGUILayout.Slider(progressNormalized, 0, 1);
        }
        
        public static void DrawFolderPath(ref string assetPathText)
        {
            EditorGUI.BeginChangeCheck();
            using (new EditorGUILayout.VerticalScope(GUI.skin.box))
            {
                assetPathText = EditorGUILayout.TextField("To Search In", assetPathText);
                
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button($"Choose Directory"))
                    {
                        assetPathText = EditorUtility.OpenFolderPanel("Choose Directory To Search Within", assetPathText, "folder");
                    }
                }
            }
        }
    }
}