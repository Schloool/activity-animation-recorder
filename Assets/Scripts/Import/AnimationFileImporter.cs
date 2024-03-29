﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimationFileImporter : AssetFileImporter<AnimationChoice.AnimationItem, AnimationList, AnimationClip>
{
    public AnimationFileImporter(Action<AnimationList> callback, string resourcesSubfolder = "Animations",
        string assetFileName = "Animation List") : base(resourcesSubfolder, assetFileName, callback)
    { }

    protected override bool FilterFileName(string fileName)
    {
        return fileName.EndsWith("fbx");
    }

    protected override void HandleFile(ModelImporter importer)
    {
        importer.importAnimation = true;
        importer.SaveAndReimport(); 
    }

    protected override AnimationChoice.AnimationItem GenerateItemFromPrefab(AnimationClip animationClip)
    {
        return new AnimationChoice.AnimationItem
        {
            name = animationClip.name,
            clips = new List<AnimationChoice.AnimationClipItem> { new AnimationChoice.AnimationClipItem()
                {
                    clip = animationClip,
                    movable = false,
                    loopable = false,
                }},
        };
    }
}