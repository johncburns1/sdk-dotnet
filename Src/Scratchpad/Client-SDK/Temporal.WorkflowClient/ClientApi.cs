﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Temporal.Common.DataModel;
using Temporal.Common.WorkflowConfiguration;

using Temporal.Async;
using Temporal.Collections;
using Temporal.Serialization;

namespace Temporal.WorkflowClient
{
    public class ClientApi
    {
    }

    // *** !Re CancellationTokens: In the actual implementation, most APIs that take a CancellationToken will also have
    // *** some overload(s) that do not and use CancellationToken.None instead.
    // *** For now, we omit most of such overloads for brevity and include only the ones needed for samples.

    #region TemporalServiceClient

    public interface ITemporalServiceClient
    {
        #region -- Namespace settings for the client --
        // A TemporalServiceClient is OPTIONALLY "bound" to a namespace.
        // If the client is NOT bound to a namespace, then only APIs that do not require a namespace can be invoked.
        // Other APIs will result in an `InvalidOperationException`.
        // Q: Shall we always bound by default to some namespace with a default name? What would that be?
        // If the client IS bound to a namespace, but that namespace does not exist on the server or the user
        // has no appropriate permissions, then a `NeedsDesignException` will be thrown from the APIs that require the namespace.
        //
        // To bind a TemporalServiceClient to a namespace, specify the namespace in the TemporalServiceClientConfiguration passed
        // to the ctor.
        string Namespace { get; }
        #endregion -- Namespace settings for the client --


        #region -- Connection management --
        bool IsConectionInitialized { get; }

        /// <summary>
        /// Before the client can talk to the Temporal server, it must execute GetSystemInfo(..) to check server health
        /// and get server capabilities. This API will explicitly perform that. If this is not done explicitly, it will happen
        /// automatically before placing any other calls. <br />
        /// This will also validate connection to the namespace if it was set in the ctor.
        /// </summary>
        Task InitializeConnectionAsync();
        #endregion -- Connection management --


        #region -- Workflow access and control APIs --

        #region StartWorkflowAsync(..)
        // Future: Consider overloads auto-generate a random GUID-based `workflowId`.

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId, 
                                                string workflowTypeName,
                                                string taskQueue);

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                string workflowTypeName,
                                                string taskQueue,
                                                CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                string workflowTypeName,
                                                string taskQueue,
                                                IDataValue inputArgs);

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                string workflowTypeName,
                                                string taskQueue,
                                                IDataValue inputArgs,
                                                CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                string workflowTypeName,
                                                string taskQueue,
                                                IDataValue inputArgs,
                                                StartWorkflowChainConfiguration workflowConfig);

        Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                string workflowTypeName,
                                                string taskQueue,
                                                IDataValue inputArgs,
                                                StartWorkflowChainConfiguration workflowConfig,
                                                CancellationToken cancelToken);
        #endregion StartWorkflowAsync(..)

        #region StartWorkflowWithSignalAsync(..)
        // Future: Consider overloads auto-generate a random GUID-based `workflowId`.
        Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                          string workflowTypeName,
                                                          string taskQueue,
                                                          string signalName,
                                                          CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                          string workflowTypeName,
                                                          string taskQueue,
                                                          string signalName,
                                                          IDataValue signalArgs,
                                                          CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                          string workflowTypeName,
                                                          string taskQueue,
                                                          IDataValue wokflowArgs,
                                                          string signalName,
                                                          CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                          string workflowTypeName,
                                                          string taskQueue,
                                                          IDataValue wokflowArgs,
                                                          string signalName,
                                                          IDataValue signalArgs,
                                                          CancellationToken cancelToken);

        Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                          string workflowTypeName,
                                                          string taskQueue, 
                                                          IDataValue wokflowArgs,
                                                          string signalName,
                                                          IDataValue signalArgs,
                                                          StartWorkflowChainConfiguration workflowConfig,                                                                   
                                                          CancellationToken cancelToken);
        #endregion StartWorkflowWithSignalAsync(..)

        #region CreateUnboundWorkflowHandle(..)
        /// <summary>
        /// Create an unbound workflow chain Handle using the policy <see cref="WorkflowChainBindingPolicy.LatestChain" />.
        /// The handle will be bound to the most recent chain with the specified <c>workflowId</c> once the user interacts with it.
        /// </summary>
        IWorkflowChain CreateUnboundWorkflowHandle(string workflowId);

        /// <summary>
        /// Create an unbound workflow chain Handle using the specified policy (<c>bindPolicy</c>).
        /// The handle will be bound to some chain with the specified <c>workflowId</c> as defined by the specified policy.
        /// </summary>
        //IWorkflowChain CreateUnboundWorkflowHandle(string workflowId,
        //                                           WorkflowChainBindingPolicy bindPolicy);


        /// <summary>
        /// Create an unbound workflow chain Handle using the policy <see cref="WorkflowChainBindingPolicy.NewOrLatestChain" />.
        /// The handle will be bound to the most recent chain with the specified <c>workflowId</c> once the user binds the workflow.
        /// The parameters <c>workflowTypeName</c>, <c>taskQueue</c> and <c>workflowConfig</c> will only be used if and when the 
        /// specified binding policy will lead to starting a new workflow chain.
        /// </summary>
        //IWorkflowChain CreateUnboundWorkflowHandle(string workflowId,
        //                                           string workflowTypeName,
        //                                           string taskQueue,
        //                                           WorkflowChainBindingPolicy bindPolicy);        
        #endregion CreateUnboundWorkflowHandle(..)

        #region CreateUnboundWorkflowRunHandle(..)
        IWorkflowRun CreateUnboundWorkflowRunHandle(string workflowId,
                                                    string workflowRunId);
        #endregion CreateUnboundWorkflowRunHandle(..)

        #region CreateUnboundWorkflowStub<TStub>(..)
        // ! These will not be part of V-Alpha. !

        /// <summary>
        /// ! These will not be part of V-Alpha. !
        /// Create an unbound workflow chain Stub of type <c>TStub</c> using the policy
        /// <see cref="WorkflowChainBindingPolicy.LatestChain" />.
        /// The stub will be bound to the most recent chain with the specified <c>workflowId</c> once the user interacts with it.
        /// 
        /// 'TStub' must be an Interface and ANY interface can be specified.
        /// It is not required that the workflow implementation hosted in the worker implements this interface:
        ///  - The name of the workflow type is provided by this Workflow class instance.
        ///  - The names of the signals/queries will be deduced from the WorkflowXxxStub attributes.
        ///  - Parameters will be sent as specified.
        /// Mismatches will result in descriptive runtime errors (e.g. "no such signal handler").
        /// This makes it very easy to create interfaces for workflow implemented in any language, not necessarily a .NET language.
        /// Note that even for .NET-based workflows, the user may choose not to implement a client side iface by the workflow implementation.
        /// E.g., workflow handlers can take 'WorkflowContext' parameters, which are not part of the client-side iface.
        /// 
        /// NOTE: A stub returned by these methods can always be cast to 'IWorkflowChainStub'.
        /// </summary>
        TStub CreateUnboundWorkflowStub<TStub>(string workflowId);

        /// Create an unbound workflow chain Stub of type <c>TStub</c> using the policy
        /// <see cref="WorkflowChainBindingPolicy.NewOrLatestChain" />.
        /// The stub will be bound to a new OR to the most recent chain with the specified <c>workflowId</c> (as per the policy)
        /// once the user invokes the main workflow routine method. Other stub methods may only be invoked once the users has called
        /// the main routine.
        TStub CreateUnboundWorkflowStub<TStub>(string workflowId, 
                                               string workflowTypeName,
                                               string taskQueue);

        TStub CreateUnboundWorkflowStub<TStub>(string workflowId, 
                                               string workflowTypeName,
                                               string taskQueue,
                                               StartWorkflowChainConfiguration workflowConfig);

        /// <summary>
        /// Create an unbound workflow chain Stub using the specified policy (<c>bindPolicy</c>).
        /// The handle will be bound to some chain with the specified <c>workflowId</c> as defined by the specified policy.
        /// The parameters <c>workflowTypeName</c>, <c>taskQueue</c> and <c>workflowConfig</c> will only be used if and when the 
        /// specified binding policy will lead to starting a new workflow chain.
        /// </summary>
        TStub CreateUnboundWorkflowStub<TStub>(WorkflowChainBindingPolicy stubConfig,
                                               string workflowId,
                                               string workflowTypeName,
                                               string taskQueue);

        TStub CreateUnboundWorkflowStub<TStub>(WorkflowChainBindingPolicy stubConfig,
                                               string workflowId,
                                               string workflowTypeName,
                                               string taskQueue,
                                               StartWorkflowChainConfiguration workflowConfig);
        #endregion CreateUnboundWorkflowStub<TStub>(..)

        #region TryGetWorkflowAsync(..)
        /// <summary>
        /// <para>CHECK whether a specified workflow EXISTS, and if it does, return a handle to the respective workflow chain.</para>
        /// Invokes the overload <see cref="TryGetWorkflowAsync(String, String, CancellationToken)" />
        /// with <c>workflowChainId</c>=NULL.
        /// </summary>
        Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId);

        Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId,
                                                               CancellationToken cancelToken);

        /// <summary>
        /// <para>
        ///   CHECK whether a specified workflow EXISTS, and if id does, return a handle to the respective workflow chain.
        /// </para>
        /// <para>
        ///   <c>workflowId</c> and <c>workflowChainId</c> may NOT be BOTH null. However, ONE of these values MAY be null.<br/>
        ///   (<c>workflowChainId</c> is the <c>workflowRunId</c> of the first run in the chain.)
        /// </para>
        /// <para>
        ///   If <c>workflowId</c> and <c>workflowChainId</c> are BOTH NOT NULL:<br />
        ///   Search by tuple (workflowId, workflowChainId).
        ///   If not exists => Not found.
        /// </para>
        /// <para>
        ///   If <c>workflowId</c> is NULL:<br />
        ///   Search by workflowChainId. This may be a long/slow/inefficient scan.
        ///   If not exists => Not found.
        ///   If exists => <c>workflowId</c> of the created <c>IWorkflowChain</c> instance is set by the found server value.
        /// </para>
        /// <para>
        ///   If <c>workflowChainId</c> is NULL:<br />
        ///   Search by <c>workflowId</c> and select the latest (most recently started) chain with that <c>workflowId</c>.
        ///   This may be a long/slow/inefficient scan.
        ///   If not exists => Not found.
        ///   If exists => <c>workflowChainId</c> of the created <c>IWorkflowChain</c> instance is set by the found server value.
        /// </para>        
        /// </summary>
        Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId,
                                                               string workflowChainId,
                                                               CancellationToken cancelToken);
        #endregion TryGetWorkflowAsync(..)

        #region TryGetWorkflowRunAsync(..)
        /// <summary>
        /// CHECK whether a specified workflow run EXISTS, and if it does, return a handle to the respective workflow run.<br />
        /// The chain handle containing the found run can be accessed via the `GetOwnerWorkflowAsync(..)` method of the 
        /// returned `IWorkflowRun` instance.<br />
        /// See <see cref="TryGetWorkflowRunAsync(String, String, CancellationToken)" /> for more detais.
        /// </summary>
        Task<TryGetResult<IWorkflowRun>> TryGetWorkflowRunAsync(string workflowRunId,
                                                                CancellationToken cancelToken);

        /// <summary>
        /// <para>
        ///   CHECK whether a specified workflow run EXISTS, and if it does, return a handle to the respective workflow run.<br />
        ///   The chain handle containing the found run can be accessed via the `GetWorkflowChainAsync(..)` method of the 
        ///   returned `IWorkflowRun` instance.
        /// </para>
        /// <c>workflowId</c> may be null. In that case the API may require an inefficient/long DB scan. <br />
        /// <c>workflowRunId</c> may not be null.  <br />
        /// If a run with the specified <c>workflowRunId</c> exists, but the <c>workflowId</c> is not null and does not match,
        /// this API will NOT find that run. <br />
        /// </summary>        
        Task<TryGetResult<IWorkflowRun>> TryGetWorkflowRunAsync(string workflowId,
                                                                string workflowRunId,
                                                                CancellationToken cancelToken);
        #endregion TryGetWorkflowRunAsync(..)

        #endregion -- Workflow access and control APIs --

        // ----------- APIs below in this class will not be part of the initial version (V-Alpha). -----------

        #region -- Workflow listing APIs --
        /// <summary>
        /// Lists `IWorkflowChain`s (not `IWorkflowRun`s). Accepts sone soft of filter/query. May have a few overloads.
        /// Need to gain a better understanding of the underlying gRPC List/Scan APIs to design the most appropriate shape.
        /// May also have a `CountWorkflowsAsync(..)` equivalent if this makes sense from the query runtime perspective.
        /// </summary>        
        Task<IPaginatedReadOnlyCollectionPage<IWorkflowChain>> ListWorkflowsAsync(NeedsDesign oneOrMoreArgs);

        /// <summary>
        /// Lists `IWorkflowRun`s (not `IWorkflowChain`s). Accepts sone soft of filter/query. May have a few overloads.
        /// Need to gain a better understanding of the underlying gRPC List/Scan APIs to design the most appropriate shape.
        /// May also have a `CountWorkflowRunsAsync(..)` equivalent if this makes sense from the query runtime perspective.
        /// </summary>        
        Task<IPaginatedReadOnlyCollectionPage<IWorkflowChain>> ListWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);

        // Research notes:
        // These are direct mappings of existing gRPC APIs for listing/scanning Workflow Runs:
        //Task<NeedsDesign> ListOpenWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);
        //Task<NeedsDesign> ListClosedWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);
        //Task<NeedsDesign> ListWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);
        //Task<NeedsDesign> ListArchivedWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);
        //Task<NeedsDesign> ScanWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);  // Difference ListWorkflowsAsync vs. ScanWorkflowsAsync
        //Task<NeedsDesign> CountWorkflowRunsAsync(NeedsDesign oneOrMoreArgs);  // What exactly does the GRPC API count?
        #endregion -- Workflow listing APIs --


        #region -- Server management APIs (not scoped to the bound namespace) --
        // These will not be part of V-Alpha.
        // Long-term they may be exposed either the underlyng GRPC cleint or here.
        // Expposing them here has some advantages:
        //  - All Temporal server access in in one place.
        //  - Flexibility to replace the common .NET gRPC client with Temporal Core to take advantage of the 
        //    shared functionality (metrics, reties, etc..)
        Task<NeedsDesign> GetClusterInfoAsync(NeedsDesign oneOrMoreArgs);
        Task<NeedsDesign> GetSystemInfoAsync(NeedsDesign oneOrMoreArgs);
        Task<NeedsDesign> GetSearchAttributesAsync(NeedsDesign oneOrMoreArgs);

        Task<NeedsDesign> RegisterNamespaceAsync(NeedsDesign oneOrMoreArgs);
        Task<NeedsDesign> ListNamespacesAsync(NeedsDesign oneOrMoreArgs);
        #endregion -- Server management APIs (not scoped to the bound namespace) --


        #region -- Server management APIs (scoped to the bound namespace) --
        // These will not be part of V-Alpha.
        // Same considerations apply as for the "server management APIs NOT scoped to the bound namespace" above.
        Task<NeedsDesign> DescribeTaskQueueAsync(NeedsDesign oneOrMoreArgs);
        #endregion -- Server management APIs (scoped to the bound namespace) --


        #region -- Namespace access APIs --
        // These will not be part of V-Alpha. Same considerations apply as for the "server management APIs" above.
        // Each Namespace access API has overloads that apply to the namespace "bound by the client" (see above)
        // and equivalent overloads that allow the namespace as a parameter.
        Task<NeedsDesign> DescribeNamespaceAsync(NeedsDesign oneOrMoreArgs);
        Task<NeedsDesign> DescribeNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs);

        Task<NeedsDesign> UpdateNamespaceAsync(NeedsDesign oneOrMoreArgs);
        Task<NeedsDesign> UpdateNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs);

        Task<NeedsDesign> DeprecateNamespaceAsync(NeedsDesign oneOrMoreArgs);  // Depricated in proto. Need it?
        Task<NeedsDesign> DeprecateNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs);  // Depricated in proto. Need it?
        #endregion -- Namespace access APIs --
    }

    public class TemporalServiceClient : ITemporalServiceClient
    {
        private static TemporalServiceClientConfiguration CreateDefaultConfiguration() { return new TemporalServiceClientConfiguration(); }

        public static async Task<TemporalServiceClient> ConnectNewAsync()
        {
            TemporalServiceClient client = new();
            await client.InitializeConnectionAsync();
            return client;
        }

        public static async Task<TemporalServiceClient> ConnectNewAsync(TemporalServiceClientConfiguration config)
        {
            TemporalServiceClient client = new(config);
            await client.InitializeConnectionAsync();
            return client;
        }

        public TemporalServiceClient() : this(CreateDefaultConfiguration()) { }

        public TemporalServiceClient(TemporalServiceClientConfiguration config) { }


        #region --- --- Interface implementation --- ---

        // ** All clarifications and comments are in the interface definition.
        // ** This section is just to make things compile for now.

        #region -- Connection management --
        public bool IsConectionInitialized { get; }
        public Task InitializeConnectionAsync() { return null; }
        #endregion -- Connection management --

        #region -- Namespace settings for the client --
        public string Namespace { get; }
        public Task<bool> TrySetNamespaceAsync(string @namespace) { return null; }
        public Task<bool> TrySetNamespaceAsync(string @namespace, CancellationToken cancelToken) { return null; }
        public Task<bool> IsNamespaceAccessibleAsync(string @namespace, CancellationToken cancelToken) { return null; }
        #endregion -- Namespace settings for the client --

        #region -- Workflow access and control APIs --

        #region StartWorkflowAsync(..)
        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue) { return null; }
        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue,
                                                       CancellationToken cancelToken) { return null; }
        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue,
                                                       IDataValue inputArgs) { return null; }
        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue,
                                                       IDataValue inputArgs,
                                                       CancellationToken cancelToken) { return null; }

        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue,
                                                       IDataValue inputArgs,
                                                       StartWorkflowChainConfiguration workflowConfig) { return null; }

        public Task<IWorkflowChain> StartWorkflowAsync(string workflowId,
                                                       string workflowTypeName,
                                                       string taskQueue,
                                                       IDataValue inputArgs,
                                                       StartWorkflowChainConfiguration workflowConfig,
                                                       CancellationToken cancelToken) { return null; }
        #endregion StartWorkflowAsync(..)

        #region StartWorkflowWithSignalAsync(..)
        public Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                                 string workflowTypeName,
                                                                 string taskQueue,
                                                                 string signalName,
                                                                 CancellationToken cancelToken) { return null; }

        public Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                                 string workflowTypeName,
                                                                 string taskQueue,
                                                                 string signalName,
                                                                 IDataValue signalArgs,
                                                                 CancellationToken cancelToken) { return null; }

        public Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                                 string workflowTypeName,
                                                                 string taskQueue,
                                                                 IDataValue wokflowArgs,
                                                                 string signalName,
                                                                 CancellationToken cancelToken) { return null; }

        public Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                                 string workflowTypeName,
                                                                 string taskQueue,
                                                                 IDataValue wokflowArgs,
                                                                 string signalName,
                                                                 IDataValue signalArgs,
                                                                 CancellationToken cancelToken) { return null; }

        public Task<IWorkflowChain> StartWorkflowWithSignalAsync(string workflowId,
                                                                 string workflowTypeName,
                                                                 string taskQueue,
                                                                 IDataValue wokflowArgs,
                                                                 string signalName,
                                                                 IDataValue signalArgs,
                                                                 StartWorkflowChainConfiguration workflowConfig,
                                                                 CancellationToken cancelToken) { return null; }
        #endregion StartWorkflowWithSignalAsync(..)


        #region CreateUnboundWorkflowHandle(..)
        public IWorkflowChain CreateUnboundWorkflowHandle(string workflowId) { return null; }
        #endregion CreateUnboundWorkflowHandle(..)

        #region CreateUnboundWorkflowRunHandle(..)
        public IWorkflowRun CreateUnboundWorkflowRunHandle(string workflowId,
                                                           string workflowRunId) { return null; }
        #endregion CreateUnboundWorkflowRunHandle(..)

        #region CreateUnboundWorkflowStub<TStub>(..)
        public TStub CreateUnboundWorkflowStub<TStub>(string workflowId) { return default(TStub); }

        public TStub CreateUnboundWorkflowStub<TStub>(string workflowId,
                                                      string workflowTypeName,
                                                      string taskQueue)  { return default(TStub); }

        public TStub CreateUnboundWorkflowStub<TStub>(string workflowId,
                                                      string workflowTypeName,
                                                      string taskQueue,
                                                      StartWorkflowChainConfiguration workflowConfig) { return default(TStub); }

        public TStub CreateUnboundWorkflowStub<TStub>(WorkflowChainBindingPolicy bindPolicy,
                                                      string workflowId,
                                                      string workflowTypeName,
                                                      string taskQueue) { return default(TStub); }

        public TStub CreateUnboundWorkflowStub<TStub>(WorkflowChainBindingPolicy bindPolicy,
                                                      string workflowId,
                                                      string workflowTypeName,
                                                      string taskQueue,
                                                      StartWorkflowChainConfiguration workflowConfig) { return default(TStub); }
        #endregion CreateUnboundWorkflowStub<TStub>(..)

        #region TryGetWorkflowAsync(..)
        public Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId) { return null; }

        public Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId,
                                                                      CancellationToken cancelToken) { return null; }

        public Task<TryGetResult<IWorkflowChain>> TryGetWorkflowAsync(string workflowId,
                                                                      string workflowChainId,
                                                                      CancellationToken cancelToken) { return null; }
        #endregion TryGetWorkflowAsync(..)

        #region TryGetWorkflowRunAsync(..)
        public Task<TryGetResult<IWorkflowRun>> TryGetWorkflowRunAsync(string workflowRunId,
                                                                       CancellationToken cancelToken) { return null; }
        public Task<TryGetResult<IWorkflowRun>> TryGetWorkflowRunAsync(string workflowId,
                                                                       string workflowRunId,
                                                                       CancellationToken cancelToken) { return null; }
        #endregion TryGetWorkflowRunAsync(..)
        
        #endregion -- Workflow access and control APIs --

        #region -- Workflow listing APIs --
        public Task<IPaginatedReadOnlyCollectionPage<IWorkflowChain>> ListWorkflowsAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<IPaginatedReadOnlyCollectionPage<IWorkflowChain>> ListWorkflowRunsAsync(NeedsDesign oneOrMoreArgs) { return null; }
        #endregion -- Workflow listing APIs --

        #region -- Server management APIs (not scoped to the bound namespace) --
        public Task<NeedsDesign> GetClusterInfoAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> GetSystemInfoAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> GetSearchAttributesAsync(NeedsDesign oneOrMoreArgs) { return null; }

        public Task<NeedsDesign> RegisterNamespaceAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> ListNamespacesAsync(NeedsDesign oneOrMoreArgs) { return null; }
        #endregion -- Server management APIs (not scoped to the bound namespace) --


        #region -- Server management APIs (scoped to the bound namespace) --
        public Task<NeedsDesign> DescribeTaskQueueAsync(NeedsDesign oneOrMoreArgs) { return null; }
        #endregion -- Server management APIs (scoped to the bound namespace) --


        #region -- Namespace access APIs --
        public Task<NeedsDesign> DescribeNamespaceAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> DescribeNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs) { return null; }

        public Task<NeedsDesign> UpdateNamespaceAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> UpdateNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs) { return null; }

        public Task<NeedsDesign> DeprecateNamespaceAsync(NeedsDesign oneOrMoreArgs) { return null; }
        public Task<NeedsDesign> DeprecateNamespaceAsync(string @namespace, NeedsDesign oneOrMoreArgs) { return null; }
        #endregion -- Namespace access APIs --

        #endregion --- --- Interface implementation --- ---
    }

    #endregion TemporalServiceClient


    #region class WorkflowChain

    /// <summary>
    /// Final type name is pending the terminology discussion.
    /// </summary>
    public interface IWorkflowChain
    {
        string Namespace { get; }
        string WorkflowId { get; }
        bool IsBound { get; }

        Task<string> GetWorkflowTypeNameAsync(CancellationToken cancelToken);

        /// <summary>Id of the first run in the chain.</summary>
        Task<string> GetWorkflowChainIdAsync();
        Task<string> GetWorkflowChainIdAsync(CancellationToken cancelToken);

        Task<bool> IsRunningAsync();
        Task<bool> IsRunningAsync(CancellationToken cancelToken);

        Task<WorkflowExecutionStatus> GetStatusAsync();
        Task<WorkflowExecutionStatus> GetStatusAsync(CancellationToken cancelToken);

        Task<WorkflowChainInfo> DescribeAsync();
        Task<WorkflowChainInfo> DescribeAsync(CancellationToken cancelToken);

        /// <summary>The returned stub is bound to this workflow chain.</summary>
        /// <seealso cref="TemporalServiceClient.CreateWorkflowStub{TStub}(String, String, String, CancellationToken)" />
        /// <remarks>See docs for `WorkflowXxxStubAttribute` for more detials on binding.</remarks>
        TStub GetStub<TStub>();

        #region StartAsync(..)
        /// <summary>If already bound - fail. Otherwise, start and bind to result.</summary>        
        Task StartAsync(string workflowTypeName,
                        string taskQueue,
                        IDataValue inputArgs,
                        StartWorkflowChainConfiguration workflowConfig,
                        CancellationToken cancelToken);
        #endregion StartAsync(..)

        #region StartIfNotRunningAsync(..)
        /// <summary>If already bound - fail. Otherwise, start and bind to result.
        /// If start fails due to already-running, then return false and don't bind, instead of throwing.</summary>
        Task<bool> StartIfNotRunningAsync(string workflowTypeName,
                                          string taskQueue);

        Task<bool> StartIfNotRunningAsync(string workflowTypeName,
                                          string taskQueue,
                                          IDataValue inputArgs);

        Task<bool> StartIfNotRunningAsync(string workflowTypeName,
                                          string taskQueue,
                                          IDataValue inputArgs,
                                          StartWorkflowChainConfiguration workflowConfig,
                                          CancellationToken cancelToken);
        #endregion StartIfNotRunningAsync(..)

        /// <summary>
        /// If already bound - just return (before checking cancelToken).
        /// If not bound and the WorkflowChainBindingPolicy REQUIRES starting new chain - fail telling to use the other overload.
        /// Otherwise - call describe to bind to most recent chain.
        /// </summary>        
        Task EnsureBoundAsync();
        Task EnsureBoundAsync(CancellationToken cancelToken);

        /// <summary>
        /// If already bound - just return (before checking cancelToken).
        /// If not bound and the WorkflowChainBindingPolicy REQUIRES starting new chain - delegate to StartAsync(..) but be
        /// resilient towards already-exists failures.
        /// Otherwise (OR if start fails with already-exists) - call describe to bind to most recent chain.
        /// </summary>        
        Task EnsureBoundAsync(string workflowTypeName,
                              string taskQueue,
                              IDataValue inputArgs,
                              StartWorkflowChainConfiguration workflowConfig,
                              CancellationToken cancelToken);

        #region --- GetXxxRunAsync(..) APIs to access a specific run ---

        /// <summary>Get the run with the specified run-id. Throw if not found.</summary>
        Task<IWorkflowRun> GetRunAsync(string workflowRunId, CancellationToken cancelToken);

        /// <summary>Get the run with the specified run-id. Return false if not found.</summary>
        Task<TryGetResult<IWorkflowRun>> TryGetRunAsync(string workflowRunId, CancellationToken cancelToken);

        /// <summary>Get the first / initial run in this chain.</summary>
        Task<IWorkflowRun> GetFirstRunAsync(CancellationToken cancelToken);

        /// <summary>Get the most recent run in this chain.</summary>
        Task<IWorkflowRun> GetLatestRunAsync(CancellationToken cancelToken);

        /// <summary>
        /// Get the very last run IF it is already known to be final (no further runs can/will follow).<br />
        /// If it is not yet known whether the latest run is final, this API will not fail, but it will return False.
        /// </summary>        
        Task<TryGetResult<IWorkflowRun>> TryGetFinalRunAsync(CancellationToken cancelToken);

        #endregion --- GetXxxRunAsync(..) APIs to access a specific run ---

        /// <summary>Lists runs of this consicution only. Needs overloads with filters? Not in V-Alpha.</summary>
        Task<IPaginatedReadOnlyCollectionPage<IWorkflowRun>> ListRunsAsync(NeedsDesign oneOrMoreArgs);

        #region --- APIs to interact with the chain ---

        // Invoking these APIs will interact with the currently active (aka latest, aka running) Run in this chain.
        // In all common scenarios this is what you want.
        // In some rare scenarios when you need to interact with a specific Run, obrain the corresponding IWorkflowRun instance
        // and invoke the corresponding API on that instance.

        /// <summary>The returned task completes when this chain finishes (incl. any runs not yet started). Performs long poll.</summary>
        Task<IWorkflowChainResult> GetResultAsync();
        Task<IWorkflowChainResult> GetResultAsync(CancellationToken cancelToken);
        Task<IWorkflowChainResult<TResult>> GetResultAsync<TResult>() where TResult : IDataValue;
        Task<IWorkflowChainResult<TResult>> GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;

        /// <summary>Get result if chain has finished. Otherwise return False. No long poll.</summary>
        Task<TryGetResult<IWorkflowChainResult>> TryGetResultIfAvailableAync();
        Task<TryGetResult<IWorkflowChainResult>> TryGetResultIfAvailableAync(CancellationToken cancelToken);
        Task<TryGetResult<IWorkflowChainResult<TResult>>> TryGetResultIfAvailableAync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;

        Task SignalAsync(string signalName, CancellationToken cancelToken);
        Task SignalAsync(string signalName, IDataValue arg);
        Task SignalAsync(string signalName, IDataValue arg, CancellationToken cancelToken);

        Task<TResult> QueryAsync<TResult>(string queryName) where TResult : IDataValue;
        Task<TResult> QueryAsync<TResult>(string queryName, CancellationToken cancelToken) where TResult : IDataValue;
        Task<TResult> QueryAsync<TResult>(string queryName, IDataValue arg, CancellationToken cancelToken) where TResult : IDataValue;

        Task RequestCancellationAsync();
        Task RequestCancellationAsync(CancellationToken cancelToken);

        Task TerminateAsync(string reason);
        Task TerminateAsync(string reason, IDataValue details, CancellationToken cancelToken);

        #endregion --- APIs to interact with the chain ---

        /// <summary>Gets the `TemporalServiceClient` that created this `IWorkflowChain`.</summary>
        /// <remarks>Do we need this? What is the scenario? Can it break encapsulation?</remarks>
        ITemporalServiceClient ServiceClient { get; }
    }
    #endregion class WorkflowChain


    #region class WorkflowRun
    public interface IWorkflowRun
    {
        string Namespace { get; }
        string WorkflowId { get; }
        string WorkflowRunId { get; }
        bool IsBound { get; }

        Task<IWorkflowChain> GetOwnerWorkflowAsync(CancellationToken cancelToken);

        Task<bool> IsRunningAsync(CancellationToken cancelToken);
        
        Task<WorkflowExecutionStatus> GetStatusAsync(CancellationToken cancelToken);
        Task<WorkflowRunInfo> DescribeAsync(CancellationToken cancelToken);

        /// <summary>
        /// If already bound - just return (before checking cancelToken).
        /// If not bound and the WorkflowChainBindingPolicy REQUIRES starting new chain:
        ///   - fail telling that starting must happen on Chain level.
        /// Otherwise - call describe. If run exists - bind. Otherwise fail.
        /// </summary>        
        Task EnsureBoundAsync(CancellationToken cancelToken);
        
        #region --- APIs to interact with the run ---

        // Invoking these APIs will interact with the this Workflow Run in this chain.
        // In all common scenarios this actually NOT what you want.
        // Instead, you want the corresponding API on IWorkflowChain, which will automatically select the
        // latest run withn the chain and interact with that.
        // In some rare scenarios when you need to interact with a specific Run, use the APIs below.

        /// <summary>The returned task completes when this chain finishes (incl. any runs not yet started). Performs long poll.</summary>        
        Task<IWorkflowRunResult> GetResultAsync(CancellationToken cancelToken);
        Task<IWorkflowRunResult<TResult>> GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;

        /// <summary>Get result if chain has finished. Otherwise return False. No long poll.</summary>        
        Task<TryGetResult<IWorkflowRunResult>> TryGetResultIfAvailableAync(CancellationToken cancelToken);
        Task<TryGetResult<IWorkflowRunResult<TResult>>> TryGetResultIfAvailableAync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;

        Task SignalAsync(string signalName, CancellationToken cancelToken);
        Task SignalAsync(string signalName, IDataValue arg, CancellationToken cancelToken);

        Task<TResult> QueryAsync<TResult>(string queryName, CancellationToken cancelToken) where TResult : IDataValue;
        Task<TResult> QueryAsync<TResult>(string queryName, IDataValue arg, CancellationToken cancelToken) where TResult : IDataValue;

        Task RequestCancellationAsync(CancellationToken cancelToken);

        Task TerminateAsync(string reason);
        Task TerminateAsync(string reason, IDataValue details, CancellationToken cancelToken);

        #endregion --- APIs to interact with the run ---
    }
    #endregion class WorkflowRun

    public sealed class WorkflowRunInfo
    {
        // @ToDo. Roughly corresponds to DescribeWorkflowExecutionResponse.
    }

    #region Workflow Routine Results

    public interface IWorkflowRoutineResult
    {
        PayloadsCollection ResultPayload { get; }
        WorkflowExecutionStatus Status { get; }
        Exception Failure { get; }
        IDataValue GetValue(); // Wraps if result was not IDataValue. Returns IDataValue.Void.Instance if there is no result value.
    }

    public interface IWorkflowRoutineResult<out TResult> : IWorkflowRoutineResult
                                                                    where TResult : IDataValue
    {
        TResult Value { get; }
    }

    public interface IWorkflowChainResult : IWorkflowRoutineResult
    {
        bool IsCompletedNormally { get; }
    }

    public interface IWorkflowChainResult<out TResult> : IWorkflowChainResult, IWorkflowRoutineResult<TResult>
                                                                    where TResult : IDataValue
    {
    }

    public interface IWorkflowRunResult : IWorkflowRoutineResult
    {
        bool IsCompletedNormally { get; }
        RetryState RetryState { get; }
        bool IsContinuedAsNew { get; }
        Task<TryGetResult<IWorkflowRun>> TryGetContinuationRunAsync();
    }

    public interface IWorkflowRunResult<out TResult> : IWorkflowRunResult, IWorkflowRoutineResult<TResult>
                                                                    where TResult : IDataValue
    {
    }

    #endregion Workflow Routine Results

    public class TemporalServiceClientConfiguration
    {
        public String ServiceUrl { get; init; }
        public bool IsHttpsEnabled { get; init; }
        public string Namespace { get; init; }

        /// <summary>
        /// Factory gets namespace, workflow type name, and workflow id; returns a new non-null data converter to be
        /// applied to ALL calls made by the client constructued with this config.
        /// </summary>
        public Func<string, string, string, IDataConverter> DataConverterFactory { get; init; }

        /// <summary>
        /// Factory gets namespace, workflow type name, workflow id, and a list of all already existing interceptors,
        /// i.e. the interceptors generated by the system. That interceptor list is never null, but may be empty.
        /// The interceptor list does NOT include the final "sink" interceptor - it will always remain LAST and the factory 
        /// shall not be able to affect that. However, the list DOES include all other system interceptors (e.g. the ones
        /// that implement distributed tracing. The factory may modify that list any way it wants, including removing
        /// system interceptors (expect the aforementioned sink) or adding new interceptors before or after.
        /// Nulls must not be added to the list.
        /// </summary>
        public Action<string, string, string, IList<ITemporalServiceClientInterceptor>> TemporalServiceClientInterceptorFactory { get; init; }

        // . . .
    }

    // ----------- -----------

    public interface IWorkflowChainStub
    {
        public interface IStartNewConfiguration
        {
            string WorkflowTypeName { get; }
            string TaskQueue { get; }
            StartWorkflowChainConfiguration WorkflowConfig { get; }
        }

        string WorkflowId { get; }
        bool IsBound { get; }
        WorkflowChainBindingPolicy BindingPolicy { get; }
        bool TryGetWorkflow(out IWorkflowChain workflow);
        bool TryGetStartNewConfiguration(out IStartNewConfiguration startNewConfiguration);
    }

    
    // ----------- -----------

    /// <summary>
    /// One instance if this interface will be created PER IWorkflowChain instance.
    /// Implementations need to design/use local state accordingly.
    /// 
    /// If we rename types `IWorkflowChain` or `IWorkflowRun`, rename this type and its APIs accordingly.
    /// This iface has one OnXxx API for each "primary" method on `IWorkflowChain` and `IWorkflowRun`.
    /// If a method has several overloads that delegate to one "most complete" overload, there is
    /// only a handler for that most complete overload.
    /// 
    /// The interceptor must have handlers for all relevant APIs where we support interception.
    /// For now we define only a subset of them, so that we have less work when the APIs of
    /// TemporalServiceClient, IWorkflowChain and IWorkflowRun change. We'll add the rest as soon as those APIs consolidate.
    /// </summary>
    public interface ITemporalServiceClientInterceptor
    {
        void Init(string @namespace, string workflowId, string workflowTypeName, ITemporalServiceClientInterceptor nextInterceptor);

        #region ---  TemporalServiceClient API interceptors ---
        Task<IWorkflowChain> OnServiceClient_StartWorkflowWithSignalAsync(string workflowId,
                                                                          string workflowTypeName,
                                                                          string taskQueue,
                                                                          IDataValue wokflowArgs,
                                                                          string signalName,
                                                                          IDataValue signalArgs,
                                                                          StartWorkflowChainConfiguration workflowConfig,
                                                                          CancellationToken cancelToken);
        Task<IWorkflowChain> OnServiceClient_GetWorkflowAsync(string workflowId,
                                                              string workflowChainId,
                                                              CancellationToken cancelToken);
        Task<IWorkflowRun> OnServiceClient_GetWorkflowRunAsync(string workflowId,
                                                               string workflowRunId,
                                                               CancellationToken cancelToken);
        // ...
        #endregion ---  TemporalServiceClient API interceptors ---

        #region ---  WorkflowChain API interceptors ---
        Task<IWorkflowRun> OnChain_GetRunAsync(string workflowRunId, CancellationToken cancelToken);
        Task<IWorkflowRun> OnChain_GetLatestRunAsync(CancellationToken cancelToken);
        Task<IWorkflowChainResult<TResult>> OnChain_GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;
        Task OnChain_SignalAsync(string signalName, IDataValue arg, CancellationToken cancelToken);
        Task OnChain_TerminateAsync(string reason, IDataValue details, CancellationToken cancelToken);
        // ...
        #endregion ---  WorkflowChain API interceptors ---


        #region ---  WorkflowRun API interceptors ---
        Task<WorkflowRunInfo> OnRun_GetInfoAsync();
        Task<IWorkflowRunResult<TResult>> OnRun_GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue;
        Task<TResult> OnRun_QueryAsync<TResult>(string queryName, IDataValue arg, CancellationToken cancelToken) where TResult : IDataValue;
        Task OnRun_RequestCancellationAsync(CancellationToken cancelToken);
        // ...
        #endregion ---  WorkflowRun API interceptors ---
    }

    public abstract class TemporalServiceClientInterceptorBase : ITemporalServiceClientInterceptor
    {
        private ITemporalServiceClientInterceptor _nextInterceptor = null;

        protected string Namespace { get; private set; }
        protected string WorkflowTypeName { get; private set; }
        protected string WorkflowId { get; private set; }
        protected IWorkflowChain Workflow { get; private set; }

        protected ITemporalServiceClientInterceptor NextInterceptor
        {
            get
            {
                if (_nextInterceptor == null)
                {
                    throw new InvalidOperationException($"The {nameof(NextInterceptor)} of this {this.GetType().Name} instance is null."
                                                       + " Was this instance initialized?");
                }

                return _nextInterceptor;
            }
        }

        public virtual void Init(string @namespace, string workflowId, string workflowTypeName, ITemporalServiceClientInterceptor nextInterceptor)
        {
            Namespace = @namespace;
            WorkflowId = workflowId;
            WorkflowTypeName = workflowTypeName;
            _nextInterceptor = nextInterceptor;
        }
        
        // The TemporalServiceClient APIs interceptors that result in creating a new `IWorkflowChain` instance must
        // initialize the `Workflow` field of this instance.
        public virtual async Task<IWorkflowChain> OnServiceClient_StartWorkflowWithSignalAsync(string workflowId,
                                                                                               string workflowTypeName,
                                                                                               string taskQueue,
                                                                                               IDataValue wokflowArgs,
                                                                                               string signalName,
                                                                                               IDataValue signalArgs,
                                                                                               StartWorkflowChainConfiguration workflowConfig,
                                                                                               CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            Workflow = await NextInterceptor.OnServiceClient_StartWorkflowWithSignalAsync(workflowId,
                                                                                          workflowTypeName,
                                                                                          taskQueue,
                                                                                          wokflowArgs,
                                                                                          signalName,
                                                                                          signalArgs,
                                                                                          workflowConfig,
                                                                                          cancelToken);
            return Workflow;
        }

        public virtual async Task<IWorkflowChain> OnServiceClient_GetWorkflowAsync(string workflowId,
                                                                                   string workflowChainId,
                                                                                   CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            Workflow = await NextInterceptor.OnServiceClient_GetWorkflowAsync(workflowId,
                                                                              workflowChainId,
                                                                              cancelToken);
            return Workflow;
        }
        public virtual Task<IWorkflowRun> OnServiceClient_GetWorkflowRunAsync(string workflowId,
                                                                              string workflowRunId,
                                                                              CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnServiceClient_GetWorkflowRunAsync(workflowId,
                                                                       workflowRunId,
                                                                       cancelToken);
        }

        // ...

        public virtual Task<IWorkflowRun> OnChain_GetLatestRunAsync(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnChain_GetLatestRunAsync(cancelToken);
        }

        public virtual Task<IWorkflowChainResult<TResult>> OnChain_GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnChain_GetResultAsync<TResult>(cancelToken);
        }

        public virtual Task<IWorkflowRun> OnChain_GetRunAsync(string workflowRunId, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnChain_GetRunAsync(workflowRunId, cancelToken);
        }

        public virtual Task OnChain_SignalAsync(string signalName, IDataValue arg, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnChain_SignalAsync(signalName, arg, cancelToken);
        }

        public virtual Task OnChain_TerminateAsync(string reason, IDataValue details, CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnChain_TerminateAsync(reason, details, cancelToken);
        }

        // ...

        public virtual Task<WorkflowRunInfo> OnRun_GetInfoAsync()
        {
            return NextInterceptor.OnRun_GetInfoAsync();
        }

        public virtual Task<IWorkflowRunResult<TResult>> OnRun_GetResultAsync<TResult>(CancellationToken cancelToken) where TResult : IDataValue
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnRun_GetResultAsync<TResult>(cancelToken);
        }

        public virtual Task<TResult> OnRun_QueryAsync<TResult>(string queryName, IDataValue arg, CancellationToken cancelToken) where TResult : IDataValue
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnRun_QueryAsync<TResult>(queryName, arg, cancelToken);
        }

        public virtual Task OnRun_RequestCancellationAsync(CancellationToken cancelToken)
        {
            cancelToken.ThrowIfCancellationRequested();
            return NextInterceptor.OnRun_RequestCancellationAsync(cancelToken);
        }

        // ...
    }

    // ----------- -----------

    public class StartWorkflowChainConfiguration
    {
        public static StartWorkflowChainConfiguration Default { get; }

        public TimeSpan WorkflowExecutionTimeout { get; init; }
        public TimeSpan WorkflowRunTimeout { get; init; }
        public TimeSpan WorkflowTaskTimeout { get; init; }
        public string Identity { get; init; }
        public WorkflowIdReusePolicy WorkflowIdReusePolicy { get; init; }
        public RetryPolicy RetryPolicy { get; init; }
        public string CronSchedule { get; init; }
        public Memo Memo { get; init; }
        public SearchAttributes SearchAttributes { get; init; }
        public Header Header { get; init; }

        // Settings that are ot user-facing:
        // string RequestId 
    }

    // ----------- -----------

    public class NeedsDesign
    {
        // Placeholder. @ToDo.
    }

    public class NeedsDesignException : Exception
    {
        // Placeholder. @ToDo.
    }
}
