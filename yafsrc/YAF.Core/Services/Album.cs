/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2023 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services;

using System.IO;

using Model;

using Types.Models;
using Types.Objects;

using YAF.Types.Attributes;

/// <summary>
/// Album Service for the current user.
/// </summary>
public class Album : IAlbum, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Album"/> class.
    /// </summary>
    /// <param name="serviceLocator">
    /// The service locator.
    /// </param>
    public Album([NotNull] IServiceLocator serviceLocator)
    {
        this.ServiceLocator = serviceLocator;
    }

    /// <summary>
    /// Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// Deletes the specified album/image.
    /// </summary>
    /// <param name="uploadFolder">
    /// The Upload folder.
    /// </param>
    /// <param name="albumId">
    /// The album id.
    /// </param>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="imageId">
    /// The image id.
    /// </param>
    public void AlbumImageDelete(
        [NotNull] string uploadFolder,
        [CanBeNull] int? albumId,
        int userId,
        [NotNull] int? imageId)
    {
        if (albumId.HasValue)
        {
            var albums = BoardContext.Current.GetRepository<UserAlbumImage>().List(albumId.Value);

            albums.ForEach(
                dr =>
                    {
                        var fullName = $"{uploadFolder}/{userId}.{albumId}.{dr.FileName}.yafalbum";
                        var file = new FileInfo(fullName);

                        try
                        {
                            if (!file.Exists)
                            {
                                return;
                            }

                            File.SetAttributes(fullName, FileAttributes.Normal);
                            File.Delete(fullName);
                        }
                        finally
                        {
                            var imageIdDelete = dr.ID;
                            BoardContext.Current.GetRepository<UserAlbumImage>().DeleteById(imageIdDelete);
                            BoardContext.Current.GetRepository<UserAlbum>().DeleteCover(imageIdDelete);
                        }
                    });

            this.GetRepository<UserAlbumImage>().Delete(a => a.AlbumID == albumId.ToType<int>());

            this.GetRepository<UserAlbum>().Delete(a => a.ID == albumId.ToType<int>());
        }
        else
        {
            var image = this.GetRepository<UserAlbumImage>().GetImage(imageId.Value);

            var fileName = image.Item1.FileName;
            var imgAlbumId = image.Item1.AlbumID.ToString();
            var fullName = $"{uploadFolder}/{userId}.{imgAlbumId}.{fileName}.yafalbum";
            var file = new FileInfo(fullName);

            try
            {
                if (!file.Exists)
                {
                    return;
                }

                File.SetAttributes(fullName, FileAttributes.Normal);
                File.Delete(fullName);
            }
            finally
            {
                this.GetRepository<UserAlbumImage>().DeleteById(imageId.Value);
                this.GetRepository<UserAlbum>().DeleteCover(imageId.Value);
            }
        }
    }

    /// <summary>
    /// The change image caption.
    /// </summary>
    /// <param name="imageId">
    /// The Image id.
    /// </param>
    /// <param name="newCaption">
    /// The New caption.
    /// </param>
    /// <returns>
    /// the return object.
    /// </returns>
    public ReturnClass ChangeImageCaption(int imageId, [NotNull] string newCaption)
    {
        // load the DB so BoardContext can work...
        CodeContracts.VerifyNotNull(newCaption);

        this.GetRepository<UserAlbumImage>().UpdateCaption(imageId, newCaption);

        var returnObject = new ReturnClass { NewTitle = newCaption };

        returnObject.NewTitle = newCaption == string.Empty
                                    ? this.Get<ILocalization>().GetText(
                                        "ALBUM",
                                        "ALBUM_IMAGE_CHANGE_CAPTION")
                                    : newCaption;
        returnObject.Id = imageId.ToString();
        return returnObject;
    }
}