// Ignore Spelling: Json

namespace Install_1.DOM
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Serialization;

	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.Sections;

	public class DomImporter
	{
		private static readonly JsonSerializer JsonSerializer = JsonSerializer.Create(new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			ContractResolver = new DefaultContractResolver { IgnoreSerializableInterface = true },
		});

		private readonly ModuleSettingsHelper moduleSettingsHelper;
		private readonly Func<DMSMessage[], DMSMessage[]> sendSLNetMessages;
		private DomHelper domHelper;
		private ItemProgressEventArgs itemProgressEventArgs;
		private JsonTextReader jsonTextReader;

		public DomImporter(Func<DMSMessage[], DMSMessage[]> sendSLNetMessages)
		{
			this.sendSLNetMessages = sendSLNetMessages ?? throw new ArgumentNullException(nameof(sendSLNetMessages));
			moduleSettingsHelper = new ModuleSettingsHelper(sendSLNetMessages);
		}

		public event EventHandler<ItemProgressEventArgs> Progress;

		public void Import(string path)
		{
			InitProgressCounter();

			try
			{
				using (var reader = new Reader(path))
				{
					jsonTextReader = reader.JsonTextReader;
					jsonTextReader.Read(); // start array
					while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
					{
						ImportModule();
						jsonTextReader.Read(); // end object
					}
				}
			}
			catch (CrudFailedException e)
			{
				throw new DomEditorException(String.Join("\n", e.TraceData.ErrorData), e);
			}
			catch (IOException e)
			{
				throw new DomEditorException(e.Message, e);
			}
			catch (JsonException e)
			{
				throw new DomEditorException("File has an invalid structure.", e);
			}
		}

		private void Import<T>(ICrudHelperComponent<T> crudHelperComponent, FilterElement<T> equalityFilter, T dataType)
			where T : DataType
		{
			bool exists = crudHelperComponent.Read(equalityFilter).Any();

			if (exists)
			{
				crudHelperComponent.Update(dataType);
			}
			else
			{
				crudHelperComponent.Create(dataType);
			}

			IncrementProgressCounter(1);
		}

		private void ImportDomBehaviorDefinitions()
		{
			jsonTextReader.Read();
			jsonTextReader.Read();
			while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
			{
				var behaviorDefinition = JsonSerializer.Deserialize<DomBehaviorDefinition>(jsonTextReader);
				Import(
					domHelper.DomBehaviorDefinitions,
					DomBehaviorDefinitionExposers.Id.Equal(behaviorDefinition.ID),
					behaviorDefinition);
			}
		}

		private void ImportDomDefinitions()
		{
			jsonTextReader.Read();
			jsonTextReader.Read();
			while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
			{
				var domDefinition = JsonSerializer.Deserialize<DomDefinition>(jsonTextReader);
				Import(domHelper.DomDefinitions, DomDefinitionExposers.Id.Equal(domDefinition.ID), domDefinition);
			}
		}

		private void ImportDomTemplates()
		{
			jsonTextReader.Read();
			jsonTextReader.Read();
			while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
			{
				var domTemplate = JsonSerializer.Deserialize<DomTemplate>(jsonTextReader);
				Import(domHelper.DomTemplates, DomTemplateExposers.Id.Equal(domTemplate.ID), domTemplate);
			}
		}

		private void ImportDomInstances()
		{
			jsonTextReader.Read();
			jsonTextReader.Read();
			while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
			{
				var domInstance = JsonSerializer.Deserialize<DomInstance>(jsonTextReader);
				Import(domHelper.DomInstances, DomInstanceExposers.Id.Equal(domInstance.ID), domInstance);
			}
		}

		private void ImportModule()
		{
			jsonTextReader.Read(); // property name
			jsonTextReader.Read(); // start object
			var moduleSettings = JsonSerializer.Deserialize<ModuleSettings>(jsonTextReader);
			ImportModuleSettings(moduleSettings);
			domHelper = new DomHelper(sendSLNetMessages, moduleSettings.ModuleId);

			ImportSectionDefinitions();
			ImportDomBehaviorDefinitions();
			ImportDomDefinitions();
			ImportDomTemplates();
			ImportDomInstances();
		}

		private void ImportModuleSettings(ModuleSettings moduleSettings)
		{
			Import(
				moduleSettingsHelper.ModuleSettings,
				ModuleSettingsExposers.ModuleId.Equal(moduleSettings.ModuleId),
				moduleSettings);
		}

		private void ImportSectionDefinitions()
		{
			jsonTextReader.Read();
			jsonTextReader.Read();
			while (jsonTextReader.Read() && jsonTextReader.TokenType == JsonToken.StartObject)
			{
				var sectionDefinition = JsonSerializer.Deserialize<CustomSectionDefinition>(jsonTextReader);
				Import(
					domHelper.SectionDefinitions,
					SectionDefinitionExposers.ID.Equal(sectionDefinition.ID),
					sectionDefinition);
			}
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

		private sealed class Reader : IDisposable
		{
			private readonly FileStream fileStream;
			private readonly StreamReader streamReader;

			public Reader(string path)
			{
				try
				{
					fileStream = new FileStream(path, FileMode.Open);
					streamReader = new StreamReader(fileStream, Encoding.UTF8);
					JsonTextReader = new JsonTextReader(streamReader);
					JsonTextReader.SupportMultipleContent = true;
				}
				catch
				{
					Dispose();
					throw;
				}
			}

			public JsonTextReader JsonTextReader { get; }

			public void Dispose()
			{
				((IDisposable)JsonTextReader)?.Dispose();
				streamReader?.Dispose();
				fileStream?.Dispose();
			}
		}
	}
}