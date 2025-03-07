﻿/* Yet Another Forum.NET
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

namespace YAF.Core.Controllers.Modals;

using System;

using Microsoft.Extensions.Logging;

using YAF.Core.BasePages;
using YAF.Core.Filters;
using YAF.Core.Services.Import;
using YAF.Types.Modals;
using YAF.Types.Objects;

/// <summary>
/// BBCode Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class BBCodeController : ForumBaseController
{
    /// <summary>
    /// Import
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>IActionResult.</returns>
    [ValidateAntiForgeryToken]
    [HttpPost("Import")]
    public IActionResult Import([FromForm] ImportModal model)
    {
        if (!model.Import.ContentType.StartsWith("text"))
        {
            return this.Ok(
                new MessageModalNotification(
               this.GetTextFormatted("IMPORT_FAILED", model.Import.ContentType),
                MessageTypes.danger));
        }

        try
        {
            var importedCount = DataImport.BBCodeExtensionImport(
                this.PageBoardContext.PageBoardID,
                model.Import.OpenReadStream());

            return this.Ok(
                new MessageModalNotification(
                importedCount > 0
                    ? string.Format(this.GetText("ADMIN_BANNEDIP_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_NOTHING"),
                MessageTypes.success));
        }
        catch (Exception x)
        {
            this.Get<ILogger<BBCodeController>>().Error(
                x,
                string.Format(this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_FAILED"), x.Message));

            return this.Ok(
                new MessageModalNotification(
                string.Format(this.GetText("ADMIN_BBCODE_IMPORT", "IMPORT_FAILED"), x.Message),
                MessageTypes.danger));
        }
    }
}