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
namespace YAF.Core.Tasks;

using System;
using System.Threading;

using Microsoft.Extensions.Logging;

using YAF.Core.Model;
using YAF.Types.Models;

/// <summary>
/// The Update Search Index task.
/// </summary>
public class UpdateSearchIndexTask : LongBackgroundTask
{
    /// <summary>
    ///   Initializes a new instance of the <see cref = "UpdateSearchIndexTask" /> class.
    /// </summary>
    public UpdateSearchIndexTask()
    {
        // set interval values...
        this.StartDelayMs = 30000;
    }

    /// <summary>
    ///   Gets TaskName.
    /// </summary>
    public static string TaskName { get; } = "UpdateSearchIndexTask";

    /// <summary>
    /// The run once.
    /// </summary>
    public override void RunOnce()
    {
        try
        {
            Thread.BeginCriticalRegion();

            if (BoardContext.Current == null)
            {
                return;
            }

            if (!IsTimeToUpdateSearchIndex())
            {
                return;
            }

            var forums = this.GetRepository<Forum>().ListAll(BoardContext.Current.PageBoardID);

            forums.ForEach(
                forum =>
                    {
                        var messages = this.GetRepository<Message>().GetAllSearchMessagesByForum(forum.Item2.ID);

                        this.Get<ISearch>().AddSearchIndexAsync(messages).Wait();
                    });



            this.Get<ILogger<UpdateSearchIndexTask>>().Info("search index updated");
        }
        catch (Exception x)
        {
            this.Logger.Error(x, $"Error In {TaskName} Task");
        }
        finally
        {
            Thread.EndCriticalRegion();
        }
    }

    /// <summary>
    /// The is time to update search index.
    /// </summary>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    private static bool IsTimeToUpdateSearchIndex()
    {
        var boardSettings = BoardContext.Current.BoardSettings;
        var lastSend = DateTime.MinValue;
        var sendEveryXHours = boardSettings.UpdateSearchIndexEveryXHours;

        if (boardSettings.ForceUpdateSearchIndex)
        {
            boardSettings.LastSearchIndexUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            boardSettings.ForceUpdateSearchIndex = false;

            BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

            return true;
        }

        if (boardSettings.LastSearchIndexUpdated.IsSet())
        {
            try
            {
                lastSend = Convert.ToDateTime(boardSettings.LastSearchIndexUpdated, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                lastSend = DateTime.MinValue;
            }
        }

        var updateIndex = lastSend < DateTime.Now.AddHours(-sendEveryXHours)
                          && DateTime.Now < DateTime.Today.AddHours(6);

        if (!updateIndex)
        {
            return false;
        }

        boardSettings.LastSearchIndexUpdated = DateTime.Now.ToString(CultureInfo.InvariantCulture);

        BoardContext.Current.Get<BoardSettingsService>().SaveRegistry(boardSettings);

        return true;
    }
}