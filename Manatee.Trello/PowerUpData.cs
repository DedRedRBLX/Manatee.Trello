using Manatee.Trello.Contracts;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.Synchronization;
using Manatee.Trello.Json;

namespace Manatee.Trello
{
	/// <summary>
	/// Provides the data supplied by the power-up.
	/// </summary>
	public class PowerUpData : ICacheable
	{
		private readonly Field<string> _pluginId;
		private readonly Field<string> _value;
		private readonly PowerUpDataContext _context;

		/// <summary>
		/// The ID of the data instance.
		/// </summary>
		public string Id { get; }
		/// <summary>
		/// The ID of the power-up.
		/// </summary>
		public string PluginId => _pluginId.Value;
		/// <summary>
		/// The value of the data.
		/// </summary>
		public string Value => _value.Value;

		internal IJsonPowerUpData Json
		{
			get { return _context.Data; }
			set { _context.Merge(value); }
		}

		internal PowerUpData(IJsonPowerUpData json, TrelloAuthorization auth)
		{
			Id = json.Id;
			_context = new PowerUpDataContext(Id, auth);

			_pluginId = new Field<string>(_context, nameof(PluginId));
			_value = new Field<string>(_context, nameof(Value));
		}
	}
}
