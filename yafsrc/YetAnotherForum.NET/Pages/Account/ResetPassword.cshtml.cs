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

namespace YAF.Pages.Account;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Extensions;
using YAF.Types.Interfaces.Identity;

using DataType = System.ComponentModel.DataAnnotations.DataType;

/// <summary>
/// The recover Password Page.
/// </summary>
/// <summary>
/// The login model.
/// </summary>
[AllowAnonymous]
public class ResetPasswordModel : AccountPage
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<ResetPasswordModel> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordModel"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public ResetPasswordModel(ILogger<ResetPasswordModel> logger)
        : base("ACCOUNT_RESEST_PASSWORD", ForumPages.Account_ResetPassword)
    {
        this.logger = logger;
    }

    /// <summary>
    ///   Gets a value indicating whether IsProtected.
    /// </summary>
    public override bool IsProtected => false;

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Create the Page links.
    /// </summary>
    public override void CreatePageLinks()
    {
        this.PageBoardContext.PageLinks.AddLink(this.GetText("ACCOUNT_RESEST_PASSWORD", "TITLE"));
    }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    public IActionResult OnGet(string code)
    {
        return code.IsSet() ? this.Page() : this.Get<LinkBuilder>().AccessDenied();
    }

    /// <summary>
    /// The on post async.
    /// </summary>
    /// <param name="code">
    /// The code.
    /// </param>
    /// <returns>
    /// The <see cref="Task"/>.
    /// </returns>
    public async Task<IActionResult> OnPostAsync(string code)
    {
        if (!this.ModelState.IsValid)
        {
            return this.Page();
        }

        var user = this.Get<IAspNetUsersHelper>().GetUserByEmail(this.Input.Email);

        if (user == null)
        {
            return this.PageBoardContext.Notify(this.GetText("USERNAME_FAILURE"), MessageTypes.danger);
        }

        var result = this.Get<IAspNetUsersHelper>().ResetPassword(user, HttpUtility.UrlDecode(code, Encoding.UTF8), this.Input.Password);

        if (result.Succeeded)
        {
            // Get User again to get updated Password Hash
            user = this.Get<IAspNetUsersHelper>().GetUser(user.Id);

            await this.Get<IAspNetUsersHelper>().SignInAsync(user);

            return this.Get<LinkBuilder>().Redirect(ForumPages.Index);
        }

        return this.PageBoardContext.Notify(result.Errors.FirstOrDefault()?.Description, MessageTypes.danger);
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}