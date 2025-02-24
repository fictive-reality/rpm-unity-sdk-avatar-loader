﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ReadyPlayerMe.Core;
using UnityEngine;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class contains a number of different extension methods.
    /// </summary>
    public static class ExtensionMethods
    {
        private const string TAG = nameof(ExtensionMethods);

        /// <summary>
        /// Implements a <see cref="CustomException" /> for the <paramref name="token" />.
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        public static void ThrowCustomExceptionIfCancellationRequested(this CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                throw new CustomException(FailureType.OperationCancelled, "Operation was cancelled");
            }
        }

        #region Coroutine Runner

        [ExecuteInEditMode]
        public class CoroutineRunner : MonoBehaviour
        {
            ~CoroutineRunner()
            {
                Destroy(gameObject);
            }
        }

        private static CoroutineRunner operation;

        private const HideFlags HIDE_FLAGS = HideFlags.DontSaveInEditor | HideFlags.HideInHierarchy |
                                             HideFlags.HideInInspector | HideFlags.NotEditable |
                                             HideFlags.DontSaveInBuild;

        public static Coroutine Run(this IEnumerator iEnumerator)
        {
            CoroutineRunner[] operations = Resources.FindObjectsOfTypeAll<CoroutineRunner>();
            if (operations.Length == 0)
            {
                operation = new GameObject("[CoroutineRunner]").AddComponent<CoroutineRunner>();
                operation.hideFlags = HIDE_FLAGS;
                operation.gameObject.hideFlags = HIDE_FLAGS;
            }
            else
            {
                operation = operations[0];
            }

            return operation.StartCoroutine(iEnumerator);
        }

        public static void Stop(this Coroutine coroutine)
        {
            if (operation != null)
            {
                operation.StopCoroutine(coroutine);
            }
        }

        #endregion

        #region Get Picker

        // All possible names of objects with head mesh
        private static readonly string[] HeadMeshNameFilter = { "Renderer_Head", "Renderer_Avatar", "Renderer_Head_Custom" };

        private const string BEARD_MESH_NAME_FILTER = "Renderer_Beard";
        private const string TEETH_MESH_NAME_FILTER = "Renderer_Teeth";

        /// <summary>
        /// This method extends <c>GameObject</c> to simplify getting the Ready Player Me avatar's <c>SkinnedMeshRenderer</c>.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject" /> to search for a <see cref="SkinnedMeshRenderer" />.</param>
        /// <param name="meshType">Determines the <see cref="MeshType" /> to search for.</param>
        /// <returns>The <see cref="SkinnedMeshRenderer" /> if found.</returns>
        public static SkinnedMeshRenderer GetMeshRenderer(this GameObject gameObject, MeshType meshType)
        {
            SkinnedMeshRenderer mesh;
            List<SkinnedMeshRenderer> children = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();

            if (children.Count == 0)
            {
                
                SDKLogger.AvatarLoaderLogger.Log(TAG, "ExtensionMethods.GetMeshRenderer: No SkinnedMeshRenderer found on the Game Object.");
                return null;
            }

            switch (meshType)
            {
                case MeshType.BeardMesh:
                    mesh = children.FirstOrDefault(child => BEARD_MESH_NAME_FILTER == child.name);
                    break;
                case MeshType.TeethMesh:
                    mesh = children.FirstOrDefault(child => TEETH_MESH_NAME_FILTER == child.name);
                    break;
                case MeshType.HeadMesh:
                    mesh = children.FirstOrDefault(child => HeadMeshNameFilter.Contains(child.name));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(meshType), meshType, null);
            }

            if (mesh != null) return mesh;

            SDKLogger.AvatarLoaderLogger.Log(TAG, $"ExtensionMethods.GetMeshRenderer: Mesh type {meshType} not found on the Game Object.");

            return null;
        }

        #endregion
    }
}
