
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

namespace YAF.Pages.Admin;

using System.Collections.Generic;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Models;

/// <summary>
/// The Admin Access Masks Page.
/// </summary>
public class AccessMasksModel : AdminPage
{
    /// <summary>
    /// Gets or sets the access mask list.
    /// </summary>
    /// <value>The access mask list.</value>
    [BindProperty]
    public IList<AccessMask> List { get; set; }

    public AccessMasksModel()
        : base("ADMIN_ACCESSMASKS", ForumPages.Admin_AccessMasks)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_ACCESSMASKS", "TITLE"));
    }

    /// <summary>
    /// Format access mask setting color formatting.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Set access mask flags are rendered green if true, and if not red
    /// </returns>
    public string GetItemColor(bool enabled)
    {
        // show enabled flag red
        return enabled ? "badge bg-success mb-2" : "badge bg-danger mb-2";
    }

    /// <summary>
    /// Get a user friendly item name.
    /// </summary>
    /// <param name="enabled">
    /// The enabled.
    /// </param>
    /// <returns>
    /// Item Name.
    /// </returns>
    public string GetItemName(bool enabled)
    {
        return enabled ? this.GetText("DEFAULT", "YES") : this.GetText("DEFAULT", "NO");
    }

    public IActionResult OnPostEdit(int maskId)
    {
        // redirect to editing page
        return this.Get<LinkBuilder>().Redirect(
            ForumPages.Admin_EditAccessMask,
            new {
                    i = maskId
                });
    }

    public IActionResult OnPostDelete(int maskId)
    {
        var isInUse = this.GetRepository<ForumAccess>().Exists(x => x.AccessMaskID == maskId)
                      || this.GetRepository<UserForum>().Exists(x => x.AccessMaskID == maskId);

        // attempt to delete access masks
        if (isInUse)
        {
            // used masks cannot be deleted
            return this.PageBoardContext.Notify(
                this.GetText("ADMIN_ACCESSMASKS", "MSG_NOT_DELETE"),
                MessageTypes.warning);
        }

        this.GetRepository<AccessMask>().DeleteById(maskId);

        this.BindData();

        return this.Page();
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public void OnGet()
    {
        // bind data
        this.BindData();
    }

    /// <summary>
    /// The bind data.
    /// </summary>
    private void BindData()
    {
        // list all access masks for this board
        this.List = this.GetRepository<AccessMask>().GetByBoardId();
    }
}