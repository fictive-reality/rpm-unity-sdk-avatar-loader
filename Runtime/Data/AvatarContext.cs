﻿using ReadyPlayerMe.Core;

namespace ReadyPlayerMe.AvatarLoader
{
    /// <summary>
    /// This class is used as a container to store all information about the avatar.
    /// </summary>
    public class AvatarContext : Context
    {
        public bool AvatarCachingEnabled;
        public AvatarConfig AvatarConfig;
        public AvatarUri AvatarUri;
        public byte[] Bytes;
        public object Data;
        public AvatarMetadata Metadata;
        public string ParametersHash;
        public AvatarRenderSettings RenderSettings;
        public bool SaveInProjectFolder;
        public string Url;
        public bool IsUpdated;
    }
}
