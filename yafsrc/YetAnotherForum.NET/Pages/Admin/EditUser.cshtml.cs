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

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;
using YAF.Types.Models.Identity;

/// <summary>
/// The Admin edit user page.
/// </summary>
public class EditUserModel : AdminPage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EditUserModel"/> class. 
    /// </summary>
    public EditUserModel()
        : base("ADMIN_EDITUSER", ForumPages.Admin_EditUser)
    {
    }

    [BindProperty]
    public string LastTab { get; set; } = "View1";

    [BindProperty]
    public Tuple<User, AspNetUsers, Rank, vaccess> EditUser { get; set; }

    [BindProperty]
    public AspNetUsers EditUserAspNetUsers { get; set; }

    [BindProperty]
    public Rank EditUserRank { get; set; }

    [BindProperty]
    public vaccess EditUserVaccess { get; set; }

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_USERS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_Users));
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public IActionResult OnGet(int? u = null, string tab = null)
    {
        if (!u.HasValue)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (tab.IsSet())
        {
            LastTab = tab;
        }

        var currentUserId = u.Value;

        var editUser = this.Get<IAspNetUsersHelper>().GetBoardUser(currentUserId, includeNonApproved: true);

        this.Get<IDataCache>().Set(string.Format(Constants.Cache.EditUser, currentUserId), editUser);

        if (editUser == null)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        // do admin permission check...
        if (!this.PageBoardContext.PageUser.UserFlags.IsHostAdmin && editUser.Item1.UserFlags.IsHostAdmin)
        {
            // user is not host admin and is attempted to edit host admin account...
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.EditUser = editUser;

        // current page label (no link)
        var userName = this.HtmlEncode(editUser.Item1.DisplayOrUserName());

        var header = string.Format(this.GetText("ADMIN_EDITUSER", "TITLE"), userName);

        this.PageBoardContext.PageLinks.AddLink(header, string.Empty);

        return this.Page();
    }
}