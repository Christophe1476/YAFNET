/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2022 Ingo Herbote
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

namespace YAF.Pages.Profile;

#region Using
using YAF.Types.Models;
#endregion

/// <summary>
/// The edit settings page
/// </summary>
public partial class EditSettings : ProfilePage
{
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EditSettings"/> class.
    /// </summary>
    public EditSettings()
        : base("EDIT_SETTINGS", ForumPages.Profile_EditSettings)
    {
    }

    #endregion

    /// <summary>
    /// Sets the current edit user.
    /// </summary>
    /// <value>The user.</value>
    public User EditBoardUser => this.PageBoardContext.PageUser;

    #region Methods

    /// <summary>
    /// Create the Page links.
    /// </summary>
    protected override void CreatePageLinks()
    {
        this.PageLinks.AddRoot();
        this.PageLinks.AddLink(this.PageBoardContext.PageUser.DisplayOrUserName(), this.Get<LinkBuilder>().GetLink(ForumPages.MyAccount));
        this.PageLinks.AddLink(this.GetText("TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
        if (this.EditBoardUser == null)
        {
            this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.ProfileSettings.User = this.EditBoardUser;
    }

    #endregion
}