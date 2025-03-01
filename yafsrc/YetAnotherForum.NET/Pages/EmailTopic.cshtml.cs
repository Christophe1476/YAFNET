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

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Pages;

using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Logging;

using MimeKit;

using YAF.Core.Extensions;
using YAF.Core.Services;
using YAF.Types.Models;

using DataType = System.ComponentModel.DataAnnotations.DataType;

/// <summary>
/// The Share Topic via email
/// </summary>
public class EmailTopicModel : ForumPage
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "EmailTopicModel" /> class.
    /// </summary>
    public EmailTopicModel()
        : base("EMAILTOPIC", ForumPages.EmailTopic)
    {
    }

    /// <summary>
    /// Gets or sets the email user.
    /// </summary>
    [BindProperty]
    public User EmailUser { get; set; }

    /// <summary>
    /// Gets or sets the input.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// The on get.
    /// </summary>
    /// <param name="t">
    /// The t.
    /// </param>
    public IActionResult OnGet(int? t)
    {
        if (!t.HasValue)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        if (this.PageBoardContext.PageTopic == null)
        {
            return this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Invalid);
        }

        this.PageBoardContext.PageLinks.AddCategory(this.PageBoardContext.PageCategory);

        this.PageBoardContext.PageLinks.AddForum(this.PageBoardContext.PageForum);

        this.PageBoardContext.PageLinks.AddLink(
            this.PageBoardContext.PageTopic.TopicName,
            this.Get<LinkBuilder>().GetTopicLink(this.PageBoardContext.PageTopicID, this.PageBoardContext.PageTopic.TopicName));

        this.PageBoardContext.PageLinks.AddLink(
            this.GetText("EMAILTOPIC", "TITLE"),
            string.Empty);

        this.Input = new InputModel();

        if (!this.PageBoardContext.ForumReadAccess || !this.PageBoardContext.BoardSettings.AllowEmailTopic)
        {
            return this.Get<LinkBuilder>().AccessDenied();
        }

        this.Input.Subject = this.PageBoardContext.PageTopic.TopicName;

        var emailTopic = new TemplateEmail("EMAILTOPIC") {
                                                             TemplateParams = {
                                                                                  ["{link}"] = this.Get<LinkBuilder>().GetAbsoluteLink(
                                                                                      ForumPages.Posts,
                                                                                      new {
                                                                                          t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName
                                                                                      }),
                                                                                  ["{user}"] = this.PageBoardContext.PageUser.DisplayOrUserName()
                                                                              }
                                                         };

        this.Input.Body = emailTopic.ProcessTemplate("EMAILTOPIC");

        return this.Page();
    }

    /// <summary>
    /// Send the Email
    /// </summary>
    public IActionResult OnPost()
    {
        try
        {
            var emailTopic = new TemplateEmail("EMAILTOPIC")
                             {
                                 TemplateParams = { ["{message}"] = this.Input.Body.Trim() }
                             };

            // send a change email message...
            emailTopic.SendEmail(MailboxAddress.Parse(this.Input.Email.Trim()), this.Input.Subject.Trim());

            return this.Get<LinkBuilder>().Redirect(
                ForumPages.Posts,
                new { t = this.PageBoardContext.PageTopicID, name = this.PageBoardContext.PageTopic.TopicName });
        }
        catch (Exception x)
        {
            this.Get<ILogger<EmailTopicModel>>().Log(this.PageBoardContext.PageUserID, this, x);
            return this.PageBoardContext.Notify(this.GetTextFormatted("failed", x.Message), MessageTypes.danger);
        }
    }

    /// <summary>
    /// The input model.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [Required]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        [Required]
        public string Body { get; set; }
    }
}