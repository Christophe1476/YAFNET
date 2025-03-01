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
using YAF.Core.Model;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Flags;
using YAF.Types.Models;

/// <summary>
/// Admin Page for Editing or Creating an Forum Access Mask
/// </summary>
public class EditAccessMaskModel : AdminPage
{
    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EditAccessMaskModel"/> class. 
    /// </summary>
    public EditAccessMaskModel()
        : base("ADMIN_EDITACCESSMASKS", ForumPages.Admin_EditAccessMask)
    {
    }

    /// <summary>
    /// Creates navigation page links on top of forum (breadcrumbs).
    /// </summary>
    public override void CreatePageLinks()
    {
        // administration index
        this.PageBoardContext.PageLinks.AddAdminIndex();

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("ADMIN_ACCESSMASKS", "TITLE"),
            this.Get<LinkBuilder>().GetLink(ForumPages.Admin_AccessMasks));

        // current page label (no link)
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ADMIN_EDITACCESSMASKS", "TITLE"), string.Empty);
    }

    /// <summary>
    /// Handles the Load event of the Page control.
    /// </summary>
    public  void OnGet(int? i)
    {
        this.Input = new InputModel();
        this.BindData(i);
    }

    /// <summary>
    /// Saves The Access Mask
    /// </summary>
    public IActionResult OnPostSave()
    {
        var flags = new AccessFlags
                    {
                        ReadAccess = this.Input.ReadAccess,
                        PostAccess = this.Input.PostAccess,
                        ReplyAccess = this.Input.ReplyAccess,
                        PriorityAccess = this.Input.PriorityAccess,
                        PollAccess = this.Input.PollAccess,
                        VoteAccess = this.Input.VoteAccess,
                        ModeratorAccess = this.Input.ModeratorAccess,
                        EditAccess = this.Input.EditAccess,
                        DeleteAccess = this.Input.DeleteAccess
                    };

        // save it
        this.GetRepository<AccessMask>().Save(this.Input.Id, this.Input.Name, flags, this.Input.SortOrder);

        // get back to access masks administration
        return this.Get<LinkBuilder>().Redirect(ForumPages.Admin_AccessMasks);
    }

    /// <summary>
    /// Binds the data.
    /// </summary>
    private void BindData(int? i)
    {
        if (i.HasValue)
        {
            var accessMask = this.GetRepository<AccessMask>()
                .GetById(i.Value);

            if (accessMask != null)
            {
                this.Input.Id = accessMask.ID;

                // get access mask properties
                this.Input.Name = accessMask.Name;
                this.Input.SortOrder = accessMask.SortOrder;

                // get flags
                this.Input.ReadAccess = accessMask.AccessFlags.ReadAccess;
                this.Input.PostAccess = accessMask.AccessFlags.PostAccess;
                this.Input.ReplyAccess = accessMask.AccessFlags.ReplyAccess;
                this.Input.PriorityAccess = accessMask.AccessFlags.PriorityAccess;
                this.Input.PollAccess = accessMask.AccessFlags.PollAccess;
                this.Input.VoteAccess = accessMask.AccessFlags.VoteAccess;
                this.Input.ModeratorAccess = accessMask.AccessFlags.ModeratorAccess;
                this.Input.EditAccess = accessMask.AccessFlags.EditAccess;
                this.Input.DeleteAccess = accessMask.AccessFlags.DeleteAccess;
            }
            else
            {
                this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
            }
        }
        else
        {
            this.Input.SortOrder =
                (this.GetRepository<AccessMask>().Count(x => x.BoardID == this.PageBoardContext.PageBoardID) + 1)
                .ToType<short>();
        }
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public short SortOrder { get; set; }

        public bool ReadAccess { get; set; }

        public bool PostAccess { get; set; }

        public bool ReplyAccess { get; set; }

        public bool PriorityAccess { get; set; }

        public bool PollAccess { get; set; }

        public bool VoteAccess { get; set; }

        public bool ModeratorAccess { get; set; }

        public bool EditAccess { get; set; }

        public bool DeleteAccess { get; set; }
    }
}