/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bj�rnar Henden
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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages.Profile;

using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.EventProxies;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Events;
using YAF.Types.Interfaces.Identity;
using YAF.Types.Models;

/// <summary>
/// User Page To Delete (deactivate) his account
/// </summary>
public class DeleteAccountModel : ProfilePage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "DeleteAccountModel" /> class.
    /// </summary>
    public DeleteAccountModel()
        : base("DELETE_ACCOUNT", ForumPages.Profile_DeleteAccount)
    {
    }

    /// <summary>
    /// Gets or sets the options.
    /// </summary>
    public string[] Options { get; set; } = { "suspend", "delete" };

    /// <summary>
    /// Gets or sets the option.
    /// </summary>
    [BindProperty]
    public string Option { get; set; } = "suspend";

    /// <summary>
    /// Creates page links for this page.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));

        this.PageBoardContext.PageLinks.AddLink(
            string.Format(this.GetText("DELETE_ACCOUNT", "TITLE"), this.PageBoardContext.BoardSettings.Name),
            string.Empty);
    }

    /// <summary>
    /// The on get.
    /// </summary>
    public IActionResult OnGet()
    {
        return this.PageBoardContext.PageUser.UserFlags.IsHostAdmin ? this.Get<LinkBuilder>().AccessDenied() : this.Page();
    }

    /// <summary>
    /// Delete or Suspend User
    /// </summary>
    public IActionResult OnPost()
    {
        switch (this.Option)
        {
            case "suspend":
            {
                // Suspend User for 30 Days
                // time until when user is suspended
                var suspend = this.Get<IDateTimeService>().GetUserDateTime(
                    DateTime.UtcNow,
                    this.PageBoardContext.TimeZoneInfoUser).AddDays(30);

                // suspend user by calling appropriate method
                this.GetRepository<User>().Suspend(
                    this.PageBoardContext.PageUserID,
                    suspend,
                    "User Suspended his own account",
                    this.PageBoardContext.PageUserID);

                var user = this.GetRepository<User>().GetById(
                    this.PageBoardContext.PageUserID);

                if (user != null)
                {
                    this.Get<ILogger<DeleteAccountModel>>().Log(
                        this.PageBoardContext.PageUserID,
                        this,
                        $"User {user.DisplayOrUserName()} Suspended his own account until: {suspend} (UTC)",
                        EventLogTypes.UserSuspended);

                    this.Get<IRaiseEvent>().Raise(new UpdateUserEvent(this.PageBoardContext.PageUserID));
                }
            }

                break;
            case "delete":
            {
                // (Soft) Delete User
                var user = this.PageBoardContext.MembershipUser;

                // Update IsApproved
                user.IsApproved = false;

                this.Get<IAspNetUsersHelper>().Update(user);

                var userFlags = this.PageBoardContext.PageUser.UserFlags;

                userFlags.IsDeleted = true;
                userFlags.IsApproved = false;

                this.GetRepository<User>().UpdateOnly(
                    () => new User { Flags = userFlags.BitValue },
                    u => u.ID == this.PageBoardContext.PageUserID);

                // delete posts...
                var messages = this.GetRepository<Message>().GetAllUserMessages(this.PageBoardContext.PageUserID);

                messages.ForEach(
                    x => this.GetRepository<Message>().Delete(
                        x.Topic.ForumID,
                        x.TopicID,
                        x,
                        true,
                        string.Empty,
                        true,
                        true));

                this.Get<ILogger<DeleteAccountModel>>().UserDeleted(
                    this.PageBoardContext.PageUserID,
                    $"User {this.PageBoardContext.PageUser.DisplayOrUserName()} Deleted his own account");
            }

                break;
        }

        return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
    }
}