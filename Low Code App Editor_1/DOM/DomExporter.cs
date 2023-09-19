namespace Low_Code_App_Editor_1.DOM
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
    using Skyline.DataMiner.Net.Apps.Modules;
    using Skyline.DataMiner.Net.ManagerStore;
    using Skyline.DataMiner.Net.Messages;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;

    public class DomExporter
    {
        private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new DefaultContractResolver { IgnoreSerializableInterface = true },
        });

        private readonly ModuleSettingsHelper moduleSettingsHelper;
        private readonly Func<DMSMessage[], DMSMessage[]> sendSLNetMessages;
        private DomHelper domHelper;
        private bool includeInstances;
        private JsonTextWriter jsonTextWriter;
        private ItemProgressEventArgs itemProgressEventArgs;

        public DomExporter(
            ModuleSettingsHelper moduleSettingsHelper,
            Func<DMSMessage[], DMSMessage[]> sendSLNetMessages)
        {
            this.moduleSettingsHelper =
                moduleSettingsHelper ?? throw new ArgumentNullException(nameof(moduleSettingsHelper));
            this.sendSLNetMessages = sendSLNetMessages ?? throw new ArgumentNullException(nameof(sendSLNetMessages));
        }

        public event EventHandler<ItemProgressEventArgs> Progress;

        public string Export(IEnumerable<string> moduleIds, bool includeInstances = false)
        {
            try
            {
                this.includeInstances = includeInstances;
                InitProgressCounter();

                var result = String.Empty;
                using (var writer = new Writer())
                {
                    jsonTextWriter = writer.JsonTextWriter;

                    jsonTextWriter.WriteStartArray();
                    foreach (string moduleId in moduleIds)
                    {
                        ExportModule(moduleId);
                    }

                    jsonTextWriter.WriteEndArray();
                    result = writer.ToString();
                }

                return result;
            }
            catch (IOException e)
            {
                throw new DomEditorException(e.Message, e);
            }
            catch (CrudFailedException e)
            {
                throw new DomEditorException(String.Join("\n", e.TraceData.ErrorData), e);
            }
        }

        private void ExportModule(string moduleId)
        {
            jsonTextWriter.WriteStartObject();
            ExportModuleSettings(moduleId);
            domHelper = new DomHelper(sendSLNetMessages, moduleId);
            ExportSectionDefinitions();
            ExportDomBehaviorDefinitions();
            ExportDomDefinitions();
            ExportDomTemplates();
            if (includeInstances)
            {
                ExportDomInstances();
            }
            else
            {
                jsonTextWriter.WritePropertyName("DomInstances");
                jsonTextWriter.WriteStartArray();
                jsonTextWriter.WriteEndArray();
            }

            jsonTextWriter.WriteEndObject();
        }

        private void ExportDomTemplates()
        {
            ExportPaged("DomTemplates", domHelper.DomTemplates);
        }

        private void ExportSectionDefinitions()
        {
            ExportPaged("SectionDefinitions", domHelper.SectionDefinitions);
        }

        private void ExportDomBehaviorDefinitions()
        {
            ExportPaged("DomBehaviorDefinitions", domHelper.DomBehaviorDefinitions);
        }

        private void ExportDomDefinitions()
        {
            ExportPaged("DomDefinitions", domHelper.DomDefinitions);
        }

        private void ExportDomInstances()
        {
            ExportPaged("DomInstances", domHelper.DomInstances);
        }

        private void ExportModuleSettings(string moduleId)
        {
            jsonTextWriter.WritePropertyName("ModuleSettings");

            ModuleSettings moduleSettings = moduleSettingsHelper.ModuleSettings
                .Read(ModuleSettingsExposers.ModuleId.Equal(moduleId))
                .Single();

            JsonSerializer.Serialize(jsonTextWriter, moduleSettings);
            IncrementProgressCounter(1);
        }

        private void ExportPaged<T>(string name, ICrudHelperComponent<T> crudHelperComponent) where T : DataType
        {
            jsonTextWriter.WritePropertyName(name);
            PagingHelper<T> pagingHelper =
                crudHelperComponent.PreparePaging(new TRUEFilterElement<T>());

            jsonTextWriter.WriteStartArray();
            while (pagingHelper.MoveToNextPage())
            {
                List<T> dataTypes = pagingHelper.GetCurrentPage();
                foreach (T dataType in dataTypes)
                {
                    JsonSerializer.Serialize(jsonTextWriter, dataType);
                }

                IncrementProgressCounter(dataTypes.Count);
            }

            jsonTextWriter.WriteEndArray();
        }

        private void InitProgressCounter()
        {
            itemProgressEventArgs = new ItemProgressEventArgs(0);
            Progress?.Invoke(this, itemProgressEventArgs);
        }

        private void IncrementProgressCounter(int items)
        {
            itemProgressEventArgs.Items += items;
            Progress?.Invoke(this, itemProgressEventArgs);
        }

        private sealed class Writer : IDisposable
        {
            private StringBuilder sb;

            public Writer()
            {
                sb = new StringBuilder();
                JsonTextWriter = new JsonTextWriter(new StringWriter(sb));
            }

            public JsonTextWriter JsonTextWriter { get; }

            public void Dispose()
            {
                ((IDisposable)JsonTextWriter)?.Dispose();
            }

            public override string ToString()
            {
                return sb.ToString();
            }
        }
    }

    public class ItemProgressEventArgs : EventArgs
    {
        public ItemProgressEventArgs(int items) => Items = items;

        public int Items { get; internal set; }
    }

    [Serializable]
    public class DomEditorException : Exception
    {
        public DomEditorException()
        {
        }

        public DomEditorException(string message) : base(message)
        {
        }

        public DomEditorException(string message, Exception inner) : base(message, inner)
        {
        }

        protected DomEditorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}