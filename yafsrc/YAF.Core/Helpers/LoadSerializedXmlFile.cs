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

namespace YAF.Core.Helpers;

using System;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

/// <summary>
/// The load serialized xml file.
/// </summary>
/// <typeparam name="T">
/// </typeparam>
public class LoadSerializedXmlFile<T>
    where T : class
{
    private readonly object lockObj = new();

    /// <summary>
    /// The attempt load file.
    /// </summary>
    /// <param name="xmlFileName">
    /// The File Name. 
    /// </param>
    /// <param name="cacheName">
    /// The cache Name. 
    /// </param>
    /// <param name="transformResource">
    /// The transform Resource.
    /// </param>
    /// <returns>
    /// The <see cref="T"/>.
    /// </returns>
    public T FromFile(string xmlFileName, string cacheName, Action<T> transformResource = null)
    {
        if (MemoryCache.Default.Get(cacheName) is T file)
        {
            return file;
        }

        if (!xmlFileName.IsSet() || !File.Exists(xmlFileName))
        {
            return null;
        }

        lock (this.lockObj)
        {
            var serializer = new XmlSerializer(typeof(T));
            var sourceEncoding = GetEncodingForXmlFile(xmlFileName);

            using var sourceReader = new StreamReader(xmlFileName, sourceEncoding);
            var resources = (T)serializer.Deserialize(sourceReader);

            transformResource?.Invoke(resources);

            if (!cacheName.IsSet())
            {
                return resources;
            }

            var item = new CacheItem(cacheName) { Value = resources, RegionName = xmlFileName };

            var cacheItemPolicy = new CacheItemPolicy
                                      {
                                          AbsoluteExpiration = DateTime.UtcNow.AddHours(1.0),
                                          SlidingExpiration = TimeSpan.Zero,
                                          Priority = CacheItemPriority.Default
                                      };

            MemoryCache.Default.Add(item, cacheItemPolicy);

            return resources;
        }
    }

    /// <summary>
    /// The get encoding for xml file.
    /// </summary>
    /// <param name="xmlFileName">
    /// The xml file name. 
    /// </param>
    /// <returns>
    /// The <see cref="Encoding"/>.
    /// </returns>
    private static Encoding GetEncodingForXmlFile(string xmlFileName)
    {
        var doc = new XmlDocument();

        doc.Load(xmlFileName);

        // The first child of a standard XML document is the XML declaration.
        // The following code assumes and reads the first child as the XmlDeclaration.
        if (doc.FirstChild.NodeType != XmlNodeType.XmlDeclaration)
        {
            return Encoding.UTF8;
        }

        // Get the encoding declaration.
        var decl = (XmlDeclaration)doc.FirstChild;
        try
        {
            var currentEncoding = Encoding.GetEncoding(decl.Encoding);
            return currentEncoding;
        }
        catch
        {
            // use default...
            return Encoding.UTF8;
        }
    }
}