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

namespace YAF.Web.ViewFeatures;

using Microsoft.AspNetCore.Mvc.ViewFeatures;

internal static class CultureSwitcherViewcomponent
{
    /// <summary>
    /// Gets <see cref="ModelExplorer"/> for named <paramref name="expression"/> in given
    /// <paramref name="viewData"/>.
    /// </summary>
    /// <param name="expression">Expression name, relative to <c>viewData.Model</c>.</param>
    /// <param name="viewData">
    /// The <see cref="ViewDataDictionary"/> that may contain the <paramref name="expression"/> value.
    /// </param>
    /// <param name="metadataProvider">The <see cref="IModelMetadataProvider"/>.</param>
    /// <returns>
    /// <see cref="ModelExplorer"/> for named <paramref name="expression"/> in given <paramref name="viewData"/>.
    /// </returns>
    public static ModelExplorer FromStringExpression(
        string expression,
        ViewDataDictionary viewData,
        IModelMetadataProvider metadataProvider)
    {
        if (viewData == null)
        {
            throw new ArgumentNullException(nameof(viewData));
        }

        var viewDataInfo = ViewDataEvaluator.Eval(viewData, expression);
        if (viewDataInfo == null)
        {
            // Try getting a property from ModelMetadata if we couldn't find an answer in ViewData
            var propertyExplorer = viewData.ModelExplorer.GetExplorerForProperty(expression);
            if (propertyExplorer != null)
            {
                return propertyExplorer;
            }
        }

        if (viewDataInfo != null)
        {
            if (viewDataInfo.Container == viewData &&
                viewDataInfo.Value == viewData.Model &&
                string.IsNullOrEmpty(expression))
            {
                // Nothing for empty expression in ViewData and ViewDataEvaluator just returned the model. Handle
                // using FromModel() for its object special case.
                return FromModel(viewData, metadataProvider);
            }

            ModelExplorer containerExplorer = viewData.ModelExplorer;
            if (viewDataInfo.Container != null)
            {
                containerExplorer = metadataProvider.GetModelExplorerForType(
                    viewDataInfo.Container.GetType(),
                    viewDataInfo.Container);
            }

            if (viewDataInfo.PropertyInfo != null)
            {
                // We've identified a property access, which provides us with accurate metadata.
                var containerMetadata = metadataProvider.GetMetadataForType(viewDataInfo.Container.GetType());
                var propertyMetadata = containerMetadata.Properties[viewDataInfo.PropertyInfo.Name];

                Func<object, object> modelAccessor = (ignore) => viewDataInfo.Value;
                return containerExplorer.GetExplorerForExpression(propertyMetadata, modelAccessor);
            }
            else if (viewDataInfo.Value != null)
            {
                // We have a value, even though we may not know where it came from.
                var valueMetadata = metadataProvider.GetMetadataForType(viewDataInfo.Value.GetType());
                return containerExplorer.GetExplorerForExpression(valueMetadata, viewDataInfo.Value);
            }
        }

        // Treat the expression as string if we don't find anything better.
        var stringMetadata = metadataProvider.GetMetadataForType(typeof(string));
        return viewData.ModelExplorer.GetExplorerForExpression(stringMetadata, modelAccessor: null);
    }

    private static ModelExplorer FromModel(
        ViewDataDictionary viewData,
        IModelMetadataProvider metadataProvider)
    {
        if (viewData == null)
        {
            throw new ArgumentNullException(nameof(viewData));
        }

        if (viewData.ModelMetadata.ModelType == typeof(object))
        {
            // Use common simple type rather than object so e.g. Editor() at least generates a TextBox.
            var model = viewData.Model == null ? null : Convert.ToString(viewData.Model, CultureInfo.CurrentCulture);
            return metadataProvider.GetModelExplorerForType(typeof(string), model);
        }
        else
        {
            return viewData.ModelExplorer;
        }
    }
}