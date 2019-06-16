/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Booting;

namespace Dolittle.TimeSeries.Modules.IoTEdge.Booting
{
    /// <summary>
    /// Represents the booting sequence for Edge modules
    /// </summary>
    public class Bootloader
    {
        Dolittle.Booting.Bootloader _actualBootloader;

        /// <summary>
        /// Initializes a new instance of <see cref="Bootloader"/>
        /// </summary>
        /// <param name="actualBootloader">The underlying nested <see cref="Dolittle.Booting.Bootloader"/></param>
        public Bootloader(Dolittle.Booting.Bootloader actualBootloader)
        {
            _actualBootloader = actualBootloader;
        }


        /// <summary>
        /// Configure the bootloader
        /// </summary>
        /// <param name="builderDelegate"></param>
        /// <returns></returns>
        public static Bootloader Configure(Action<IBootBuilder> builderDelegate)
        {
            var actualBootloader = Dolittle.Booting.Bootloader.Configure(builderDelegate);
            var bootloader = new Bootloader(actualBootloader);
            return bootloader;
        }       

        /// <summary>
        /// Start the boot sequence
        /// </summary>
        /// <returns></returns>
        public async Task Start()
        {
            _actualBootloader.Start();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            await WhenCancelled(cts.Token);
        }

        Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>) s).SetResult(true), tcs);
            return tcs.Task;
        }

    }
}