using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// #define で定義されているシンボルを一覧で表示するウィンドウを管理するクラス
/// </summary>
public sealed class SymbolListWindow : EditorWindow
{
    private Vector2 mScrollPos; // スクロールの座標

    /// <summary>
    /// ウィンドウを開きます
    /// </summary>
    [MenuItem("Window/Symbol List Window")]
    private static void Open()
    {
        GetWindow<SymbolListWindow>("Symbol List");
    }

    /// <summary>
    /// GUI を表示します
    /// </summary>
    private void OnGUI()
    {
        // スクロールビューの表示を開始します
        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUILayout.Height(position.height));

        // 定義されているシンボルを取得します
        var defines = EditorUserBuildSettings.activeScriptCompilationDefines;

        // 取得したシンボルを名前順でソートします
        Array.Sort(defines);

        // 定義されているシンボルの一覧を表示します
        foreach(var define in defines) {
            EditorGUILayout.BeginHorizontal(GUILayout.Height(20));

            // Copy ボタンが押された場合
            if(GUILayout.Button("Copy", GUILayout.Width(50), GUILayout.Height(20))) {
                // クリップボードにシンボル名を登録します
                EditorGUIUtility.systemCopyBuffer = define;
            }

            // 選択可能なラベルを使用してシンボル名を表示します
            EditorGUILayout.SelectableLabel(define, GUILayout.Height(20));

            EditorGUILayout.EndHorizontal();
        }

        // スクロールビューの表示を終了します
        EditorGUILayout.EndScrollView();
    }
}