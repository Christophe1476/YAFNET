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
namespace YAF.Core.Model;

using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;

using YAF.Types.Attributes;
using YAF.Types.Models;
using YAF.Types.Objects.Model;

/// <summary>
/// The Buddy repository extensions.
/// </summary>
public static class BuddyRepositoryExtensions
{
    /// <summary>
    /// Adds a buddy request. (Should be approved later by "ToUserID")
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from User Id.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    /// <returns>
    /// The name of the second user + Whether this request is approved or not.
    /// </returns>
    public static bool AddRequest(
        this IRepository<Buddy> repository,
        [NotNull] int fromUserId,
        [NotNull] int toUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        if (repository.Exists(x => x.FromUserID == fromUserId && x.ToUserID == toUserId))
        {
            return false;
        }

        if (repository.Exists(x => x.FromUserID == toUserId && x.ToUserID == fromUserId))
        {
            repository.Insert(
                new Buddy
                    {
                        FromUserID = fromUserId, ToUserID = toUserId, Approved = true, Requested = DateTime.UtcNow
                    });

            repository.UpdateOnly(
                () => new Buddy { Approved = true },
                b => b.FromUserID == toUserId && b.ToUserID == fromUserId);

            return true;
        }

        repository.Insert(
            new Buddy
                {
                    FromUserID = fromUserId,
                    ToUserID = toUserId,
                    Approved = false,
                    Requested = DateTime.UtcNow
                });

        return false;
    }

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from User Id.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    /// <param name="mutual">
    /// Should the requesting user (ToUserID) be added to FromUserID's buddy list too?
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    public static bool ApproveRequest(
        this IRepository<Buddy> repository,
        [NotNull] int fromUserId,
        [NotNull] int toUserId,
        [NotNull] bool mutual)
    {
        CodeContracts.VerifyNotNull(repository);

        if (!repository.Exists(x => x.FromUserID == fromUserId && x.ToUserID == toUserId))
        {
            return false;
        }

        repository.UpdateOnly(
            () => new Buddy { Approved = true },
            b => b.FromUserID == fromUserId && b.ToUserID == toUserId);

        if (!mutual)
        {
            return true;
        }

        if (!repository.Exists(x => x.FromUserID == toUserId && x.ToUserID == fromUserId))
        {
            repository.Insert(
                new Buddy
                    {
                        FromUserID = toUserId, ToUserID = fromUserId, Approved = true, Requested = DateTime.UtcNow
                    });
        }

        return true;
    }

    /// <summary>
    /// Denies a friend request.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from User Id.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    public static void DenyRequest(
        this IRepository<Buddy> repository,
        [NotNull] int fromUserId,
        [NotNull] int toUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        repository.Delete(b => b.FromUserID == fromUserId && b.ToUserID == toUserId);
    }

    /// <summary>
    /// removes a friend request
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="toUserId">
    /// The to User Id.
    /// </param>
    public static void RemoveRequest(
        this IRepository<Buddy> repository,
        [NotNull] int toUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        repository.Delete(b => b.FromUserID == BoardContext.Current.PageUserID && b.ToUserID == toUserId);
    }

    /// <summary>
    /// Removes the "ToUserID" from "FromUserID"'s buddy list.
    /// </summary>
    /// <param name="repository">
    /// The repository.
    /// </param>
    /// <param name="fromUserId">
    /// The from user id.
    /// </param>
    /// <param name="toUserId">
    /// The to user id.
    /// </param>
    public static void Remove(
        this IRepository<Buddy> repository,
        [NotNull] int fromUserId,
        [NotNull] int toUserId)
    {
        CodeContracts.VerifyNotNull(repository);

        repository.Delete(x => x.FromUserID == fromUserId && x.ToUserID == toUserId);

        repository.FireDeleted();
    }

    /// <summary>
    /// Gets all the friends of a certain user.
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">From user identifier.</param>
    /// <returns>
    /// The containing the friend list.
    /// </returns>
    public static List<BuddyUser> GetAllFriends(this IRepository<Buddy> repository, [NotNull] int userId)
    {
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

        // Get from user friends
        expression.Join<User>((b, u) => u.ID == b.ToUserID && (u.Flags & 2) == 2 && (u.Flags & 32) != 32)
            .Where<Buddy>(b => b.FromUserID == userId && b.Approved == true)
            .Select<Buddy, User>((b, u) => new
                                        {
                                            UserID = u.ID,
                                            u.Name,
                                            u.DisplayName,
                                            u.Joined,
                                            u.NumPosts,
                                            b.Approved,
                                            b.Requested,
                                            u.UserStyle,
                                            u.Suspended,
                                            u.Avatar,
                                            u.AvatarImage
            });

        var expression2 = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

        // Get from user friends
        expression2.Join<User>((b, u) => u.ID == b.FromUserID && (u.Flags & 2) == 2 && (u.Flags & 32) != 32)
            .Where<Buddy>(b => b.ToUserID == userId && b.Approved == true)
            .Select<Buddy, User>((b, u) => new
                                               {
                                                   UserID = u.ID,
                                                   u.Name,
                                                   u.DisplayName,
                                                   u.Joined,
                                                   u.NumPosts,
                                                   b.Approved,
                                                   b.Requested,
                                                   u.UserStyle,
                                                   u.Suspended,
                                                   u.Avatar,
                                                   u.AvatarImage
                                               });

        return repository.DbAccess.Execute(
                db => db.Connection.Select<BuddyUser>(
                    $"{expression.ToMergedParamsSelectStatement()} UNION ALL {expression2.ToMergedParamsSelectStatement()}"))
            .DistinctBy(x => x.UserID).OrderBy(x => x.Name).ToList();
    }

    /// <summary>
    /// Gets all pending Received Requests
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">From user identifier.</param>
    /// <returns>
    /// Returns all pending Received Requests
    /// </returns>
    public static List<BuddyUser> GetReceivedRequests(this IRepository<Buddy> repository, [NotNull] int userId)
    {
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

        expression.Join<User>((b, u) => u.ID == b.FromUserID && (u.Flags & 2) == 2 && (u.Flags & 32) != 32)
            .Where<Buddy>(b => b.ToUserID == userId && b.Approved == false)
            .Select<Buddy, User>((b, u) => new
            {
                UserID = u.ID,
                u.Name,
                u.DisplayName,
                u.Joined,
                u.NumPosts,
                b.Approved,
                b.Requested,
                u.UserStyle,
                u.Suspended,
                u.Avatar,
                u.AvatarImage
            });

        return repository.DbAccess.Execute(db => db.Connection.Select<BuddyUser>(expression)).DistinctBy(x => x.UserID)
            .OrderBy(x => x.Name).ToList();
    }

    /// <summary>
    /// Gets all pending Send Requests
    /// </summary>
    /// <param name="repository">The repository.</param>
    /// <param name="userId">From user identifier.</param>
    /// <returns>
    /// Returns all pending Send Requests
    /// </returns>
    public static List<BuddyUser> GetSendRequests(this IRepository<Buddy> repository, [NotNull] int userId)
    {
        CodeContracts.VerifyNotNull(repository);

        var expression = OrmLiteConfig.DialectProvider.SqlExpression<Buddy>();

        expression.Join<User>((b, u) => u.ID == b.ToUserID && (u.Flags & 2) == 2 && (u.Flags & 32) != 32)
            .Where<Buddy>(b => b.FromUserID == userId && b.Approved == false)
            .Select<Buddy, User>((b, u) => new
                                               {
                                                   UserID = u.ID,
                                                   u.Name,
                                                   u.DisplayName,
                                                   u.Joined,
                                                   u.NumPosts,
                                                   b.Approved,
                                                   b.Requested,
                                                   u.UserStyle,
                                                   u.Suspended,
                                                   u.Avatar,
                                                   u.AvatarImage
                                               });

        return repository.DbAccess.Execute(db => db.Connection.Select<BuddyUser>(expression)).DistinctBy(x => x.UserID)
            .OrderBy(x => x.Name).ToList();
    }
}