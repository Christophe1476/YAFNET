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

namespace YAF.Core.Context;

using System;

using Microsoft.Extensions.Logging;

using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The load page from database.
/// </summary>
[ExportService(ServiceLifetimeScope.InstancePerContext, null, typeof(IHandleEvent<InitPageLoadEvent>))]
public class LoadPageFromDatabase : IHandleEvent<InitPageLoadEvent>, IHaveServiceLocator
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadPageFromDatabase"/> class.
    /// </summary>
    /// <param name="serviceLocator">The service locator.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="dataCache">The data cache.</param>
    public LoadPageFromDatabase(
        [NotNull] IServiceLocator serviceLocator, ILogger<LoadPageFromDatabase> logger, [NotNull] IDataCache dataCache)
    {
        this.ServiceLocator = serviceLocator;
        this.Logger = logger;
        this.DataCache = dataCache;
    }

    /// <summary>
    /// Gets or sets the logger.
    /// </summary>
    /// <value>
    /// The logger.
    /// </value>
    public ILogger Logger { get; set; }

    /// <summary>
    /// Gets or sets DataCache.
    /// </summary>
    public IDataCache DataCache { get; set; }

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order => 1000;

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator { get; set; }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] InitPageLoadEvent @event)
    {
        try
        {
            string userKey = null;

            var context = this.Get<IHttpContextAccessor>().HttpContext;

            if (BoardContext.Current.MembershipUser != null)
            {
                userKey = BoardContext.Current.MembershipUser.Id;
            }

            var tries = 0;
            Tuple<PageLoad, User, Category, Forum, Topic, Message> pageRow;

            var forumPage = BoardContext.Current.CurrentForumPage != null
                                ? BoardContext.Current.CurrentForumPage.PageName.ToString()
                                : string.Empty;
                
            var location = context.Request.Query.GetFirstOrDefaultAs<string>("u");

            if (location.IsNotSet())
            {
                location = string.Empty;
            }

            do
            {
                pageRow = this.Get<DataBroker>().GetPageLoad(
                    context.Session.Id,
                    BoardContext.Current.PageBoardID,
                    userKey,
                    context.GetUserRealIPAddress(),
                    location,
                    forumPage,
                    @event.UserRequestData.Browser,
                    @event.UserRequestData.Platform,
                    @event.PageQueryData.CategoryID,
                    @event.PageQueryData.ForumID,
                    @event.PageQueryData.TopicID,
                    @event.PageQueryData.MessageID,
                    @event.UserRequestData.IsSearchEngine,
                    @event.UserRequestData.DontTrack);

                // if the user doesn't exist create the user...
                if (userKey != null && pageRow == null && !this.Get<IAspNetRolesHelper>().DidCreateForumUser(
                        BoardContext.Current.MembershipUser,
                        BoardContext.Current.PageBoardID))
                {
                    throw new ApplicationException("Failed to create new user.");
                }

                if (pageRow is not null && pageRow.Item1 == null)
                {
                    if (pageRow.Item2 != null)
                    {
                        // FIX user roles ?!
                        this.Get<IAspNetRolesHelper>().SetupUserRoles(
                            BoardContext.Current.PageBoardID,
                            BoardContext.Current.MembershipUser);

                        this.Get<IAspNetRolesHelper>().DidCreateForumUser(
                            BoardContext.Current.MembershipUser,
                            BoardContext.Current.PageBoardID);
                    }

                    pageRow = null;
                }

                if (tries++ < 2)
                {
                    continue;
                }

                if (userKey != null && pageRow == null)
                {
                    // probably no permissions, use guest user instead...
                    userKey = null;
                    continue;
                }

                // fail...
                break;
            }
            while (pageRow == null && userKey != null);

            // add all loaded page data into our data dictionary...
            @event.PageLoadData = pageRow ?? throw new ApplicationException("Unable to find the Guest User!");

            // update Query Data
            @event.PageQueryData.CategoryID = pageRow.Item3?.ID ?? 0;

            // add all loaded page data into our data dictionary...
            if (pageRow.Item4 != null)
            {
                @event.PageQueryData.ForumID = pageRow.Item4.ID;
            }

            if (pageRow.Item5 != null)
            {
                @event.PageQueryData.TopicID = pageRow.Item5.ID;
            }

            // clear active users list
            if (@event.PageLoadData.Item1.ActiveUpdate)
            {
                // purge the cache if something has changed...
                this.DataCache.Remove(Constants.Cache.UsersOnlineStatus);
            }
        }

#if !DEBUG
            catch (Exception x)
            {
                // log the exception...
                this.Logger.Error(
                    x,
                    $"Failure Initializing User/Page (URL: {this.Get<IHttpContextAccessor>().HttpContext.Request.Path}).");

                // log the user out...
                // FormsAuthentication.SignOut();
                if (BoardContext.Current.CurrentForumPage.PageName != ForumPages.Info)
                {
                    // show a failure notice since something is probably up with membership...
                    this.Get<LinkBuilder>().RedirectInfoPage(InfoMessage.Failure);
                }
                else
                {
                    // totally failing... just re-throw the exception...
                    throw;
                }
#else
        catch (Exception)
        {
            // re-throw exception...
            throw;
#endif
        }
    }
}