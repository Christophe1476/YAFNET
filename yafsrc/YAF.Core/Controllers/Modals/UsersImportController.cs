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
/// Users Import Controller
/// Implements the <see cref="ForumBaseController" />
/// </summary>
/// <seealso cref="ForumBaseController" />
[Produces("application/json")]
[Route("api/[controller]")]
[ApiController]
[AdminAuthorization]
public class UsersImportController : ForumBaseController
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
        try
        {
            int importedCount;

            // import selected file (if it's the proper format)...
            switch (model.Import.ContentType)
            {
                case "text/xml":
                    {
                        importedCount = DataImport.ImportingUsers(model.Import.OpenReadStream(), true);
                    }

                    break;

                case "application/vnd.csv":
                case "application/vnd.ms-excel":
                case "application/csv":
                case "text/comma-separated-values":
                    {
                        importedCount = DataImport.ImportingUsers(model.Import.OpenReadStream(), false);
                    }

                    break;

                default:
                    {
                        return this.Ok(
                            new MessageModalNotification(
                                this.GetText("ADMIN_USERS_IMPORT", "IMPORT_FAILED_FORMAT"),
                                MessageTypes.danger));
                    }
            }

            return this.Ok(
                new MessageModalNotification(
               importedCount > 0
                    ? string.Format(this.GetText("ADMIN_USERS_IMPORT", "IMPORT_SUCESS"), importedCount)
                    : this.GetText("ADMIN_USERS_IMPORT", "IMPORT_NOTHING"),
                importedCount > 0 ? MessageTypes.success : MessageTypes.info));
        }
        catch (Exception x)
        {
            this.Get<ILogger<BannedEmailController>>().Error(
                x,
                string.Format(this.GetText("ADMIN_BANNEDNAME_IMPORT", "IMPORT_FAILED"), x.Message));

            return this.Ok(
                new MessageModalNotification(
               string.Format(this.GetText("ADMIN_BANNEDNAME_IMPORT", "IMPORT_FAILED"), x.Message),
                MessageTypes.danger));
        }
    }
}