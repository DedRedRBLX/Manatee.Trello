﻿/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		AttachmentCollection.cs
	Namespace:		Manatee.Trello
	Class Name:		ReadOnlyStickerCollection, StickerCollection
	Purpose:		Collection objects for attachments.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Trello.Exceptions;
using Manatee.Trello.Internal;
using Manatee.Trello.Internal.DataAccess;
using Manatee.Trello.Internal.Validation;
using Manatee.Trello.Json;
using Manatee.Trello.Rest;

namespace Manatee.Trello
{
	/// <summary>
	/// A read-only collection of attachments.
	/// </summary>
	public class ReadOnlyStickerCollection : ReadOnlyCollection<Sticker>
	{
		internal ReadOnlyStickerCollection(string ownerId)
			: base(ownerId) {}

		/// <summary>
		/// Implement to provide data to the collection.
		/// </summary>
		protected override sealed void Update()
		{
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Read_Stickers, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = JsonRepository.Execute<List<IJsonSticker>>(TrelloAuthorization.Default, endpoint);

			Items.Clear();
			Items.AddRange(newData.Select(ja =>
				{
					var attachment = TrelloConfiguration.Cache.Find<Sticker>(a => a.Id == ja.Id) ?? new Sticker(ja, OwnerId);
					attachment.Json = ja;
					return attachment;
				}));
		}
	}

	/// <summary>
	/// A collection of <see cref="Sticker"/>s.
	/// </summary>
	public class CardStickerCollection : ReadOnlyStickerCollection
	{
		private static readonly NumericRule<int> _rotationRule = new NumericRule<int>{Min = 0, Max = 359};

		internal CardStickerCollection(string ownerId)
			: base(ownerId) {}

		/// <summary>
		/// Adds a <see cref="Sticker"/> to a <see cref="Card"/>.
		/// </summary>
		/// <param name="name">The name of the sticker.</param>
		/// <param name="left">The position of the left edge.</param>
		/// <param name="top">The position of the top edge.</param>
		/// <param name="zIndex">The z-index. Default is 0.</param>
		/// <param name="rotation">The rotation. Default is 0.</param>
		/// <returns>The attachment generated by Trello.</returns>
		/// <exception cref="ValidationException{String}">Thrown when <paramref name="name"/> is null, empty, or whitespace.</exception>
		/// <exception cref="ValidationException{Int32}">Thrown when <paramref name="rotation"/> is less than 0 or greater than 359.</exception>
		public Sticker Add(string name, double left, double top, int zIndex = 0, int rotation = 0)
		{
			var error = NotNullOrWhiteSpaceRule.Instance.Validate(null, name);
			if (error != null)
				throw new ValidationException<string>(name, new[] {error});
			error = _rotationRule.Validate(null, rotation);
			if (error != null)
				throw new ValidationException<int>(rotation, new[] {error});

			var parameters = new Dictionary<string, object>
				{
					{"image", name},
					{"top", top},
					{"left", left},
					{"zIndex", zIndex},
					{"rotate", rotation},
				};
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Write_AddSticker, new Dictionary<string, object> {{"_id", OwnerId}});
			var newData = JsonRepository.Execute<IJsonSticker>(TrelloAuthorization.Default, endpoint, parameters);

			return new Sticker(newData, OwnerId);
		}
	}

	/// <summary>
	/// A collection of <see cref="Sticker"/>s.
	/// </summary>
	public class MemberStickerCollection : ReadOnlyStickerCollection
	{
		internal MemberStickerCollection(string ownerId)
			: base(ownerId) { }

		/// <summary>
		/// Adds a <see cref="Sticker"/> to a <see cref="Member"/>'s custom sticker set by uploading data.
		/// </summary>
		/// <param name="data">The byte data of the file to attach.</param>
		/// <param name="name">A name for the attachment.</param>
		/// <returns>The attachment generated by Trello.</returns>
		public Sticker Add(byte[] data, string name)
		{
			var parameters = new Dictionary<string, object> { { RestFile.ParameterKey, new RestFile { ContentBytes = data, FileName = name } } };
			var endpoint = EndpointFactory.Build(EntityRequestType.Card_Write_AddAttachment, new Dictionary<string, object> { { "_id", OwnerId } });
			var newData = JsonRepository.Execute<IJsonSticker>(TrelloAuthorization.Default, endpoint, parameters);

			return new Sticker(newData, OwnerId);
		}
	}
}