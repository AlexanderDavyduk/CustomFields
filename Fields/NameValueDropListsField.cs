// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NameValueListsField.cs">
//   Copyright (C) 2015 by Alexander Davyduk. All rights reserved.
// </copyright>
// <summary>
//   Defines the NameValueDroplistField class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.SharedSource.CustomFields.Fields
{
  using System.Collections.Specialized;
  using System.Linq;
  using System.Web;
  using Sitecore.Data.Fields;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines NameValueListsField field
  /// </summary>
  public class NameValueDroplistsField : NameValueListField
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="NameValueDroplistsField"/> class. 
    /// </summary>
    /// <param name="innerField">
    /// Inner field.
    /// </param>
    public NameValueDroplistsField(Field innerField)
      : base(innerField)
    {
    }

    /// <summary>
    /// Gets or sets a value indicating whether this <see cref="T:Sitecore.Data.Fields.CheckboxField" /> is checked.
    /// </summary>
    /// <value>
    ///   <c>true</c> if checked; otherwise, <c>false</c>.
    /// </value>
    public new NameValueCollection NameValues
    {
      get
      {
        var collection = StringUtil.ParseNameValueCollection(this.Value, '&', '=');

        var sortedCollection = new NameValueCollection();

        var items = collection.AllKeys.Where(k => !string.IsNullOrEmpty(k))
          .Select(k => new
          {
            Item = this.InnerField.Database.GetItem(k),
            Key = k
          });

        foreach (var item in items.OrderBy(i => i.Item == null ? 0 : i.Item.Appearance.Sortorder))
        {
          sortedCollection.Add(item.Key, HttpUtility.UrlDecode(collection[item.Key]) ?? string.Empty);
        }

        return sortedCollection;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.Value = StringUtil.NameValuesToString(value, "&");
      }
    }

    /// <summary>
    /// Performs an implicit conversion from <see cref="Field"/> to <see cref="NameValueDroplistsField"/>.
    /// </summary>
    /// <param name="field">The field.</param>
    /// <returns>
    /// The result of the conversion.
    /// </returns>
    public static implicit operator NameValueDroplistsField(Field field)
    {
      return field != null ? new NameValueDroplistsField(field) : null;
    }
  }
}