/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Configuration;
using Dolittle.Configuration.Files;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Strings;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Shared;

namespace Dolittle.TimeSeries.Modules.IoTEdge
{
    /// <summary>
    /// 
    /// https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-module-twins
    /// </summary>
    public class DesiredPropertiesConfigurationObjectProvider : ICanProvideConfigurationObjects
    {
        readonly GetContainer _getContainer;
        readonly IConfigurationFileParsers _parsers;
        readonly ILogger _logger;
        Twin _twin;

        /// <summary>
        /// Initializes a new instance of <see cref="DesiredPropertiesConfigurationObjectProvider"/>
        /// </summary>
        /// <param name="getContainer"><see cref="GetContainer"/> for getting access to the IoC container</param>
        /// <param name="parsers"><see cref="IConfigurationFileParsers"/> for parsing configuration</param>
        /// <param name="logger"><see cref="ILogger"/> for logging</param>
        public DesiredPropertiesConfigurationObjectProvider(
            GetContainer getContainer,
            IConfigurationFileParsers parsers,
            ILogger logger)
        {
            _parsers = parsers;
            _logger = logger;
            _getContainer = getContainer;
        }

        /// <inheritdoc/>
        public bool CanProvide(Type type)
        {
            if (IoTEdgeHelpers.IsRunningInIotEdge()) return false;

            if( _twin == null )
            {               
                var client = _getContainer().Get<ModuleClient>();
                client.GetTwinAsync()
                    .ContinueWith(_ => _twin = _.Result)
                    .Wait();

                _logger.Information($"Desired properties : {_twin.Properties.Desired.ToJson()}");
            }

            var name = type.GetFriendlyConfigurationName().ToCamelCase();
            _logger.Information($"Ask for providing {name}");
            return _twin.Properties.Desired.Contains(name);
        }

        /// <inheritdoc/>
        public object Provide(Type type)
        {
            var name = type.GetFriendlyConfigurationName().ToCamelCase();
            var json = _twin.Properties.Desired[name].ToString();
            var instance = _parsers.Parse(type, name, json);
            if (instance != null) return instance;
            throw new UnableToProvideConfigurationObject<DesiredPropertiesConfigurationObjectProvider>(type);
        }
    }
}