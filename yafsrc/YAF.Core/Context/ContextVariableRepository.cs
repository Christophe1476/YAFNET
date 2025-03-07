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

namespace YAF.Core.Context;

using YAF.Configuration.Pattern;
using YAF.Types.Attributes;

/// <summary>
/// Place to put helper properties for context variables inside.
/// </summary>
public class ContextVariableRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContextVariableRepository"/> class.
    /// </summary>
    /// <param name="dictionary">
    /// The dictionary.
    /// </param>
    public ContextVariableRepository([NotNull] TypeDictionary dictionary)
    {
        this.Vars = dictionary;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this Flag set if the system should check if the user is suspended and redirect appropriately. Defaults to true.
    ///   Setting to false effectively disables suspend checking.
    /// </summary>
    public bool IsSuspendCheckEnabled
    {
        get => this.Vars.AsBoolean("IsSuspendCheckEnabled") ?? true;

        set => this.Vars["IsSuspendCheckEnabled"] = value;
    }

    /// <summary>
    ///   Gets Vars.
    /// </summary>
    protected TypeDictionary Vars { get; }
}