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

namespace YAF.Web.BBCodes;

using Microsoft.AspNetCore.Mvc;

using YAF.Types.Attributes;

/// <summary>
/// The Album Image BB Code Module.
/// </summary>
public class AlbumImage : BBCodeControl
{
    /// <summary>
    /// Render The Album Image as Link with Image
    /// </summary>
    /// <param name="stringBuilder">
    /// The string Builder.
    /// </param>
    public override void Render([NotNull] StringBuilder stringBuilder)
    {
        stringBuilder.AppendFormat(
            @"<div class=""card bg-dark text-white"" style=""max-width:{0}px"">",
            this.PageContext.BoardSettings.ImageThumbnailMaxWidth);

        stringBuilder.AppendFormat(
            @"<a href=""{0}"" data-gallery=""#blueimp-gallery-{2}"" title=""{1}"">",
            this.Get<IUrlHelper>().Action("GetImage", "Albums", new { imageId = this.Parameters["inner"] }),
            this.Parameters["inner"],
            this.MessageID.Value);

        stringBuilder.AppendFormat(
            @"<img src=""{0}"" class=""img-user-posted card-img-top"" style=""max-height:{2}px"" alt=""{1}"">",
            this.Get<IUrlHelper>().Action("GetImagePreview", "Albums", new { imageId = this.Parameters["inner"] }),
            this.Parameters["inner"],
            this.PageContext.BoardSettings.ImageThumbnailMaxHeight);

        stringBuilder.Append("</a>");

        stringBuilder.Append(@"<div class=""card-body py-1"">");

        stringBuilder.Append($@"<p class=""card-text text-center small"">{this.GetText("IMAGE_RESIZE_ENLARGE")}</p>");

        stringBuilder.Append(@"</div></div>");
    }
}